using System;
using System.Linq;
using System.Text;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class ActivityValue : ValueModel
  {
    protected override string Value { get; }

    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public ActivityValue()
    {
      Value = GetValue();

      Tooltip = GetTooltip;

      OnClick = static () => Find.MainTabsRoot!.SetCurrentTab(Defs.MainButtonWork);
    }

    private static string GetValue()
    {
      var lord = Active.Pawn.GetLord()?.LordJob?.GetReport(Active.Pawn)?.CapitalizeFirst();
      var jobText = Active.Pawn.jobs?.curDriver?.GetReport()?.TrimEnd('.').CapitalizeFirst();
      var target = Active.Pawn.IsAttacking() ? Active.Pawn.TargetCurrentlyAimingAt.Thing?.LabelShortCap : null;
      var activity = target is null ? lord.NullOrWhitespace() ? jobText : $"{lord} ({jobText})" : Lang.Get("Model.Info.Attacking", target);

      return activity is null ? string.Empty : Lang.Get("Model.Info.Activity", activity.Bold());
    }

    private static string? GetTooltip()
    {
      try
      {
        if (Active.Pawn.CurJob?.workGiverDef?.Worker?.def?.workType?.labelShort is null) { return null; }

        var work = Active.Pawn.CurJob.workGiverDef.Worker.def.workType.labelShort.CapitalizeFirst();

        var builder = new StringBuilder();
        builder.AppendLine(Lang.Get("Model.Info.Activity.WorkType", work));

        if (Active.Pawn.CurJob.workGiverDef.Worker.def.workType.relevantSkills is { } relevantSkills) { builder.AppendLine(Lang.Get("Model.Info.Activity.RelevantSkills", relevantSkills.Select(static skill => skill.LabelCap.ToString()).ToCommaList())); }

        return builder.ToTooltip();
      }
      catch (Exception exception)
      {
        Report.HandleWarning(exception);
        return null;
      }
    }
  }
}
