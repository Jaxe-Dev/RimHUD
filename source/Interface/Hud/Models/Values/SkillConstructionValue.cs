using System;
using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillConstructionValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillConstructionValue() : base(SkillDefOf.Construction) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.ConstructSuccessChance);
      builder.AppendStatLine(StatDefOf.ConstructionSpeed);
      builder.AppendStatLine(StatDefOf.FixBrokenDownBuildingSuccessChance);
      builder.AppendStatLine(StatDefOf.SmoothingSpeed);

      return builder.ToTooltip();
    }
  }
}
