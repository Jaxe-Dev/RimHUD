using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;

namespace RimHUD.Interface.Hud.Models.Bars;

public sealed class NeedRecreationBar : NeedBar
{
  protected override Func<string?> Tooltip { get; } = GetTooltip;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleNeeds;

  public NeedRecreationBar() : base(Defs.NeedRecreation)
  { }

  private static string? GetTooltip()
  {
    var builder = new StringBuilder();
    builder.AppendNeedLine(Active.Pawn, Defs.NeedBeauty);
    builder.AppendNeedLine(Active.Pawn, Defs.NeedComfort);
    builder.AppendNeedLine(Active.Pawn, Defs.NeedOutdoors);

    return builder.ToStringTrimmedOrNull();
  }
}
