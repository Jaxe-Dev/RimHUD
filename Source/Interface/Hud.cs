using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class Hud
    {
        private const float Padding = 4f;

        public static Rect Bounds => Theme.GetHudBounds();

        public static bool Activated { get; set; } = true;
        public static bool Visible => Activated && IsReady && (PawnModel.Selected != null);
        public static bool IsReady => Activated && (Current.Game != null) && (Current.Game.CurrentMap != null);

        public static void OnGUI()
        {
            if (!IsReady) { return; }

            var model = PawnModel.Selected;
            if (model == null) { return; }

            var bounds = Bounds;
            GUI.BeginGroup(bounds);

            Draw(bounds.AtZero(), model);

            GUI.EndGroup();
        }

        private static void Draw(Rect bounds, PawnModel model)
        {
            GUIPlus.DrawBackground(bounds);
            var inner = bounds.ContractedBy(Padding);
            var mouseOver = Mouse.IsOver(inner);

            var l = new HudLayout();
            l.Bind(inner);

            var relationColor = model.FactionRelationColor;

            l.DrawTitleRow(model.Name.Bold(), relationColor);

            if (mouseOver && Widgets.ButtonImage(new Rect(inner.xMax - 16f, inner.y, 16f, 16f), Theme.ConfigIcon)) { Dialog_Config.Open(); }
            l.PadLine();

            l.DrawDescriptionRow(model.RelationKindAndFaction, relationColor);
            l.DrawDescriptionRow(model.GenderAndAge);
            l.PadLine();

            var usesHealth = l.DrawLabelledBar(Lang.Get("Bar.Health"), model.Health);

            var healthCondition = model.HealthCondition;
            if (healthCondition != null) { l.DrawDescriptionRow(healthCondition?.Text ?? null, healthCondition?.Color); }
            if (usesHealth || (healthCondition != null)) { l.PadLine(); }

            var usesMood = l.DrawMoodBar(Lang.Get("Bar.Mood"), model.Mood, model.MoodThresholdMinor, model.MoodThresholdMajor, model.MoodThresholdExtreme);
            var mentalCondition = model.MentalCondition;
            if (mentalCondition != null) { l.DrawDescriptionRow(mentalCondition?.Text ?? null, mentalCondition?.Color); }
            if (usesMood || (mentalCondition != null)) { l.PadLine(); }

            var usesOtherNeeds = l.DrawLabelledBar(Lang.Get("Bar.Rest"), model.Rest);
            usesOtherNeeds |= l.DrawLabelledBar(Lang.Get("Bar.Food"), model.Food);
            usesOtherNeeds |= l.DrawLabelledBar(Lang.Get("Bar.Recreation"), model.Recreation);
            if (usesOtherNeeds) { l.PadLine(); }

            var usesSkills = l.DrawSkillPair(model.Shooting, model.Melee);
            usesSkills |= l.DrawSkillPair(model.Construction, model.Mining);
            usesSkills |= l.DrawSkillPair(model.Cooking, model.Medicine);
            usesSkills |= l.DrawSkillPair(model.Plants, model.Animals);
            usesSkills |= l.DrawSkillPair(model.Crafting, model.Artistic);
            usesSkills |= l.DrawSkillPair(model.Social, model.Intellectual);
            if (usesSkills) { l.PadLine(); }

            var equipped = model.Equipped;
            if (equipped != null) { l.DrawLabelRow(model.Equipped); }
            var activity = model.Activity;
            if (activity != null) { l.DrawLabelRow(model.Activity); }
        }
    }
}
