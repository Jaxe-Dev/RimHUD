using System;
using System.Linq;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class TraitsValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    var traits = Active.Pawn.story?.traits?.TraitsSorted.Select(static trait => trait.LabelCap).ToArray();
    return traits?.Length > 0 ? traits.ToCommaList() : null;
  }

  private static string? GetTooltip()
  {
    var traits = Active.Pawn.story?.traits?.TraitsSorted;
    if (traits is null || !traits.Any()) { return null; }

    var builder = new StringBuilder();

    foreach (var trait in traits)
    {
      builder.AppendLine(trait.LabelCap.Colorize(ColoredText.TipSectionTitleColor));
      builder.AppendLine(trait.TipString(Active.Pawn).StripTags());
      builder.AppendLine();
    }

    return builder.ToStringTrimmedOrNull();
  }
}
