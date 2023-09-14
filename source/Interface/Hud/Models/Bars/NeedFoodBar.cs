using System;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class NeedFoodBar : NeedBar
  {
    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    public NeedFoodBar() : base(NeedDefOf.Food)
    {
      Tooltip = GetTooltip;

      OnClick = InspectPaneTabs.ToggleNeeds;
    }

    private static string? GetTooltip()
    {
      var builder = new StringBuilder();
      if (Active.Pawn.RaceProps?.foodType is not null)
      {
        builder.AppendValue("Diet".Translate(), Active.Pawn.RaceProps.foodType.ToHumanString().CapitalizeFirst());
        builder.AppendLine();
      }

      builder.AppendStatLine(StatDefOf.EatingSpeed);
      builder.AppendStatLine(StatDefOf.BedHungerRateFactor);

      return builder.ToTooltip();
    }
  }
}
