using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class HudDocked
    {
        public static void OnGUI(PawnModel model, Rect rect)
        {
            if (!State.ValidSelected) { return; }

            GUI.BeginGroup(rect);

            Draw(rect.AtZero(), model);

            GUI.EndGroup();
        }

        private static void Draw(Rect bounds, PawnModel model)
        {
            var mouseOver = Mouse.IsOver(bounds);

            var grid = bounds.GetHGrid(GUIPlus.MediumPadding, -1f, -1f);

            var l = new HudLayout();
            l.Bind(grid[1]);

            var relationColor = model.FactionRelationColor;

            if (mouseOver && Widgets.ButtonImage(new Rect(bounds.xMax - 16f, bounds.yMax - 16f, 16f, 16f), Theme.ConfigIcon)) { Dialog_Config.Open(); }

            l.DrawDescriptionRow(model.RelationKindAndFaction, relationColor);
            l.DrawDescriptionRow(model.GenderAndAge);
            l.PadLine();

            var usesHealth = l.DrawLabelledBar(Lang.Get("Bar.Health"), model.Health);

            var healthCondition = model.HealthCondition;
            if (healthCondition != null) { l.DrawDescriptionRow(healthCondition.Text, healthCondition.Color); }
            if (model.HasNeeds && (usesHealth || (healthCondition != null))) { l.PadLine(); }

            var usesMood = l.DrawMoodBar(Lang.Get("Bar.Mood"), model.Mood, model.MoodThresholdMinor, model.MoodThresholdMajor, model.MoodThresholdExtreme);
            var mentalCondition = model.MentalCondition;
            if (mentalCondition != null) { l.DrawDescriptionRow(mentalCondition.Text, mentalCondition.Color); }
            if (usesMood || (mentalCondition != null)) { l.PadLine(); }

            l.DrawLabelledBar(Lang.Get("Bar.Rest"), model.Rest);
            l.DrawLabelledBar(Lang.Get("Bar.Food"), model.Food);
            l.DrawLabelledBar(Lang.Get("Bar.Recreation"), model.Recreation);

            l.Bind(grid[2]);

            l.DrawDescriptionRow(model.Activity);
            l.DrawDescriptionRow(model.Equipped);

            l.PadLine();

            l.DrawSkillPair(model.Shooting, model.Melee);
            l.DrawSkillPair(model.Construction, model.Mining);
            l.DrawSkillPair(model.Cooking, model.Medicine);
            l.DrawSkillPair(model.Plants, model.Animals);
            l.DrawSkillPair(model.Crafting, model.Artistic);
            l.DrawSkillPair(model.Social, model.Intellectual);
        }
    }
}
