using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class ValueWidget(string? label, string? value, Func<string?>? tooltip, Action? onHover, Action? onClick, TextStyle textStyle) : StandardWidget(label, tooltip, textStyle)
{
  public override bool Draw(Rect rect)
  {
    var showLabel = Label is not null;

    var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

    if (showLabel) { DrawText(grid[1], Label); }
    DrawText(grid[2], value, alignment: showLabel ? TextAnchor.MiddleRight : null);

    if (HudLayout.IsMouseOverConfigButton) { return true; }

    if (Mouse.IsOver(rect)) { onHover?.Invoke(); }
    if (Verse.Widgets.ButtonInvisible(rect.ExpandedBy(GUIPlus.TinyPadding))) { onClick?.Invoke(); }
    TooltipsPlus.DrawCompact(grid[0], Tooltip);

    return true;
  }
}
