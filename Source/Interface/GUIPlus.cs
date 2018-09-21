using System.Collections;
using System.Linq;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimHUD.Interface
{
    internal static class GUIPlus
    {
        public const float ScrollbarWidth = 16f;
        public const float CheckboxSize = 24f;
        public const float MediumPadding = 8f;
        public const float LargePadding = 16f;

        private static readonly Color DisabledColor = new Color(0.5f, 0.5f, 0.5f);

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

        public static void SetEnabledColor(bool enabled) => SetColor(enabled ? (Color?) null : DisabledColor);

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

        public static void DrawTexture(Rect rect, Color? color = null, int borderThickness = 0, Color? borderColor = null)
        {
            SetColor(color);
            GUI.DrawTexture(rect.ContractedBy(borderThickness), BaseContent.WhiteTex);
            ResetColor();

            if (borderThickness <= 0) { return; }

            SetColor(borderColor);
            Widgets.DrawBox(rect);
            ResetColor();
        }

        public static bool DrawButton(Rect rect, string label, string tooltip = null, bool enabled = true)
        {
            SetEnabledColor(enabled);
            var result = Widgets.ButtonText(rect, label, active: enabled);
            DrawTooltip(rect, tooltip, true);
            ResetColor();

            return result & enabled;
        }

        public static int DrawButtonRow(Rect rect, float width, float padding, params string[] labels)
        {
            var grid = rect.GetHGrid(padding, Enumerable.Repeat(width, labels.Length).ToArray());

            for (var index = 0; index < labels.Length; index++)
            {
                if (DrawButton(grid[index + 1], labels[index])) { return index + 1; }
            }

            return 0;
        }
        public static bool DrawCheckbox(Rect rect, string label, bool value, string tooltip = null, bool enabled = true, float checkSize = CheckboxSize)
        {
            SetEnabledColor(enabled);

            var labelRect = new Rect(rect.x, rect.y, rect.width - CheckboxSize - 4f, rect.height);

            Widgets.Label(labelRect, label);

            var result = value;
            if (enabled && Widgets.ButtonInvisible(rect))
            {
                result = !value;
                if (result) { SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(); }
                else { SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(); }
            }

            DrawTooltip(rect, tooltip, false);

            var image = result ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex;
            var position = new Rect((rect.x + rect.width) - CheckboxSize, rect.y, checkSize, checkSize);

            GUI.DrawTexture(position, image);
            ResetColor();

            return result;
        }

        public static void DrawBar(Rect rect, float percentage, Color color)
        {
            Widgets.DrawBoxSolid(rect, Theme.BarBackgroundColor);
            Widgets.DrawBoxSolid(rect.LeftPart(percentage), color);
        }

        public static void DrawTooltip(Rect rect, string tooltip, bool highlight)
        {
            if (tooltip.NullOrEmpty() || !Mouse.IsOver(rect)) { return; }

            if (highlight) { Widgets.DrawHighlight(rect); }
            TooltipHandler.TipRegion(rect, tooltip);
        }
    }
}
