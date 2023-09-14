using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillSocialValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillSocialValue() : base(SkillDefOf.Social) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.NegotiationAbility);
      builder.AppendStatLine(StatDefOf.TradePriceImprovement);
      builder.AppendStatLine(StatDefOf.Beauty);
      builder.AppendStatLine(StatDefOf.SocialImpact);
      if (ModsConfig.RoyaltyActive) { builder.AppendStatLine(StatDefOf.ConversionPower); }
      if (ModsConfig.IdeologyActive) { builder.AppendStatLine(StatDefOf.SuppressionPower); }

      return builder.ToTooltip();
    }
  }
}
