using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudFeature : HudWidget
    {
        protected string Label { get; }
        protected TipSignal? Tooltip { get; }
        private readonly TextStyle _textStyle;

        public override float Height { get; }

        protected HudFeature(string label, TipSignal? tooltip, TextStyle textStyle)
        {
            Label = label;
            Tooltip = tooltip;
            _textStyle = textStyle;
            Height = _textStyle.LineHeight;
        }

        protected void DrawText(Rect rect, string text) => GUIPlus.DrawText(rect, text, style: _textStyle);
    }
}
