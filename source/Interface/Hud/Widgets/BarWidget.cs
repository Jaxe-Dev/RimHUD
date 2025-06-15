using System;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class BarWidget(string? label, string? value, float fill, float[]? thresholds, Func<string?>? tooltip, Action? onHover, Action? onClick, TextStyle textStyle, BarColorStyle colorStyle) : StandardWidget(label, tooltip, textStyle)
{
  public override bool Draw(Rect rect)
  {
    if (fill < 0f) { return false; }

    var percentage = fill / 1f;

    var grid = rect.GetHGrid(GUIPlus.TinyPadding, Label is not null ? Theme.LabelWidth.Value : 0f, -1f, Theme.ValueWidth.Value);

    if (Label is not null) { DrawText(grid[1], Label); }
    WidgetsPlus.DrawBar(grid[2], percentage, colorStyle.GetColor(percentage));
    DrawThresholds(grid[2]);
    if (value is not null) { DrawText(grid[3], value); }

    if (HudLayout.IsMouseOverConfigButton) { return true; }

    if (Mouse.IsOver(rect)) { onHover?.Invoke(); }
    if (Verse.Widgets.ButtonInvisible(rect)) { onClick?.Invoke(); }
    TooltipsPlus.DrawCompact(rect, Tooltip);

    return true;
  }

  private void DrawThresholds(Rect rect)
  {
    if (thresholds is null) { return; }

    GUIPlus.SetColor(Theme.BarThresholdColor.Value);
    foreach (var threshold in thresholds.Where(static threshold => threshold > 0f)) { Verse.Widgets.DrawLineVertical(Mathf.Round(rect.x + (rect.width * threshold)), rect.y, rect.height); }
    GUIPlus.ResetColor();
  }
}
