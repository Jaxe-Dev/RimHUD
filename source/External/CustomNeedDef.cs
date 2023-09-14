using System;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Models.Bars;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD
{
  public sealed class CustomNeedDef : ExternalWidgetDef, IModel
  {
    public NeedDef? needDef;

    public BarColorStyle? colorStyle;

    [Unsaved]
    private ExternalMethodHandler<string>? _getTooltip;

    private Func<string?> GetTooltip => () => _getTooltip?.Invoke(Active.Pawn);

    public IWidget? Build(HudArgs args) => new NeedBar(needDef!, GetTooltip, colorStyle).Build(args);

    protected override void InitializeV1()
    {
      if (needDef is null) { throw new Exception("NeedDef is missing."); }

      _getTooltip = GetHandler<string>(false, "GetTooltip", typeof(Pawn));
    }
  }
}
