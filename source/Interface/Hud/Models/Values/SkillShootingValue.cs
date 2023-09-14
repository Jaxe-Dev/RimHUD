using System;
using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillShootingValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillShootingValue() : base(SkillDefOf.Shooting) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.ShootingAccuracyPawn);
      builder.AppendStatLine(StatDefOf.AimingDelayFactor);

      return builder.ToTooltip();
    }
  }
}
