using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedEnergyBar() : NeedBar(Defs.NeedEnergy)
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleNeeds;

  private static string? GetTooltip()
  {
    if (Active.Pawn.needs?.energy?.FallPerDay is null) { return null; }

    var builder = new StringBuilder();
    builder.AppendValue("CurrentMechEnergyFallPerDay".Translate(), (Active.Pawn.needs.energy.FallPerDay / 100f).ToStringPercent());

    return builder.ToStringTrimmedOrNull();
  }
}
