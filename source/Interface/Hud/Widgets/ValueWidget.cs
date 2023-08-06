using System;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets
{
  public class ValueWidget : StandardWidget
  {
    private readonly string _value;
    private readonly string _fallbackValue;
    private readonly Action _onClick;

    private ValueWidget(string label, Func<string> tooltip, string value, string fallbackValue, TextStyle textStyle, Action onClick) : base(label, tooltip, textStyle)
    {
      _value = value;
      _fallbackValue = fallbackValue;
      _onClick = onClick;
    }

    public static ValueWidget FromModel(IModelValue model, TextStyle textStyle) => model?.Hidden ?? true ? null : new ValueWidget(model.Label, model.Tooltip, model.Value, null, textStyle, model.OnClick);

    public override bool Draw(Rect rect)
    {
      if (_value.NullOrEmpty() && _fallbackValue == null) { return false; }

      var showLabel = Label != null;

      var grid = rect.GetHGrid(WidgetsPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

      if (showLabel) { DrawText(grid[1], Label); }
      DrawText(grid[2], _value, alignment: showLabel ? TextAnchor.MiddleRight : (TextAnchor?)null);

      if (!HudLayout.IsMouseOverConfigButton)
      {
        if (Verse.Widgets.ButtonInvisible(rect.ExpandedBy(WidgetsPlus.TinyPadding))) { _onClick?.Invoke(); }
        WidgetsPlus.DrawTooltip(grid[0], Tooltip, true);
      }

      return true;
    }
  }
}
