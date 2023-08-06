using System;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets
{
  public class BarWidget : StandardWidget
  {
    private const ValueStyle DefaultValueStyle = ValueStyle.Percentage;
    private const ColorStyle DefaultColorStyle = ColorStyle.LowToMain;

    private readonly float _max;
    private readonly float _value;

    private readonly ValueStyle _valueStyle;
    private readonly ColorStyle _colorStyle;

    private readonly float[] _thresholds;
    private readonly Action _onClick;

    private BarWidget(string label, float value, float max, TextStyle textStyle, Func<string> tooltip, float[] thresholds, ValueStyle valueStyle, ColorStyle colorStyle, Action onClick) : base(label, tooltip, textStyle)
    {
      _thresholds = thresholds;
      _max = max;
      _value = value;
      _valueStyle = valueStyle;
      _colorStyle = colorStyle;
      _onClick = onClick;
    }

    public static BarWidget FromModel(IModelBar model, TextStyle textStyle, string variant = null) => model?.Hidden ?? true ? null : new BarWidget(model.Label, model.Value, model.Max, textStyle, model.Tooltip, model.Thresholds, DefaultValueStyle, GetColorStyleFromVariant(variant), model.OnClick);

    private static ColorStyle GetColorStyleFromVariant(string variant)
    {
      switch (variant)
      {
        case nameof(ColorStyle.LowOnly):
          return ColorStyle.LowOnly;
        case nameof(ColorStyle.MainOnly):
          return ColorStyle.MainOnly;
        case nameof(ColorStyle.MainToLow):
          return ColorStyle.MainToLow;
        default:
          return DefaultColorStyle;
      }
    }

    public override bool Draw(Rect rect)
    {
      if (_value < 0f) { return false; }

      var percentage = _value / _max;

      var showLabel = Label != null;

      var grid = rect.GetHGrid(WidgetsPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f, _valueStyle == ValueStyle.Hidden ? 0f : Theme.ValueWidth.Value);

      DrawText(grid[1], Label);
      WidgetsPlus.DrawBar(grid[2], percentage, GetBarColor(percentage));
      DrawThresholds(grid[2]);
      DrawValue(grid[3], _value, _max);

      if (HudLayout.IsMouseOverConfigButton) { return true; }

      if (Verse.Widgets.ButtonInvisible(rect.ExpandedBy(WidgetsPlus.TinyPadding))) { _onClick?.Invoke(); }
      WidgetsPlus.DrawTooltip(rect, Tooltip, true);

      return true;
    }

    private void DrawValue(Rect rect, float value, float max)
    {
      if (_valueStyle == ValueStyle.Hidden) { return; }
      if (_valueStyle == ValueStyle.Percentage) { DrawText(rect, value.ToStringPercent()); }
      else if (_valueStyle == ValueStyle.ValueMax) { DrawText(rect, $"{value}/{max}"); }
      else if (_valueStyle == ValueStyle.ValueOnly) { DrawText(rect, $"{value}"); }
      else { throw new Mod.Exception($"Invalid {nameof(ValueStyle)}"); }
    }

    private void DrawThresholds(Rect rect)
    {
      if (_thresholds == null) { return; }

      GUIPlus.SetColor(Theme.BarThresholdColor.Value);
      foreach (var threshold in _thresholds.Where(threshold => threshold > 0f)) { Verse.Widgets.DrawLineVertical(Mathf.Round(rect.x + (rect.width * threshold)), rect.y, rect.height); }
      GUIPlus.ResetColor();
    }

    private Color GetBarColor(float percentage)
    {
      switch (_colorStyle)
      {
        case ColorStyle.LowOnly:
          return Theme.BarLowColor.Value;
        case ColorStyle.MainToLow:
          return Color.Lerp(Theme.BarMainColor.Value, Theme.BarLowColor.Value, percentage);
        case ColorStyle.MainOnly:
          return Theme.BarMainColor.Value;
        default:
          return Color.Lerp(Theme.BarLowColor.Value, Theme.BarMainColor.Value, percentage);
      }
    }

    public static string GetColorStyleLabel(ColorStyle colorStyle) => Lang.Get($"Layout.Variant.{colorStyle}");

    public enum ValueStyle
    {
      Hidden,
      Percentage,
      ValueMax,
      ValueOnly
    }

    public enum ColorStyle
    {
      LowToMain,
      LowOnly,
      MainToLow,
      MainOnly
    }
  }
}
