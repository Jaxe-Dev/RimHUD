using System.Linq;
using RimHUD.Data;
using RimHUD.Interface.Dialog;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal static class HudDocked
    {
        public static void OnGUI(PawnModel model, Rect rect)
        {
            if (!State.PawnSelected) { return; }

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

            if (mouseOver && Widgets.ButtonImage(new Rect(bounds.xMax - 16f, bounds.yMax - 16f, 16f, 16f), Textures.ConfigIcon)) { Dialog_Config.Open(); }

            l.DrawDescriptionRow(model.RelationKindAndFaction, relationColor, model.BioTooltip);
            l.PadLine();

            var usesHealth = l.DrawLabelledBar(Lang.Get("Bar.Health"), model.Health, tooltip: model.HealthTooltip);

            var healthCondition = model.HealthCondition;
            if (healthCondition != null) { l.DrawDescriptionRow(healthCondition.Text, healthCondition.Color, model.HealthTooltip); }
            if (model.HasNeeds && (usesHealth || (healthCondition != null))) { l.PadDown(); }

            var usesMood = l.DrawLabelledBar(Lang.Get("Bar.Mood"), model.Mood, model.MoodThresholdMinor, model.MoodThresholdMajor, model.MoodThresholdExtreme, model.MoodTooltip);
            var mentalCondition = model.MentalCondition;
            if (mentalCondition != null) { l.DrawDescriptionRow(mentalCondition.Text, mentalCondition.Color, model.MoodTooltip); }

            if (usesMood || (mentalCondition != null)) { l.PadDown(); }

            l.DrawLabelledBar(Lang.Get("Bar.Rest"), model.Rest);
            l.DrawLabelledBar(Lang.Get("Bar.Food"), model.Food);
            l.DrawLabelledBar(Lang.Get("Bar.Recreation"), model.Recreation);

            if (model.HasNeeds && Theme.IntegrationGeneralShowCustomNeeds.Value)
            {
                var otherNeeds = model.Base.needs.AllNeeds.Where(need => !Theme.SkippedNeeds.Contains(need.def) && (need.def.needClass != typeof(Need_Chemical)) && need.ShowOnNeedList).ToArray();
                foreach (var need in otherNeeds) { l.DrawLabelledBar(need.LabelCap, need.CurLevelPercentage); }
            }

            var bottomRowsY = grid[0].yMax - ((Theme.SmallTextStyle.LineHeight * 2) + (Layout.DefaultPadding * 2) + Layout.DefaultLinePadding);
            l.SetY(bottomRowsY);
            l.PadLine();
            l.DrawDescriptionRow(model.Activity, textToTooltip: true);
            l.DrawDescriptionRow(model.Queued, textToTooltip: true);

            l.Bind(grid[2]);

            l.DrawDescriptionRow(model.GenderAndAge, tooltip: model.BioTooltip);
            l.PadLine();

            if (model.HasSkills)
            {
                l.DrawStatPair(model.Shooting, model.Melee);
                l.DrawStatPair(model.Construction, model.Mining);
                l.DrawStatPair(model.Cooking, model.Medicine);
                l.DrawStatPair(model.Plants, model.Animals);
                l.DrawStatPair(model.Crafting, model.Artistic);
                l.DrawStatPair(model.Social, model.Intellectual);
            }

            if (model.HasTraining)
            {
                l.DrawStatPair(model.Tameness, null);
                l.DrawStatPair(model.Obedience, model.Release);
                l.DrawStatPair(model.Rescue, model.Haul);
            }

            var compInfo = model.CompInfo;
            if (compInfo != null) { l.DrawDescriptionRow(model.CompInfo, textToTooltip: true); }

            l.SetY(bottomRowsY);
            l.PadLine();
            var equipped = model.Equipped;
            if (equipped != null) { l.DrawDescriptionRow(model.Equipped, textToTooltip: true); }
            l.DrawDescriptionRow(model.Carrying, textToTooltip: true);
        }
    }
}
