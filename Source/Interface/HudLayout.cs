using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class HudLayout : Layout
    {
        private const float BarLabelWidth = 50f;
        private const float SkillLabelWidth = 80f;
        private const float SkillValueWidth = 40f;
        public HudLayout() : base(Theme.RegularTextStyle)
        { }

        public void DrawStyledLabelRow(string text, Color? color = null, TextStyle style = null)
        {
            Next(style);
            DrawLabel(text, color, style);
            PadDown();
        }
        public void DrawLabelRow(string text, Color? color = null) => DrawStyledLabelRow(text, color, Theme.RegularTextStyle);
        public void DrawTitleRow(string text, Color? color = null) => DrawStyledLabelRow(text, color, Theme.LargeTextStyle);
        public void DrawDescriptionRow(string text, Color? color = null) => DrawStyledLabelRow(text, color, Theme.SmallTextStyle);

        public bool DrawLabelledBar(string label, float barPercent)
        {
            if (barPercent < 0f) { return false; }

            Next(BarLabelWidth);
            DrawLabel(label);

            PadRight();
            Next(-(SkillValueWidth + DefaultPadding));
            DrawBar(barPercent);

            PadRight();
            Next();
            DrawLabel(barPercent.ToPercentageString());
            PadDown();

            return true;
        }

        public bool DrawMoodBar(string label, float barPercent, float thresholdMinor, float thresholdMajor, float thresholdExtreme)
        {
            if (barPercent < 0f) { return false; }

            Next(BarLabelWidth);
            DrawLabel(label);

            PadRight();
            Next(-(SkillValueWidth + DefaultPadding));
            DrawBar(barPercent);
            DrawThreshold(thresholdMinor, Theme.MentalThresholdMinor);
            DrawThreshold(thresholdMajor, Theme.MentalThresholdMajor);
            DrawThreshold(thresholdExtreme, Theme.MentalThresholdExtreme);

            PadRight();
            Next();
            DrawLabel(barPercent.ToPercentageString());
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

        private void DrawLabelledSkill(SkillModel skill)
        {
            Next(SkillLabelWidth);
            DrawLabel(skill.Label, skill.Color);

            Next(SkillValueWidth);
            DrawLabel(skill.Level, skill.Color);
        }

        public bool DrawSkillPair(SkillModel left, SkillModel right)
        {
            if (!left.Disabled)
            {
                DrawLabelledSkill(left);
                PadRight(Mathf.Max(RemainingWidth - (SkillLabelWidth + SkillValueWidth), 0f));
            }

            if (!right.Disabled) { DrawLabelledSkill(right); }
            if (left.Disabled && right.Disabled) { return false; }

            PadDown();
            return true;
        }
    }
}
