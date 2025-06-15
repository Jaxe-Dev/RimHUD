using System;
using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class SkillAnimalsValue : SkillValue
{
  protected override Func<string?> Tooltip { get; }

  public SkillAnimalsValue() : base(SkillDefOf.Animals) => Tooltip = GetTooltip;

  private string? GetTooltip()
  {
    var builder = PrepareBuilder();

    builder.AppendStatLine(StatDefOf.AnimalGatherSpeed);
    builder.AppendStatLine(StatDefOf.AnimalGatherYield);
    builder.AppendStatLine(StatDefOf.TameAnimalChance);
    builder.AppendStatLine(StatDefOf.TrainAnimalChance);
    builder.AppendStatLine(StatDefOf.HuntingStealth);

    return builder.ToStringTrimmedOrNull();
  }
}
