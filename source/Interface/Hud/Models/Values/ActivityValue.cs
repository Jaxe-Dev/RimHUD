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

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class ActivityValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = static () => Find.MainTabsRoot!.SetCurrentTab(Defs.MainButtonWork);

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    var hasJob = Active.Pawn.CurJobDef is not null;
    var lord = hasJob ? Active.Pawn.GetLord()?.LordJob?.GetReport(Active.Pawn)?.CapitalizeFirst() : null;
    var jobText = hasJob ? Active.Pawn.jobs?.curDriver?.GetReport()?.TrimEnd('.').CapitalizeFirst() : null;

    var target = Active.Pawn.IsAttacking() ? Active.Pawn.TargetCurrentlyAimingAt.Thing?.LabelShortCap : null;

    var activity = target is null ? lord.NullOrWhitespace() ? jobText : $"{lord} ({jobText})" : Lang.Get("Model.Info.Attacking", target);
    return activity is null ? string.Empty : Lang.Get("Model.Info.Activity").WithValue(activity.Bold());
  }

  private static string? GetTooltip()
  {
    var work = Active.Pawn.CurJob?.workGiverDef?.Worker?.def?.workType;
    if (work is null) { return null; }

    var label = work.labelShort.CapitalizeFirst();

    var builder = new StringBuilder();
    builder.AppendValue(Lang.Get("Model.Info.Activity.WorkType"), label);

    if (work.relevantSkills is { } relevantSkills) { builder.AppendValue(Lang.Get("Model.Info.Activity.RelevantSkills"), relevantSkills.Select(static skill => skill.LabelCap.ToString()).ToCommaList()); }

    return builder.ToStringTrimmedOrNull();
  }
}
