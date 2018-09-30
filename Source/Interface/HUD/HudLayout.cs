using System;
using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudLayout : Layout
    {
        public HudLayout() : base(Theme.RegularTextStyle, Theme.MainTextColor.Value)
        { }

        public void DrawStyledLabelRow(string text, Color? color = null, TextStyle style = null, Func<string> tooltip = null)
        {
            Next(style);
            DrawLabel(text, color, style ?? Theme.RegularTextStyle);
            GUIPlus.DrawTooltip(CurrentRect, tooltip, false);
            PadDown();
        }
        public void DrawLabelRow(string text, Color? color = null, Func<string> tooltip = null) => DrawStyledLabelRow(text, color, Theme.RegularTextStyle, tooltip);
        public void DrawTitleRow(string text, Color? color = null, Func<string> tooltip = null) => DrawStyledLabelRow(text, color, Theme.LargeTextStyle, tooltip);
        public void DrawDescriptionRow(string text, Color? color = null, Func<string> tooltip = null, bool textToTooltip = false) => DrawStyledLabelRow(text, color, Theme.SmallTextStyle, textToTooltip ? () => text.Size(Theme.SmallTextStyle.ActualSize) : tooltip);

        public bool DrawLabelledBar(string label, float barPercent, float thresholdMinor = -1f, float thresholdMajor = -1f, float thresholdExtreme = -1f, Func<string> tooltip = null)
        {
            if (barPercent < 0f) { return false; }

            Next(Theme.LabelWidth.Value);

            var firstRect = CurrentRect;

            DrawLabel(label);

            PadRight();
            Next(-(Theme.ValueWidth.Value + DefaultPadding));
            DrawBar(barPercent);
            if (thresholdMinor >= 0) { DrawThreshold(thresholdMinor, Theme.BarThresholdMinorColor.Value); }
            if (thresholdMinor >= 0) { DrawThreshold(thresholdMajor, Theme.BarThresholdMajorColor.Value); }
            if (thresholdMinor >= 0) { DrawThreshold(thresholdExtreme, Theme.BarThresholdExtremeColor.Value); }

            PadRight();
            Next();
            DrawLabel(barPercent.ToPercentageString());
            var tooltipRect = new Rect(firstRect.x, firstRect.y, CurrentRect.xMax - firstRect.x, CurrentRect.yMax - firstRect.y);
            GUIPlus.DrawTooltip(tooltipRect, tooltip, false);
            PadDown();

            return true;
        }

        private void DrawThreshold(float percent, Color color)
        {
            var previousColor = GUI.color;
            GUI.color = color;
            Widgets.DrawLineVertical(Mathf.Round(CurrentRect.x + (CurrentRect.width * percent)), CurrentRect.y, CurrentRect.height);
            GUI.color = previousColor;
        }

        private void DrawLabelledStat(StatModel stat)
        {
            Next(Theme.LabelWidth.Value);
            var firstRect = CurrentRect;
            DrawLabel(stat.Label, stat.Color);

            Next(Theme.ValueWidth.Value);
            DrawLabel(stat.Level, stat.Color);

            var tooltipRect = new Rect(firstRect.x, firstRect.y, CurrentRect.xMax - firstRect.x, CurrentRect.yMax - firstRect.y);
            GUIPlus.DrawTooltip(tooltipRect, stat.Tooltip, false);
        }

        public bool DrawStatPair(StatModel left, StatModel right) => DrawStatPairing(left, right, false);
        public bool DrawStatPairFilled(StatModel left, StatModel right) => DrawStatPairing(left, right, true);

        private bool DrawStatPairing(StatModel left, StatModel right, bool fill)
        {
            if (!left?.Hidden ?? false)
            {
                DrawLabelledStat(left);
                PadRight(fill ? Mathf.Max(RemainingWidth - (Theme.LabelWidth.Value + Theme.ValueWidth.Value), 0f) : DefaultPadding);
            }

            if (!right?.Hidden ?? false) { DrawLabelledStat(right); }
            if ((left?.Hidden ?? false) && (right?.Hidden ?? false)) { return false; }

            PadDown();
            return true;
        }
    }
}
