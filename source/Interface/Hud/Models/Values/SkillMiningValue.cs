using System;
using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillMiningValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillMiningValue() : base(SkillDefOf.Mining) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.MiningYield);
      builder.AppendStatLine(StatDefOf.MiningSpeed);
      builder.AppendStatLine(StatDefOf.DeepDrillingSpeed);

      return builder.ToTooltip();
    }
  }
}
