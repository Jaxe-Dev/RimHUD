using System.Text.RegularExpressions;
using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class ListingPlus : Listing_Standard
    {
        private const float LabelWidth = 150f;
        private const float ValueWidth = 100f;
        private const float ElementPadding = 1f;

        private static readonly Regex RangeSliderEntryRegex = new Regex(@"^[-]?\d{0,4}$");

        public void Label(string label, string tooltip = null, GameFont? font = null, Color? color = null)
        {
            GUIPlus.SetFont(font);
            GUIPlus.SetColor(color);

            base.Label(label, tooltip: tooltip);

            GUIPlus.ResetColor();
            GUIPlus.ResetFont();
        }

        public void TextStyleEditor(TextStyle style, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            Label(style.Label.Bold());
            RangeSlider(style.Size, enabled);
            RangeSlider(style.Height, enabled);

            GUIPlus.ResetColor();
        }

        public void RangeSlider(RangeOption range, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

            GUIPlus.DrawLabel(grid[1], range.Label);
            GUIPlus.DrawLabel(grid[2], range.ToString());

            var value = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
            if (enabled) { range.Value = value; }

            GUIPlus.DrawTooltip(grid[0], range.Tooltip);
            Gap(verticalSpacing);

            GUIPlus.ResetColor();
        }

        public void RangeSliderEntry(RangeOption range, ref string text, int id, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);

            var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

            GUIPlus.DrawLabel(grid[1], range.Label);

            var entryName = "RangeSliderEntry_Text" + id;
            var isFocused = GUI.GetNameOfFocusedControl() == entryName;
            if (!isFocused) { text = range.Value.ToString(); }

            GUI.SetNextControlName(entryName);
            var newText = Widgets.TextField(grid[2], text, 5, RangeSliderEntryRegex);
            if (enabled) { text = newText; }

            var textValue = text.ToInt();

            if (textValue.HasValue)
            {
                if (textValue.Value < range.Min)
                {
                    range.Value = range.Min;
                }
                else if (textValue.Value > range.Max)
                {
                    range.Value = range.Max;
                }
                else { range.Value = textValue.Value; }
            }

            var sliderName = "RangeSliderEntry_Slider" + id;
            GUI.SetNextControlName(sliderName);
            var sliderValue = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
            if (enabled && (range.Value != sliderValue))
            {
                range.Value = sliderValue;
                text = range.Value.ToString();
            }
            if (Widgets.ButtonInvisible(grid[3])) { GUI.FocusControl(sliderName); }

            GUIPlus.DrawTooltip(grid[0], range.Tooltip);
            Gap(verticalSpacing);

            GUIPlus.ResetColor();
        }

        public bool CheckboxLabeled(string label, bool value, string tooltip = null, bool enabled = true)
        {
            GUIPlus.SetEnabledColor(enabled);
            CheckboxLabeled(label, ref value, tooltip);
            GUIPlus.ResetColor();

            return value;
        }
    }
}
