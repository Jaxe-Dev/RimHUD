using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedSuppressionBar : NeedBar
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleSlave;

  public NeedSuppressionBar() : base(Defs.NeedSuppression)
  { }

  private static string? GetTooltip()
  {
    var builder = new StringBuilder();

    builder.AppendStatLine(StatDefOf.SlaveSuppressionFallRate);
    builder.AppendStatLine(StatDefOf.Terror);

    return builder.ToStringTrimmedOrNull();
  }
}
