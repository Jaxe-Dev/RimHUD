using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class NeedSuppressionBar : NeedBar
  {
    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    public NeedSuppressionBar() : base(Defs.NeedSuppression)
    {
      Tooltip = GetTooltip;

      OnClick = InspectPaneTabs.ToggleSlave;
    }

    private static string? GetTooltip()
    {
      var builder = new StringBuilder();

      builder.AppendStatLine(StatDefOf.SlaveSuppressionFallRate);
      builder.AppendStatLine(StatDefOf.Terror);

      return builder.ToTooltip();
    }
  }
}
