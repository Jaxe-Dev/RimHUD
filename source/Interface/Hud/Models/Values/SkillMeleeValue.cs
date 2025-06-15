using System;
using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class SkillMeleeValue : SkillValue
{
  protected override Func<string?> Tooltip { get; }

  public SkillMeleeValue() : base(SkillDefOf.Melee) => Tooltip = GetTooltip;

  private string? GetTooltip()
  {
    var builder = PrepareBuilder();

    builder.AppendStatLine(StatDefOf.MeleeDPS);
    builder.AppendStatLine(StatDefOf.MeleeHitChance);
    builder.AppendStatLine(StatDefOf.MeleeDodgeChance);

    return builder.ToStringTrimmedOrNull();
  }
}
