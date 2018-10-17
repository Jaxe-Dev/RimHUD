using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class TextModel
    {
        public string Text { get; }
        public TipSignal? Tooltip { get; }
        public Color? Color { get; }

        private TextModel(string text, TipSignal? tooltip, Color? color)
        {
            Text = text;
            Tooltip = tooltip;
            Color = color;
        }

        public static TextModel Create(string text, TipSignal? tooltip, Color? color) => text == null ? null : new TextModel(text, tooltip, color);
    }
}
