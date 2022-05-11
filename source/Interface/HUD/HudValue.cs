using System;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
  internal class HudValue : HudFeature
  {
    private readonly string _value;
    private readonly string _fallbackValue;
    private readonly Action _onClick;

    private HudValue(string label, Func<string> tooltip, string value, string fallbackValue, TextStyle textStyle, Action onClick) : base(label, tooltip, textStyle)
    {
      _value = value;
      _fallbackValue = fallbackValue;
      _onClick = onClick;
    }

    private HudValue(IValueModel model, TextStyle textStyle) : this(model.Label, model.Tooltip, model.Value, null, textStyle, model.OnClick)
    { }

    private HudValue(TextModel model, TextStyle textStyle, Action onClick) : this(null, model.Tooltip, model.Text, null, textStyle, onClick)
    { }

    public static HudValue FromValueModel(IValueModel model, TextStyle textStyle) => model == null || model.Hidden ? null : new HudValue(model, textStyle);
    public static HudValue FromTextModel(TextModel model, TextStyle textStyle) => model == null ? null : new HudValue(model, textStyle, model?.OnClick);

    protected override bool DoDraw(Rect rect)
    {
      if (_value.NullOrEmpty() && _fallbackValue == null) { return false; }

      var showLabel = Label != null;

      var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

      if (showLabel) { DrawText(grid[1], Label); }
      DrawText(grid[2], _value, alignment: showLabel ? TextAnchor.MiddleRight : (TextAnchor?) null);
      if (!Hud.IsMouseOverConfigButton && Widgets.ButtonInvisible(rect.ExpandedBy(GUIPlus.TinyPadding))) { _onClick?.Invoke(); }

      if (!Hud.IsMouseOverConfigButton) { GUIPlus.DrawTooltip(grid[0], Tooltip, false); }
      return true;
    }
  }
}
