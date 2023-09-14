using System;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class NeedSleepBar : NeedBar
  {
    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    public NeedSleepBar() : base(NeedDefOf.Rest)
    {
      Tooltip = GetTooltip;

      OnClick = InspectPaneTabs.ToggleNeeds;
    }

    private static string? GetTooltip()
    {
      var builder = new StringBuilder();
      builder.AppendStatLine(StatDefOf.RestRateMultiplier);

      return builder.ToTooltip();
    }
  }
}
