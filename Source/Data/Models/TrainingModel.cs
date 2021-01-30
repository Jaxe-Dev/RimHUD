using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class TrainingModel : ValueModel
    {
        public override bool Hidden { get; }

        public override string Label { get; }
        public override string Value { get; }
        public override Color? Color { get; }
        public override TipSignal? Tooltip { get; }

        public TrainingModel(PawnModel model, TrainableDef def) : base(model)
        {
            bool canTrainNow;
            try { canTrainNow = model.Base.RaceProps?.trainability != null && model.Base.training != null && model.Base.training.CanAssignToTrain(def, out var visible).Accepted && visible; }
            catch { canTrainNow = false; }

            if (!canTrainNow)
            {
                Hidden = true;
                return;
            }

            Label = def.LabelCap;

            var disabled = !model.Base.training.GetWanted(def);
            var hasLearned = model.Base.training.HasLearned(def);

            var steps = GetSteps(model.Base, def);
            var value = steps + " / " + def.steps;
            Value = hasLearned ? value.Bold() : value;

            Tooltip = model.GetAnimalTooltip(def);

            if (disabled) { Color = Theme.DisabledColor.Value; }
            else if (hasLearned) { Color = Theme.SkillMinorPassionColor.Value; }
            else { Color = Theme.MainTextColor.Value; }

            OnClick = InspectPanePlus.ToggleTrainingTab;
        }

        private static int GetSteps(Pawn pawn, TrainableDef def) => (int) Access.Method_RimWorld_Pawn_TrainingTracker_GetSteps.Invoke(pawn.training, def);
    }
}
