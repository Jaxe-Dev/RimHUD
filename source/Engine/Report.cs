using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Engine
{
  public static class Report
  {
    private const string ExceptionDataResetOnlyName = "ResetOnly";

    public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
    public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
    public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
    public static void ErrorOnce(string message) => Verse.Log.ErrorOnce(PrefixMessage(message), message.GetHashCode());
    public static void Alert(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);
    private static string? PrefixMessage(string message) => message.NullOrEmpty() ? null : $"[{Mod.Name} v{Mod.Version}] {message}";

    public static void HandleWarning(Exception exception)
    {
      if (!Prefs.DevMode) { return; }
      if (Mod.DevMode) { HandleError(exception); }
      else { Warning($"Non-critical exception:\n{exception.Message}\n\nTrace:\n{exception.StackTrace}"); }
    }

    public static void HandleError(Exception exception)
    {
      State.Activated = false;

      Dialog_Error.Open(new ErrorInfo(exception));
    }

    public static Exception AddData(this Exception self, string? prefix = null, bool resetOnly = false)
    {
      if (prefix is not null) { Traverse.Create(self)!.Field("_message")!.SetValue(prefix + self.Message); }
      if (resetOnly) { self.Data[ExceptionDataResetOnlyName] = true; }
      return self;
    }

    public sealed class ErrorInfo
    {
      private const string TriggerExternal = "The error appears to have triggered outside of RimHUD";
      private const string TriggerMod = "This error appears to have been triggered by";

      private static readonly Regex TraceLineExternalModRegex = new("^\\s*at (?:\\([^)]+\\) )?(\\w+)\\.\\w+", RegexOptions.Multiline);
      private static readonly Regex TraceLineTidyRegex = new("( \\[[^]]+\\] in <[^>]+>:\\d+)", RegexOptions.Multiline);

      public string Message { get; }
      public string Trace { get; }
      public string? Notice { get; }

      public bool IsResetOnly { get; }

      private readonly string? _externalModInvolved;

      public ErrorInfo(Exception exception)
      {
        var baseException = exception.GetBaseException();
        Message = exception.Message + (exception == baseException ? null : $"\n{baseException.Message}");

        var isResetOnly = false;

        void CheckIsResetOnly(Exception currentException)
        {
          if (!isResetOnly) { isResetOnly = currentException.Data[ExceptionDataResetOnlyName] as bool? ?? false; }
        }

        CheckIsResetOnly(exception);
        var isExternal = !string.Equals(exception.Source, Mod.Id);
        _externalModInvolved = isExternal ? LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies!.loadedAssemblies.Any(assembly => assembly.GetName().Name == exception.Source))?.Name : null;

        var details = new StringBuilder();
        void AppendException(Exception currentException) => details.AppendLine($"[{currentException.Source}] {currentException.Message}\n{currentException.StackTrace}");

        AppendException(exception);

        if (exception.InnerException is not null)
        {
          var innerException = exception.InnerException;
          while (innerException.InnerException is not null)
          {
            innerException = innerException.InnerException;
            details.AppendLine();
            AppendException(innerException);
            CheckIsResetOnly(innerException);
          }
        }

        IsResetOnly = isResetOnly;
        Trace = TraceLineTidyRegex.Replace(details.ToString(), string.Empty);

        var matches = TraceLineExternalModRegex.Matches(Trace);
        if (matches.Count is 0) { return; }

        foreach (Match match in matches)
        {
          if (Reflection.GetModFromAssemblyName(match.Groups[1].Value) is not { } possible) { continue; }

          _externalModInvolved = $"{possible.Name} [{possible.PackageId}]";
          break;
        }

        if (IsResetOnly) { Notice = "Clicking Reactivate will reset your config in order to avoid this error".Colorize(Color.yellow); }
        else if (isExternal) { Notice = (_externalModInvolved is null ? TriggerExternal : $"{TriggerMod}:\n{_externalModInvolved.Bold()}").Colorize(Color.yellow); }
      }

      public void CopyToClipboard() => GUIUtility.systemCopyBuffer = $"[[RimHUD v{Mod.Version} Auto-deactivation report]]\n{(_externalModInvolved is null ? null : $"({TriggerMod}'{_externalModInvolved}')\n\n")}{Message}\n\n{nameof(Trace)}:\n{Trace}";
    }
  }
}
