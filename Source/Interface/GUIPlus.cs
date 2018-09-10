using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class GUIPlus
    {
        public static void DrawBackground(Rect rect)
        {
            var previousColor = GUI.color;
            GUI.color = Theme.BackgroundColor;
            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            GUI.color = previousColor;
        }

        public static void DrawBar(Rect rect, float percentage, Color color)
        {
            Widgets.DrawBoxSolid(rect, Theme.BarBackgroundColor);
            Widgets.DrawBoxSolid(rect.LeftPart(percentage), color);
        }

        public static GUIStyle GetGUIStyle(TextAnchor alignment, int fontSize, bool wrap) => new GUIStyle(Theme.BaseFontStyle) { alignment = alignment, wordWrap = wrap, fontSize = fontSize };
    }
}
