using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class QueuedValue : ValueModel
  {
    protected override string Value { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public QueuedValue() => Value = GetValue();

    private static string GetValue()
    {
      if (Active.Pawn.jobs?.curJob is null || Active.Pawn.jobs.jobQueue!.Count is 0) { return string.Empty; }

      var queued = Active.Pawn.jobs.jobQueue[0]!.job.GetReport(Active.Pawn)?.TrimEnd('.').CapitalizeFirst().Bold();
      var remaining = Active.Pawn.jobs.jobQueue.Count - 1;
      if (remaining > 0) { queued += $" (+{remaining})"; }

      return queued is null ? string.Empty : Lang.Get("Model.Info.Queued", queued);
    }
  }
}
