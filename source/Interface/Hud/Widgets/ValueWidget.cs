using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets
{
  public sealed class ValueWidget : StandardWidget
  {
    private readonly string? _value;

    private readonly Action? _onHover;
    private readonly Action? _onClick;

    public ValueWidget(string? label, string? value, Func<string?>? tooltip, Action? onHover, Action? onClick, TextStyle textStyle) : base(label, tooltip, textStyle)
    {
      _value = value;

      _onHover = onHover;
      _onClick = onClick;
    }

    public override bool Draw(Rect rect)
    {
      var showLabel = Label is not null;

      var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

      if (showLabel) { DrawText(grid[1], Label); }
      DrawText(grid[2], _value, alignment: showLabel ? TextAnchor.MiddleRight : null);

      if (HudLayout.IsMouseOverConfigButton) { return true; }

      if (Mouse.IsOver(rect)) { _onHover?.Invoke(); }
      if (Verse.Widgets.ButtonInvisible(rect.ExpandedBy(GUIPlus.TinyPadding))) { _onClick?.Invoke(); }
      TooltipsPlus.DrawCompact(grid[0], Tooltip);

      return true;
    }
  }
}
