using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class Hud
    {
        private const float Width = 250f;
        private const float Height = 350f;
        private const float Padding = 4f;

        private const float TitleHeight = 17f;
        private const float DescriptionHeight = 11f;
        private const float BarLabelWidth = 50f;
        private const float SkillLabelWidth = 80f;
        private const float SkillValueWidth = 40f;

        public static Vector2 Offset { get; set; } = new Vector2(-8f, 8f);
        public static Rect Bounds => new Rect((UI.screenWidth - Width) + Offset.x, Offset.y, Width, Height);
        private static Rect Inner => new Rect(Bounds.ContractedBy(Padding));

        public static bool Activated { get; set; } = true;
        public static bool Visible => Activated && IsReady && (PawnModel.Selected != null);
        public static bool IsReady => Activated && (Current.Game != null) && (Current.Game.CurrentMap != null);

        public static void Draw()
        {
            if (!IsReady) { return; }

            var pawn = PawnModel.Selected;
            if (pawn == null) { return; }

            GUIPlus.DrawBackground(Bounds);

            var l = new HudListing();

            l.Bind(Inner);

            var relationColor = pawn.FactionRelationColor;

            l.DrawLabelRow(pawn.Name.Bold(), 13, color: relationColor, height: TitleHeight);
            l.GapLine();

            l.DrawDescriptionRow(pawn.RelationKindAndFaction, color: relationColor);
            l.DrawDescriptionRow(pawn.GenderAndAge);
            l.GapLine();

            var usesHealth = l.DrawLabelledBar(Lang.Get("Bar.Health"), pawn.Health);

            var healthCondition = pawn.HealthCondition;
            if (healthCondition != null) { l.DrawLabelRow(healthCondition?.Text ?? null, color: healthCondition?.Color); }
            if (usesHealth || (healthCondition != null)) { l.GapLine(); }

            var usesMood = l.DrawMoodBar(Lang.Get("Bar.Mood"), pawn.Mood, pawn.MoodThresholdMinor, pawn.MoodThresholdMajor, pawn.MoodThresholdExtreme);
            var mentalCondition = pawn.MentalCondition;
            if (mentalCondition != null) { l.DrawLabelRow(mentalCondition?.Text ?? null, color: mentalCondition?.Color); }
            if (usesMood || (mentalCondition != null)) { l.GapLine(); }

            var usesOtherNeeds = l.DrawLabelledBar(Lang.Get("Bar.Rest"), pawn.Rest);
            usesOtherNeeds |= l.DrawLabelledBar(Lang.Get("Bar.Food"), pawn.Food);
            usesOtherNeeds |= l.DrawLabelledBar(Lang.Get("Bar.Recreation"), pawn.Recreation);
            if (usesOtherNeeds) { l.GapLine(); }

            var usesSkills = l.DrawSkillPair(pawn.Shooting, pawn.Melee);
            usesSkills |= l.DrawSkillPair(pawn.Construction, pawn.Mining);
            usesSkills |= l.DrawSkillPair(pawn.Cooking, pawn.Medicine);
            usesSkills |= l.DrawSkillPair(pawn.Plants, pawn.Animals);
            usesSkills |= l.DrawSkillPair(pawn.Crafting, pawn.Artistic);
            usesSkills |= l.DrawSkillPair(pawn.Social, pawn.Intellectual);
            if (usesSkills) { l.GapLine(); }

            var equipped = pawn.Equipped;
            if (equipped != null) { l.DrawLabelRow(pawn.Equipped); }
            var activity = pawn.Activity;
            if (activity != null) { l.DrawDescriptionRow(pawn.Activity); }
        }

        private static void DrawLabelRow(this HudListing self, string text, int fontSize = Theme.BaseFontSize, TextAnchor alignment = HudListing.DefaultTextAnchor, Color? color = null, float height = HudListing.DefaultNullValue, float padding = HudListing.DefaultPadding)
        {
            self.Next(height: height);
            self.DrawLabel(text, fontSize, alignment, color);
            self.Gap(padding);
        }

        private static void DrawDescriptionRow(this HudListing self, string text, int fontSize = Theme.BaseFontSize, TextAnchor alignment = HudListing.DefaultTextAnchor, Color? color = null, float padding = HudListing.DefaultPadding) => self.DrawLabelRow(text, fontSize, alignment, color, DescriptionHeight, padding);

        private static bool DrawLabelledBar(this HudListing self, string label, float barPercent)
        {
            if (barPercent < 0f) { return false; }

            self.Next(BarLabelWidth);
            self.DrawLabel(label);

            self.Pad();
            self.Next(-(SkillValueWidth + HudListing.DefaultPadding));
            self.DrawBar(barPercent);

            self.Pad();
            self.Next();
            self.DrawLabel(barPercent.ToPercentageString());
            self.Gap();

            return true;
        }

        private static bool DrawMoodBar(this HudListing self, string label, float barPercent, float thresholdMinor, float thresholdMajor, float thresholdExtreme)
        {
            if (barPercent < 0f) { return false; }

            self.Next(BarLabelWidth);
            self.DrawLabel(label);

            self.Pad();
            self.Next(-(SkillValueWidth + HudListing.DefaultPadding));
            self.DrawBar(barPercent);
            self.DrawThreshold(thresholdMinor, Theme.MentalThresholdMinor);
            self.DrawThreshold(thresholdMajor, Theme.MentalThresholdMajor);
            self.DrawThreshold(thresholdExtreme, Theme.MentalThresholdExtreme);

            self.Pad();
            self.Next();
            self.DrawLabel(barPercent.ToPercentageString());
            self.Gap();

            return true;
        }

        private static void DrawThreshold(this HudListing self, float percent, Color color)
        {
            var previousColor = GUI.color;
            GUI.color = color;
            Widgets.DrawLineVertical(self.CurrentRect.x + (self.CurrentRect.width * percent), self.CurrentRect.y, self.CurrentRect.height);
            GUI.color = previousColor;
        }

        private static void DrawLabelledSkill(this HudListing self, SkillModel skill)
        {
            self.Next(SkillLabelWidth);
            self.DrawLabel(skill.Label, color: skill.Color);

            self.Next(SkillValueWidth);
            self.DrawLabel(skill.Level, color: skill.Color);
        }
        private static bool DrawSkillPair(this HudListing self, SkillModel left, SkillModel right)
        {
            if (!left.Disabled)
            {
                self.DrawLabelledSkill(left);
                self.Pad(Inner.width - ((SkillLabelWidth + SkillValueWidth) * 2f));
            }

            if (!right.Disabled) { self.DrawLabelledSkill(right); }
            if (left.Disabled && right.Disabled) { return false; }

            self.Gap();
            return true;
        }
    }
}
