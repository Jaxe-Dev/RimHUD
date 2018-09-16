using System.Collections;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class GUIPlus
    {
        private static readonly Stack SavedColors = new Stack();
        private static readonly Stack SavedFonts = new Stack();

        public static void SetColor(Color? color)
        {
            if (color == null)
            {
                SavedColors.Push(null);
                return;
            }

            SavedColors.Push((Color?) GUI.color);

            GUI.color = color.Value;
        }
        public static void ResetColor()
        {
            var color = (Color?) SavedColors.Pop();
            if (color == null) { return; }

            GUI.color = color.Value;
        }

        public static void SetEnabledColor(bool enabled) => SetColor(enabled ? (Color?) null : Theme.DisabledColor);

        public static void SetFont(GameFont? font)
        {
            if (font == null)
            {
                SavedFonts.Push(null);
                return;
            }

            SavedFonts.Push((GameFont?) Text.Font);

            Text.Font = font.Value;
        }

        public static void ResetFont()
        {
            var font = (GameFont?) SavedFonts.Pop();
            if (font == null) { return; }

            Text.Font = font.Value;
        }

        public static void DrawLabel(Rect rect, string text, Color? color = null, TextStyle style = null)
        {
            SetColor(color);
            GUI.Label(rect, text, style?.GUIStyle ?? Text.CurFontStyle);
            ResetColor();
        }

        public static void DrawBackground(Rect rect)
        {
            SetColor(Theme.BackgroundColor);
            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            ResetColor();
        }

        public static void DrawBar(Rect rect, float percentage, Color color)
        {
            Widgets.DrawBoxSolid(rect, Theme.BarBackgroundColor);
            Widgets.DrawBoxSolid(rect.LeftPart(percentage), color);
        }

        public static void DrawTooltip(Rect rect, string tooltip)
        {
            if (tooltip.NullOrEmpty()) { return; }

            if (Mouse.IsOver(rect)) { Widgets.DrawHighlight(rect); }
            TooltipHandler.TipRegion(rect, tooltip);
        }
    }
}
