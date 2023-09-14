using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Bars
{
  public sealed class NeedRecreationBar : NeedBar
  {
    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    public NeedRecreationBar() : base(NeedDefOf.Joy)
    {
      Tooltip = GetTooltip;

      OnClick = InspectPaneTabs.ToggleNeeds;
    }

    private static string? GetTooltip()
    {
      var builder = new StringBuilder();
      builder.AppendNeedLine(Active.Pawn, Defs.NeedBeauty);
      builder.AppendNeedLine(Active.Pawn, Defs.NeedComfort);
      builder.AppendNeedLine(Active.Pawn, Defs.NeedOutdoors);

      return builder.ToTooltip();
    }
  }
}
