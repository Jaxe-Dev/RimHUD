using System;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedFoodBar : NeedBar
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleNeeds;

  public NeedFoodBar() : base(NeedDefOf.Food)
  { }

  private static string? GetTooltip()
  {
    var builder = new StringBuilder();
    if (Active.Pawn.RaceProps?.foodType is not null)
    {
      builder.AppendValue("Diet".TranslateSimple(), Active.Pawn.RaceProps.foodType.ToHumanString().CapitalizeFirst());
      builder.AppendLine();
    }

    builder.AppendStatLine(StatDefOf.EatingSpeed);
    builder.AppendStatLine(StatDefOf.BedHungerRateFactor);

    return builder.ToStringTrimmedOrNull();
  }
}
