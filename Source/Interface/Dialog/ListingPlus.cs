using System.Text.RegularExpressions;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using UnityEngine;
using Verse;
using ColorOption = RimHUD.Data.Configuration.ColorOption;

namespace RimHUD.Interface.Dialog
{
    internal class ListingPlus : Listing_Standard
    {
        private const float LabelWidth = 150f;
        private const float ValueWidth = 100f;
        private const float ElementPadding = 1f;

        private static readonly Color LinkColor = new Color(0.3f, 0.7f, 1f);
        private static readonly Regex RangeSliderEntryRegex = new Regex(@"^[-]?\d{0,4}$");

        public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect)
        {
            if (viewRect == default) { viewRect = new Rect(rect.x, rect.y, rect.width - GUIPlus.ScrollbarWidth, 99999f); }

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);

            Begin(viewRect);
        }

        public void EndScrollView(ref Rect viewRect)
        {
            End();
            Widgets.EndScrollView();
            viewRect.height = CurHeight;
        }

        public bool Label(string label, TipSignal? tooltip = null, GameFont? font = null, Color? color = null, bool highlight = false)
        {
            GUIPlus.SetFont(font);
            GUIPlus.SetColor(color);

            var rect = GetRect(Text.CalcHeight(label, ColumnWidth));
            Widgets.Label(rect, label);
            GUIPlus.DrawTooltip(rect, tooltip, highlight);
            Gap(verticalSpacing);

            GUIPlus.ResetColor();
            GUIPlus.ResetFont();

            return Widgets.ButtonInvisible(rect);
        }

        public void LinkLabel(string label, string url, TipSignal? tooltip = null)
        {
            if (Label(label, tooltip, GameFont.Tiny, LinkColor, true)) { Application.OpenURL(url); }
        }

        public void ColorOptionSelect(ColorOption colorOption, ref ColorOption selected, bool enabled = true)
        {
            GUIPlus.SetColor(colorOption.Value);
            if (RadioButton(colorOption.Label, selected == colorOption, tooltip: colorOption.Tooltip)) { selected = colorOption; }
            GUIPlus.ResetColor();
        }

        public void TextStyleEditor(TextStyle style, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            Label(style.Label.Bold());
            RangeSlider(style.Size, enabled);
            RangeSlider(style.Height, enabled);

            GUIPlus.ResetColor();
        }

        public void BoolToggle(BoolOption option, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);
            option.Value = CheckboxLabeled(option.Label, option.Value, option.Tooltip, enabled);
            GUIPlus.ResetColor();
        }

        public void RangeSlider(RangeOption range, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

            GUIPlus.DrawText(grid[1], range.Label);
            GUIPlus.DrawText(grid[2], range.ToString());

            var value = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
            if (enabled) { range.Value = value; }

            GUIPlus.DrawTooltip(grid[0], range.Tooltip, true);
            Gap(verticalSpacing);

            GUIPlus.ResetColor();
        }

        public void RangeSliderEntry(RangeOption range, ref string text, int id, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

            GUIPlus.DrawText(grid[1], range.Label);

            var entryName = "RangeSliderEntry_Text" + id;
            var isFocused = GUI.GetNameOfFocusedControl() == entryName;
            if (!isFocused) { text = range.Value.ToString(); }

            GUI.SetNextControlName(entryName);
            var newText = Widgets.TextField(grid[2], text, 5, RangeSliderEntryRegex);
            if (enabled) { text = newText; }

            var textValue = text.ToInt();

            if (textValue.HasValue)
            {
                if (textValue.Value < range.Min) { range.Value = range.Min; }
                else if (textValue.Value > range.Max) { range.Value = range.Max; }
                else { range.Value = textValue.Value; }
            }

            var sliderName = "RangeSliderEntry_Slider" + id;
            GUI.SetNextControlName(sliderName);
            var sliderValue = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
            if (enabled && range.Value != sliderValue)
            {
                range.Value = sliderValue;
                text = range.Value.ToString();
            }
            if (Widgets.ButtonInvisible(grid[3])) { GUI.FocusControl(sliderName); }

            GUIPlus.DrawTooltip(grid[0], range.Tooltip, true);
            Gap(verticalSpacing);

            GUIPlus.ResetColor();
        }

        public bool CheckboxLabeled(string label, bool value, string tooltip = null, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);
            var previous = value;
            CheckboxLabeled(label, ref value, tooltip);
            GUIPlus.ResetColor();

            return enabled ? value : previous;
        }

        public bool ButtonText(string label, string tooltip = null, GameFont? font = null, bool enabled = true)
        {
            var result = GUIPlus.DrawButton(GetRect(GUIPlus.ButtonHeight), label, tooltip, font, enabled);
            Gap(verticalSpacing);
            return result;
        }

        public Rect GetRemaining()
        {
            var height = listingRect.height - curY;

            NewColumnIfNeeded(height);
            var result = new Rect(curX, curY, ColumnWidth, height);
            curY += height;
            return result;
        }

        public Rect[] GetButtonGrid(params float[] widths)
        {
            var rect = GetRect(GUIPlus.ButtonHeight);
            Gap(verticalSpacing);

            return rect.GetHGrid(GUIPlus.SmallPadding, widths);
        }
    }
}
