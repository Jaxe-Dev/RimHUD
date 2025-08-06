using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class QueuedValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    if (Active.Pawn.jobs?.curJob is null || Active.Pawn.jobs.jobQueue!.Count is 0) { return string.Empty; }

    var queued = Active.Pawn.jobs.jobQueue[0]!.job!.GetReport(Active.Pawn)?.TrimEnd('.').CapitalizeFirst();
    var remaining = Active.Pawn.jobs.jobQueue.Count - 1;
    if (remaining > 0) { queued += $" (+{remaining})"; }

    return queued is null ? string.Empty : "Queued".TranslateSimple().WithValue(queued.Bold());
  }
}
