using System;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudFeature : HudWidget
    {
        protected string Label { get; }
        protected Func<string> Tooltip { get; }
        private readonly TextStyle _textStyle;

        public override float Height { get; }

        protected HudFeature(string label, Func<string> tooltip, TextStyle textStyle)
        {
            Label = label;
            Tooltip = tooltip;
            _textStyle = textStyle;
            Height = _textStyle.LineHeight;
        }

        protected void DrawText(Rect rect, string text, Color? color = null, TextAnchor? alignment = null) => GUIPlus.DrawText(rect, text, color, _textStyle, alignment);
    }
}
