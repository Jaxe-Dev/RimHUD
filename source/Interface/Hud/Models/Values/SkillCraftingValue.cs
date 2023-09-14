using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillCraftingValue : SkillValue
  {
    private static readonly StatDef SmeltingSpeed = DefDatabase<StatDef>.GetNamed("SmeltingSpeed");
    private static readonly StatDef ButcheryMechanoidSpeed = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidSpeed");
    private static readonly StatDef ButcheryMechanoidEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidEfficiency");

    protected override Func<string?> Tooltip { get; }

    public SkillCraftingValue() : base(SkillDefOf.Crafting) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(SmeltingSpeed);
      builder.AppendStatLine(ButcheryMechanoidSpeed);
      builder.AppendStatLine(ButcheryMechanoidEfficiency);

      return builder.ToTooltip();
    }
  }
}
