using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class SkillIntellectualValue : SkillValue
  {
    protected override Func<string?> Tooltip { get; }

    public SkillIntellectualValue() : base(SkillDefOf.Intellectual) => Tooltip = GetTooltip;

    private string? GetTooltip()
    {
      var builder = PrepareBuilder();

      builder.AppendStatLine(StatDefOf.ResearchSpeed);
      if (ModsConfig.IdeologyActive) { builder.AppendStatLine(StatDefOf.HackingSpeed); }

      return builder.ToTooltip();
    }
  }
}
