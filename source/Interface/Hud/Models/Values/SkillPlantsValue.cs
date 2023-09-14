using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillPlantsValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillPlantsValue() : base(SkillDefOf.Plants) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.PlantWorkSpeed);
      builder.AppendStatLine(StatDefOf.PlantHarvestYield);
      if (ModsConfig.IdeologyActive) { builder.AppendStatLine(StatDefOf.PruningSpeed); }

      return builder.ToTooltip();
    }
  }
}
