using System;
using UnityEngine;

namespace RimHUD.Data.Models
{
    internal class TextModel
    {
        public string Text { get; }
        public Func<string> Tooltip { get; }
        public Color? Color { get; }
        public Action OnClick { get; }

        private TextModel(string text, Func<string> tooltip, Color? color, Action onClick)
        {
            Text = text;
            Tooltip = tooltip;
            Color = color;
            OnClick = onClick;
        }

        public static TextModel Create(string text, Func<string> tooltip, Color? color, Action onClick = null) => text == null ? null : new TextModel(text, tooltip, color, onClick);
    }
}
