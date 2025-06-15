using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using Verse;

namespace RimHUD;

public sealed class CustomValueDef : ExternalWidgetDef, IModel
{
  [DefaultValue(WidgetTextStyle.Small)]
  public WidgetTextStyle textStyle;

  [Unsaved]
  private ExternalMethodHandler<(string? label, string? value, Func<string?>? tooltip, Action? onHover, Action? onClick)>? _getParameters;

  public IWidget Build(HudArgs? args)
  {
    var parameters = _getParameters?.Invoke(Active.Pawn) ?? throw new Exception($"Error getting {nameof(CustomValueDef)} parameters.").AddData(resetOnly: true);
    return new ValueWidget(parameters.value is null ? null : parameters.label, parameters.value ?? parameters.label, parameters.tooltip, parameters.onHover, parameters.onClick, textStyle.GetActual());
  }

  protected override void InitializeV1() => _getParameters = GetHandler<(string? label, string? value, Func<string?>? tooltip, Action? onHover, Action? onClick)>(true, "GetParameters", typeof(Pawn));
}
