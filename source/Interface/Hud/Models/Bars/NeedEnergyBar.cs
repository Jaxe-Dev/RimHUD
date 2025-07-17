using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedEnergyBar : NeedBar
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleNeeds;

  public NeedEnergyBar() : base(Defs.NeedEnergy)
  { }

  private static string? GetTooltip()
  {
    if (Active.Pawn.needs?.energy?.FallPerDay is null) { return null; }

    var builder = new StringBuilder();
    builder.AppendValue("CurrentMechEnergyFallPerDay".TranslateSimple(), (Active.Pawn.needs.energy.FallPerDay / 100f).ToStringPercent());

    return builder.ToStringTrimmedOrNull();
  }
}
