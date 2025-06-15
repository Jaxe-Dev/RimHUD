using System;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Bars;

public class NeedBar : BarModel
{
  protected override string? Label { get; }
  protected override string? Value { get; }

  protected override float Fill { get; } = -1f;
  protected override Func<string?>? Tooltip { get; }

  protected override BarColorStyle? ColorStyle { get; }

  public NeedBar(NeedDef def, Func<string?>? tooltip = null, BarColorStyle? colorStyle = null)
  {
    var need = Active.Pawn.needs?.TryGetNeed(def);
    if (need is null) { return; }

    Label = def.GetDefNameOrLabel();

    var percent = need.CurLevelPercentage;
    Value = percent.ToStringPercent();
    Fill = need.CurLevelPercentage;
    Tooltip = tooltip;

    ColorStyle = colorStyle;
  }
}
