using System;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedSleepBar : NeedBar
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleNeeds;

  public NeedSleepBar() : base(NeedDefOf.Rest)
  { }

  private static string? GetTooltip()
  {
    var builder = new StringBuilder();
    builder.AppendStatLine(StatDefOf.RestRateMultiplier);

    return builder.ToStringTrimmedOrNull();
  }
}
