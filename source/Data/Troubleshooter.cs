using System;
using System.Linq;
using System.Text.RegularExpressions;
using RimHUD.Interface.Dialog;
using Verse;

namespace RimHUD.Data
{
  internal static class Troubleshooter
  {
    private static readonly Regex StackTraceLine = new Regex("^\\s*at (?:\\([^)]+\\) )?(\\w+)\\.\\w+", RegexOptions.Multiline);

    public static void HandleWarning(Exception exception)
    {
      if (!Prefs.DevMode) { return; }
      if (Mod.DevMode) { HandleError(exception); }
      else { Mod.Warning($"Non-critical exception:\n{exception.Message}\n\nStacktrace:\n{exception.StackTrace}"); }
    }

    public static void HandleError(Exception exception)
    {
      State.Activated = false;
      Dialog_Error.Open(new ExceptionInfo(exception));
    }

    public class ExceptionInfo
    {
      public string Message { get; }
      public string StackTrace { get; }
      public bool IsExternalError { get; }
      public string PossibleMod { get; }
      public string Text => (PossibleMod == null ? null : $"(This error appears to have been triggered by {PossibleMod})\n\n") + Message + "\n\nStacktrace:\n" + StackTrace;

      public ExceptionInfo(Exception exception)
      {
        Message = exception.Message;
        StackTrace = BuildStacktrace(exception);

        IsExternalError = !string.Equals(exception.Source, Mod.Id);

        PossibleMod = IsExternalError ? LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.Any(assembly => assembly.GetName().Name == exception.Source))?.Name : null;

        if (exception.InnerException != null)
        {
          var innerException = exception.InnerException;
          var level = 1;
          do
          {
            Message += $"\n{new string('+', level)} [{innerException.Source}] {innerException.Message}";
            StackTrace += "\n\n" + BuildStacktrace(innerException);
            level++;
            innerException = innerException.InnerException;
          } while (innerException != null);
        }

        var matches = StackTraceLine.Matches(StackTrace);
        if (matches.Count == 0) { return; }

        foreach (Match match in matches)
        {
          var possible = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == match.Groups[1].Value) != null);
          if (possible == null || string.Equals(possible.PackageId, Mod.PackageId, StringComparison.OrdinalIgnoreCase)) { continue; }

          PossibleMod = $"{possible.Name} [{possible.PackageId}]";
          break;
        }
      }

      private static string BuildStacktrace(Exception exception) => $"[{exception.Source}: {exception.Message}]\n{exception.StackTrace}";
    }
  }
}
