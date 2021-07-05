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
        private readonly Color? _color;
        private readonly Action _onClick;

        private HudValue(string label, TipSignal? tooltip, string value, string fallbackValue, Color? color, TextStyle textStyle, Action onClick) : base(label, tooltip, textStyle)
        {
            _value = value;
            _fallbackValue = fallbackValue;
            _color = color;
            _onClick = onClick;
        }

        private HudValue(IValueModel model, TextStyle textStyle) : this(model.Label, model.Tooltip, model.Value, null, model.Color, textStyle, model.OnClick) { }

        private HudValue(TextModel model, TextStyle textStyle, Action onClick) : this(null, model.Tooltip, model.Text, null, model.Color, textStyle, onClick) { }

        public static HudValue FromValueModel(IValueModel model, TextStyle textStyle) => model == null || model.Hidden ? null : new HudValue(model, textStyle);
        public static HudValue FromTextModel(TextModel? model, TextStyle textStyle) => model == null ? null : new HudValue(model.Value, textStyle, model.Value.OnClick);
        public static HudValue FromText(string text, TipSignal? tooltip, TextStyle textStyle, Action onClick = null) => new HudValue(null, tooltip, text, null, null, textStyle, onClick);

        public override bool Draw(Rect rect)
        {
            if (_value.NullOrEmpty() && _fallbackValue == null) { return false; }

            var showLabel = Label != null;

            var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

            GUIPlus.SetColor(_color);
            if (showLabel) { DrawText(grid[1], Label); }
            DrawText(grid[2], _value, alignment: showLabel ? TextAnchor.MiddleRight : (TextAnchor?) null);
            if (!Hud.IsMouseOverConfigButton && Widgets.ButtonInvisible(rect.ExpandedBy(GUIPlus.TinyPadding))) { _onClick?.Invoke(); }
            GUIPlus.ResetColor();

            if (!Hud.IsMouseOverConfigButton) { GUIPlus.DrawTooltip(grid[0], Tooltip, false); }
            return true;
        }
    }
}
