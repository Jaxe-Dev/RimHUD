using RimHUD.Data.Models;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudValue : HudFeature
    {
        private readonly string _value;
        private readonly string _fallbackValue;
        private readonly Color? _color;

        public HudValue(string label, TipSignal? tooltip, string value, string fallbackValue, Color? color, TextStyle textStyle) : base(label, tooltip, textStyle)
        {
            _value = value;
            _fallbackValue = fallbackValue;
            _color = color;
        }

        private HudValue(ValueModel model, TextStyle textStyle) : this(model.Label, model.Tooltip, model.Value, null, model.Color, textStyle)
        { }

        private HudValue(TextModel model, TextStyle textStyle) : this(null, model.Tooltip, model.Text, null, model.Color, textStyle)
        { }

        public static HudValue FromValueModel(ValueModel model, TextStyle textStyle) => (model == null) || model.Hidden ? null : new HudValue(model, textStyle);
        public static HudValue FromTextModel(TextModel model, TextStyle textStyle) => model == null ? null : new HudValue(model, textStyle);
        public static HudValue FromText(string text, TipSignal? tooltip, TextStyle textStyle) => new HudValue(null, tooltip, text, null, null, textStyle);

        public override bool Draw(Rect rect)
        {
            if (_value.NullOrEmpty() && (_fallbackValue == null)) { return false; }

            var showLabel = Label != null;

            var grid = rect.GetHGrid(GUIPlus.TinyPadding, showLabel ? Theme.LabelWidth.Value : 0f, -1f);

            GUIPlus.SetColor(_color);
            DrawText(grid[1], Label);
            DrawText(grid[2], _value);
            GUIPlus.ResetColor();

            GUIPlus.DrawTooltip(grid[0], Tooltip, false);
            return true;
        }
    }
}
