using UnityEngine;

namespace RimHUD.Data
{
    internal class StringPlus
    {
        public string Text { get; }
        public Color Color { get; }

        private StringPlus(string text, Color color)
        {
            Color = color;
            Text = text;
        }

        public static StringPlus Create(string text, Color color) => text == null ? null : new StringPlus(text, color);
    }
}
