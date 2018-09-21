using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class HudLayout : Layout
    {
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

            Next(Theme.LabelWidth.Value);
            DrawLabel(label);

            PadRight();
            Next(-(Theme.ValueWidth.Value + DefaultPadding));
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

            Next(Theme.LabelWidth.Value);
            DrawLabel(label);

            PadRight();
            Next(-(Theme.ValueWidth.Value + DefaultPadding));
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
            Next(Theme.LabelWidth.Value);
            DrawLabel(skill.Label, skill.Color);

            Next(Theme.ValueWidth.Value);
            DrawLabel(skill.Level, skill.Color);
        }

        public bool DrawSkillPair(SkillModel left, SkillModel right) => DrawSkillPairing(left, right, false);
        public bool DrawSkillPairFilled(SkillModel left, SkillModel right) => DrawSkillPairing(left, right, true);

        private bool DrawSkillPairing(SkillModel left, SkillModel right, bool fill)
        {
            if (!left.Disabled)
            {
                DrawLabelledSkill(left);
                PadRight(fill ? Mathf.Max(RemainingWidth - (Theme.LabelWidth.Value + Theme.ValueWidth.Value), 0f) : DefaultPadding);
            }

            if (!right.Disabled) { DrawLabelledSkill(right); }
            if (left.Disabled && right.Disabled) { return false; }

            PadDown();
            return true;
        }
    }
}
