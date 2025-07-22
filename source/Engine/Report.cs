using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Engine;

public static class Report
{
  private const string ExceptionDataResetOnlyName = "ResetOnly";

  public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
  public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
  public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
  public static void ErrorOnce(string message) => Verse.Log.ErrorOnce(PrefixMessage(message), message.GetHashCode());
  public static void Alert(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);
  private static string? PrefixMessage(string message) => message.NullOrEmpty() ? null : $"[{Mod.Name} v{Mod.Version}] {message}";

  public static void HandleError(System.Exception exception)
  {
    State.Activated = false;
    Dialog_Error.Open(new ErrorInfo(exception));
  }

  private static System.Exception GetDeepestException(System.Exception ex)
  {
    while (ex.InnerException is not null) { ex = ex.InnerException; }
    return ex;
  }

  public static System.Exception AddData(this System.Exception self, string? prefix = null, bool resetOnly = false)
  {
    if (prefix is not null) { Traverse.Create(self)!.Field("_message")!.SetValue(prefix + self.Message); }
    if (resetOnly) { self.Data[ExceptionDataResetOnlyName] = true; }
    return self;
  }

  public sealed class ErrorInfo
  {
    private const string TriggerExternal = "The error appears to have triggered outside of RimHUD";
    private const string TriggerMod = "This error appears to have been triggered by";
    private const string TriggerModExtra = "Please consider reporting this issue to the relevant mod author(s).";
    private const string NoticeReactivate = "Clicking Reactivate will reset your config in order to avoid this error.";
    private const string NoticeDuplicate = "This is a duplicate error and no longer contains useful information to report.";

    private const string DuplicateLine = "Duplicate stacktrace, see ref for original";

    private static readonly Regex RegexExternal = new(@"^\s*(?:- )?(?:at|TRANSPILER|PREFIX|POSTFIX|FINALIZER)\s+(?:\([^\)]+\)\s+)?(\w+)", RegexOptions.Multiline);
    private static readonly Regex RegexDuplicate = new(@"^\s*\[Ref[^\]]+\] " + DuplicateLine, RegexOptions.Multiline);
    private static readonly Regex RegexTidy = new(@"( \[[^]]+\] in <[^>]+>:\d+|^\s*\[Ref \w+\]\s*$)", RegexOptions.Multiline);

    public string Message { get; }
    public string Trace { get; }
    public string? Notice { get; }
    public bool IsResetOnly { get; }
    public bool IsDuplicate { get; }

    private readonly string? _externalModInvolved;

    public ErrorInfo(System.Exception exception)
    {
      var root = GetDeepestException(exception);

      Message = exception.Message + (exception != root ? $"\n{root.Message}" : string.Empty);
      IsResetOnly = CheckIsResetOnly(exception);

      var details = new StringBuilder();
      var inner = exception;
      while (inner is not null)
      {
        details.AppendLine($"[{inner.GetType().Name}] {inner.Message}");
        details.AppendLine(inner.StackTrace ?? "(No stacktrace)");

        inner = inner.InnerException;
        if (inner is not null) { details.AppendLine(); }
      }

      Trace = RegexTidy.Replace(details.ToString(), string.Empty);

      if (RegexDuplicate.IsMatch(Trace))
      {
        IsDuplicate = true;
        Notice = NoticeDuplicate;

        throw new Exception("Repeated deactivations, check full log", exception);
      }

      _externalModInvolved = GetExternalMod(Trace);

      if (IsResetOnly) { Notice = NoticeReactivate.Colorize(Color.yellow); }
      else if (!string.Equals(exception.Source, nameof(RimHUD))) { Notice = (_externalModInvolved is null ? TriggerExternal : $"{TriggerMod}:\n\n{_externalModInvolved.Bold()}\n\n{TriggerModExtra}").Colorize(Color.yellow); }
    }

    private static bool CheckIsResetOnly(System.Exception? exception)
    {
      while (exception is not null)
      {
        if (exception.Data[ExceptionDataResetOnlyName] is true) { return true; }
        exception = exception.InnerException!;
      }

      return false;
    }

    private static string? GetExternalMod(string trace)
    {
      var matches = RegexExternal.Matches(trace);
      foreach (Match match in matches)
      {
        var name = match.Groups[1].Value;
        if (string.IsNullOrWhiteSpace(name) || name is nameof(RimHUD)) { continue; }

        var mod = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies!.loadedAssemblies.Any(assembly => assembly.GetName().Name == name));
        if (mod is { IsOfficialMod: false }) { return $"{mod.Name} [{mod.PackageId}]"; }
      }
      return null;
    }

    public void CopyToClipboard() => GUIUtility.systemCopyBuffer = $"[[RimHUD v{Mod.Version} Auto-deactivation report]]\n" + $"{(_externalModInvolved is null ? null : $"({TriggerMod} '{_externalModInvolved}')\n\n")}" + $"{Message}\n\nStacktrace:\n{Trace}";
  }

  public class Exception : System.Exception
  {
    public Exception(string message) : base(message)
    { }

    public Exception(string message, System.Exception innerException) : base(message, innerException)
    { }
  }
}
