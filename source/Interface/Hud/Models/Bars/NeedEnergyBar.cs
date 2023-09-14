using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class NeedEnergyBar : NeedBar
  {
    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    public NeedEnergyBar() : base(Defs.NeedEnergy)
    {
      Tooltip = GetTooltip;

      OnClick = InspectPaneTabs.ToggleNeeds;
    }

    private static string? GetTooltip()
    {
      if (Active.Pawn.needs?.energy?.FallPerDay is null) { return null; }

      var builder = new StringBuilder();
      builder.AppendValue("CurrentMechEnergyFallPerDay".Translate(), (Active.Pawn.needs.energy.FallPerDay / 100f).ToStringPercent());

      return builder.ToTooltip();
    }
  }
}
