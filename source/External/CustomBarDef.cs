using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using Verse;

namespace RimHUD
{
  public sealed class CustomBarDef : ExternalWidgetDef, IModel
  {
    [DefaultValue(WidgetTextStyle.Regular)]
    public WidgetTextStyle textStyle;

    public BarColorStyle colorStyle;

    [Unsaved]
    private ExternalMethodHandler<(string? label, string? value, float fill, float[]? thresholds, Func<string?>? tooltip, Action? onHover, Action? onClick)>? _getParameters;

    public IWidget Build(HudArgs? args)
    {
      var parameters = _getParameters?.Invoke(Active.Pawn) ?? throw new Exception($"Error getting {nameof(CustomBarDef)} parameters.").AddData(resetOnly: true);
      return new BarWidget(parameters.label, parameters.value, parameters.fill, parameters.thresholds, parameters.tooltip, parameters.onHover, parameters.onClick, textStyle.GetActual(), colorStyle);
    }

    protected override void InitializeV1()
    {
      _getParameters = GetHandler<(string? label, string? value, float fill, float[]? thresholds, Func<string?>? tooltip, Action? onHover, Action? onClick)>(true, "GetParameters", typeof(Pawn));
    }
  }
}
