using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class SkillCookingValue : SkillValue
{
  private static readonly StatDef CookSpeed = DefDatabase<StatDef>.GetNamed("CookSpeed");
  private static readonly StatDef ButcheryFleshSpeed = DefDatabase<StatDef>.GetNamed("ButcheryFleshSpeed");
  private static readonly StatDef ButcheryFleshEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryFleshEfficiency");
  private static readonly StatDef DrugCookingSpeed = DefDatabase<StatDef>.GetNamed("DrugCookingSpeed");

  protected override Func<string?> Tooltip { get; }

  public SkillCookingValue() : base(SkillDefOf.Cooking) => Tooltip = GetTooltip;

  private string? GetTooltip()
  {
    var builder = PrepareBuilder();

    builder.AppendStatLine(CookSpeed);
    builder.AppendStatLine(StatDefOf.FoodPoisonChance);
    builder.AppendStatLine(ButcheryFleshEfficiency);
    builder.AppendStatLine(ButcheryFleshSpeed);
    builder.AppendStatLine(DrugCookingSpeed);

    return builder.ToStringTrimmedOrNull();
  }
}
