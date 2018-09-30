using System;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal class TrainingModel : StatModel
    {
        public override bool Hidden { get; }

        public override string Label { get; }
        public override string Level { get; }
        public override Color Color { get; }
        public override Func<string> Tooltip { get; } = null;

        public TrainingModel(Pawn pawn, TrainableDef def) : base(pawn)
        {
            var visible = false;
            var canTrain = pawn.training?.CanAssignToTrain(def, out visible);
            if ((!canTrain?.Accepted ?? false) || !visible)
            {
                Hidden = true;
                return;
            }

            Label = def.LabelCap;

            var disabled = !pawn.training.GetWanted(def);
            var hasLearned = pawn.training.HasLearned(def);

            var steps = GetSteps(pawn, def);
            var value = steps + " / " + def.steps;
            Level = hasLearned ? value.Bold() : value;

            if (disabled) { Color = Theme.SkillDisabledColor.Value; }
            else if (hasLearned) { Color = Theme.SkillMinorPassionColor.Value; }
            else { Color = Theme.MainTextColor.Value; }
        }

        private static int GetSteps(Pawn pawn, TrainableDef def) => (int) Access.Method_RimWorld_Pawn_TrainingTracker_GetSteps.Invoke(pawn.training, new object[] { def });
    }
}
