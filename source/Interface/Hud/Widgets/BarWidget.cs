using System;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class BarWidget : StandardWidget
{
  private readonly string? _value;
  private readonly float _fill;
  private readonly float[]? _thresholds;
  private readonly Action? _onHover;
  private readonly Action? _onClick;
  private readonly BarColorStyle _colorStyle;

  public BarWidget(string? label, string? value, float fill, float[]? thresholds, Func<string?>? tooltip, Action? onHover, Action? onClick, TextStyle textStyle, BarColorStyle colorStyle) : base(label, tooltip, textStyle)
  {
    _value = value;
    _fill = fill;
    _thresholds = thresholds;
    _onHover = onHover;
    _onClick = onClick;
    _colorStyle = colorStyle;
  }

  public override bool Draw(Rect rect)
  {
    if (_fill < 0f) { return false; }

    var percentage = _fill / 1f;

    var grid = rect.GetHGrid(GUIPlus.TinyPadding, Label is not null ? Theme.LabelWidth.Value : 0f, -1f, Theme.ValueWidth.Value);

    if (Label is not null) { DrawText(grid[1], Label); }
    WidgetsPlus.DrawBar(grid[2], percentage, _colorStyle.GetColor(percentage));
    DrawThresholds(grid[2]);
    if (_value is not null) { DrawText(grid[3], _value); }

    if (HudLayout.IsMouseOverConfigButton) { return true; }

    if (Mouse.IsOver(rect)) { _onHover?.Invoke(); }
    if (Verse.Widgets.ButtonInvisible(rect)) { _onClick?.Invoke(); }
    TooltipsPlus.DrawCompact(rect, Tooltip);

    return true;
  }

  private void DrawThresholds(Rect rect)
  {
    if (_thresholds is null) { return; }

    GUIPlus.SetColor(Theme.BarThresholdColor.Value);
    foreach (var threshold in _thresholds.Where(static threshold => threshold > 0f)) { Verse.Widgets.DrawLineVertical(Mathf.Round(rect.x + (rect.width * threshold)), rect.y, rect.height); }
    GUIPlus.ResetColor();
  }
}
