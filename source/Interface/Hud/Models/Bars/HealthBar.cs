using System;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class HealthBar : BarModel
  {
    protected override string? Label { get; }
    protected override string? Value { get; }

    protected override float Fill { get; } = -1f;

    protected override Func<string?>? Tooltip { get; }

    protected override Action? OnClick { get; }

    public HealthBar()
    {
      if (Active.Pawn.health is null) { return; }

      Label = "Health".Translate();

      var percent = Active.Pawn.health?.summaryHealth?.SummaryHealthPercent ?? -1f;
      Value = percent < 0f ? null : percent.ToStringPercent();
      Fill = percent;

      Tooltip = HealthTooltip.Get;

      OnClick = InspectPaneTabs.ToggleHealth;
    }
  }
}
