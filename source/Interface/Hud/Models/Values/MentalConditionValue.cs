using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class MentalConditionValue : ValueModel
{
  private const float MoodExcellentLevel = 0.9f;
  private const float MoodGoodLevel = 0.65f;

  protected override string? Value { get; }

  protected override Func<string?>? Tooltip { get; }

  protected override Action? OnClick { get; }

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  public MentalConditionValue()
  {
    if (Active.Pawn.mindState?.mentalStateHandler is null) { return; }

    Value = GetValue();

    Tooltip = MentalTooltip.Get;

    OnClick = InspectPaneTabs.ToggleNeeds;
  }

  private static string? GetValue()
  {
    if (GetStun() is { } stun) { return stun; }

    if (Active.Pawn.mindState!.mentalStateHandler!.InMentalState) { return Active.Pawn.mindState.mentalStateHandler.CurState!.InspectLine.Colorize(Active.Pawn.mindState.mentalStateHandler.CurState.def!.IsAggro || Active.Pawn.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value); }

    if (Active.Pawn.needs?.mood is null || Active.Pawn.mindState?.mentalBreaker is null) { return null; }

    if (Active.Pawn.mindState.mentalBreaker?.BreakExtremeIsImminent ?? true) { return Lang.Get("Model.Mood.ExtremeBreakImminent").Colorize(Theme.CriticalColor.Value); }
    if (Active.Pawn.mindState.mentalBreaker?.BreakMajorIsImminent ?? true) { return Lang.Get("Model.Mood.MajorBreakImminent").Colorize(Theme.WarningColor.Value); }
    if (Active.Pawn.mindState.mentalBreaker?.BreakMinorIsImminent ?? true) { return Lang.Get("Model.Mood.MinorBreakImminent").Colorize(Theme.WarningColor.Value); }

    return GetInspiration() ?? Active.Pawn.needs.mood.CurLevel switch
    {
      > MoodExcellentLevel => Lang.Get("Model.Mood.Excellent").Colorize(Theme.ExcellentColor.Value),
      > MoodGoodLevel => Lang.Get("Model.Mood.Good").Colorize(Theme.GoodColor.Value),
      _ => Lang.Get("Model.Mood.Neutral").Colorize(Theme.InfoColor.Value)
    };
  }

  private static string? GetStun()
  {
    var stunner = Active.Pawn.stances?.stunner;
    if (stunner?.Stunned ?? false)
    {
      var duration = stunner.StunTicksLeft.ToStringSecondsFromTicks();
      return (stunner.Hypnotized ? "InTrance".TranslateSimple() : Lang.Get("Model.Mood.Stunned", duration)).Colorize(Theme.CriticalColor.Value);
    }

    var stagger = Active.Pawn.stances?.stagger;
    if (stagger?.Staggered ?? false) { return Lang.Get("Model.Mood.Staggered", stagger.StaggerTicksLeft.ToStringSecondsFromTicks()).Colorize(Theme.WarningColor.Value); }

    return null;
  }

  private static string? GetInspiration() => GetInspirationInspectLine()?.Colorize(Theme.ExcellentColor.Value);

  private static string? GetInspirationInspectLine()
  {
    if (!Active.Pawn.Inspired || Active.Pawn.Inspiration is null || Active.Pawn.InspirationDef is null) { return null; }

    var duration = ((int)((Active.Pawn.InspirationDef.baseDurationDays - Active.Pawn.Inspiration.AgeDays) * 60000)).ToStringTicksToPeriod();
    return Lang.Get("Model.Mood.Inspired", Active.Pawn.InspirationDef!.LabelCap, duration);
  }
}
