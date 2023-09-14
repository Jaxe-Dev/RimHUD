using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class MentalConditionValue : ValueModel
  {
    private const float MoodHappyLevel = 0.9f;
    private const float MoodContentLevel = 0.65f;

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
      if (Active.Pawn.mindState!.mentalStateHandler!.InMentalState) { return Active.Pawn.mindState.mentalStateHandler.CurState!.InspectLine.Colorize(Active.Pawn.mindState.mentalStateHandler.CurState.def!.IsAggro || Active.Pawn.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value); }

      if (Active.Pawn.needs?.mood is null || Active.Pawn.mindState?.mentalBreaker is null) { return null; }

      if (Active.Pawn.mindState.mentalBreaker?.BreakExtremeIsImminent ?? true) { return Lang.Get("Model.Mood.ExtremeBreakImminent").Colorize(Theme.CriticalColor.Value); }
      if (Active.Pawn.mindState.mentalBreaker?.BreakMajorIsImminent ?? true) { return Lang.Get("Model.Mood.MajorBreakImminent").Colorize(Theme.WarningColor.Value); }
      if (Active.Pawn.mindState.mentalBreaker?.BreakMinorIsImminent ?? true) { return Lang.Get("Model.Mood.MinorBreakImminent").Colorize(Theme.WarningColor.Value); }

      return GetInspiration() ?? Active.Pawn.needs.mood.CurLevel switch
      {
        > MoodHappyLevel => Lang.Get("Model.Mood.Happy").Colorize(Theme.ExcellentColor.Value),
        > MoodContentLevel => Lang.Get("Model.Mood.Content").Colorize(Theme.GoodColor.Value),
        _ => Lang.Get("Model.Mood.Indifferent").Colorize(Theme.InfoColor.Value)
      };
    }

    private static string? GetInspiration() => GetInspirationInspectLine()?.Colorize(Theme.ExcellentColor.Value);

    private static string? GetInspirationInspectLine()
    {
      try { return Active.Pawn.Inspired ? Active.Pawn.Inspiration?.InspectLine : null; }
      catch { return null; }
    }
  }
}
