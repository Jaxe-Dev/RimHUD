using System;
using RimHUD.Access;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedMoodBar : NeedBar
{
  protected override float[]? Thresholds { get; }

  protected override Func<string?> Tooltip { get; }

  protected override Action OnClick { get; }

  public NeedMoodBar() : base(Defs.NeedMood)
  {
    if (Active.Pawn.mindState?.mentalBreaker is { } mental)
    {
      Thresholds =
      [
        mental.BreakThresholdMinor,
        mental.BreakThresholdMajor,
        mental.BreakThresholdExtreme
      ];
    }

    Tooltip = MentalTooltip.Get;

    OnClick = InspectPaneTabs.ToggleNeeds;
  }
}
