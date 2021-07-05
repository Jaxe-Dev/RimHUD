using System;
using System.Linq;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudBar : HudFeature
    {
        private readonly float _max;
        private readonly float _value;
        private readonly ValueStyle _valueStyle;
        private readonly float[] _thresholds;
        private readonly Action _onClick;

        public HudBar(string label, float value, float max, ValueStyle valueStyle, TextStyle textStyle, TipSignal? tooltip = null, float[] thresholds = null, Action onClick = null) : base(label, tooltip, textStyle)
        {
            _thresholds = thresholds;
            _max = max;
            _value = value;
            _valueStyle = valueStyle;
            _onClick = onClick;
        }

        private HudBar(IBarModel model, TextStyle textStyle) : this(model.Label, model.Value, model.Max, model.ValueStyle, textStyle, model.Tooltip, model.Thresholds, model.OnClick) { }

        public static HudBar FromModel(IBarModel model, TextStyle textStyle) => model == null || model.Hidden ? null : new HudBar(model, textStyle);

        public override bool Draw(Rect rect)
        {
            if (_value < 0f) { return false; }

            var percentage = _value / _max;

            var showLabel = Label != null;

            var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f, _valueStyle == ValueStyle.Hidden ? 0f : Theme.ValueWidth.Value);

            DrawText(grid[1], Label);
            GUIPlus.DrawBar(grid[2], percentage, GetBarColor(percentage));
            DrawThresholds(grid[2]);
            DrawValue(grid[3], _value, _max);

            if (Hud.IsMouseOverConfigButton) { return true; }

            if (Widgets.ButtonInvisible(rect.ExpandedBy(GUIPlus.TinyPadding))) { _onClick?.Invoke(); }
            if (Mouse.IsOver(rect)) { GUIPlus.DrawTooltip(grid[0], Tooltip, false); }

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
            foreach (var threshold in _thresholds.Where(threshold => threshold > 0f)) { Widgets.DrawLineVertical(Mathf.Round(rect.x + (rect.width * threshold)), rect.y, rect.height); }
            GUIPlus.ResetColor();
        }

        private static Color GetBarColor(float percentage) => Color.Lerp(Theme.BarLowColor.Value, Theme.BarMainColor.Value, percentage);

        public enum ValueStyle
        {
            Hidden,
            Percentage,
            ValueMax,
            ValueOnly
        }
    }
}
