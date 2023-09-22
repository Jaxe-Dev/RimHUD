using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;
using Verse;

namespace RimHUD
{
  public sealed class CustomSelectorDef : ExternalWidgetDef, IModel
  {
    [DefaultValue(WidgetTextStyle.Small)]
    public WidgetTextStyle textStyle;

    [Unsaved]
    private ExternalMethodHandler<(string? label, Func<string?>? tooltip, Action? onClick, Action? onHover, Color? backColor)>? _getParameters;

    public IWidget Build(HudArgs? args)
    {
      var parameters = _getParameters?.Invoke(Active.Pawn) ?? throw new Exception($"Error getting {nameof(CustomSelectorDef)} parameters.").AddData(resetOnly: true);
      return new SelectorWidget(parameters.label, parameters.tooltip, parameters.onHover, parameters.onClick, textStyle.GetActual(), parameters.backColor);
    }

    protected override void InitializeV1() => _getParameters = GetHandler<(string? label, Func<string?>? tooltip, Action? onClick, Action? onHover, Color? backColor)>(true, "GetParameters", typeof(Pawn));
  }
}
