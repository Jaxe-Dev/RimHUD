using System.Text;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class SkillModel : ValueModel
    {
        private static readonly StatDef SmeltingSpeed = DefDatabase<StatDef>.GetNamed("SmeltingSpeed");
        private static readonly StatDef SmithingSpeed = DefDatabase<StatDef>.GetNamed("SmithingSpeed");
        private static readonly StatDef TailoringSpeed = DefDatabase<StatDef>.GetNamed("TailoringSpeed");
        private static readonly StatDef ButcheryMechanoidSpeed = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidSpeed");
        private static readonly StatDef ButcheryMechanoidEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidEfficiency");

        private static readonly StatDef SculptingSpeed = DefDatabase<StatDef>.GetNamed("SculptingSpeed");

        private static readonly StatDef MedicalOperationSpeed = DefDatabase<StatDef>.GetNamed("MedicalOperationSpeed");

        private static readonly StatDef CookSpeed = DefDatabase<StatDef>.GetNamed("CookSpeed");
        private static readonly StatDef ButcheryFleshSpeed = DefDatabase<StatDef>.GetNamed("ButcheryFleshSpeed");
        private static readonly StatDef ButcheryFleshEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryFleshEfficiency");
        private static readonly StatDef DrugCookingSpeed = DefDatabase<StatDef>.GetNamed("DrugCookingSpeed");
        public override bool Hidden { get; }

        public override string Label { get; }
        public override string Value { get; }
        public override Color? Color { get; }
        public SkillDef Def { get; }
        public SkillRecord Skill { get; }

        public override TipSignal? Tooltip => GetTooltip();

        public SkillModel(PawnModel model, SkillDef def) : base(model)
        {
            Def = def;

            var skill = model.Base.skills?.GetSkill(def);

            if (skill == null)
            {
                Hidden = true;
                return;
            }

            Skill = skill;

            Label = def.LabelCap + new string('+', (int) skill.passion);
            Value = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(skill.XpProgressPercent.ToPercentageInt()) + (skill.LearningSaturatedToday ? "*" : null);
            Color = skill.TotallyDisabled ? Theme.DisabledColor.Value : GetSkillPassionColor(skill.passion);

            OnClick = InspectPanePlus.ToggleBioTab;
        }

        private static Color GetSkillPassionColor(Passion passion)
        {
            if (passion == Passion.None) { return Theme.MainTextColor.Value; }
            if (passion == Passion.Minor) { return Theme.SkillMinorPassionColor.Value; }
            if (passion == Passion.Major) { return Theme.SkillMajorPassionColor.Value; }

            throw new Mod.Exception("Invalid skill passion level.");
        }

        private string GetSkillDescription() => (string) Access.Method_RimWorld_SkillUI_GetSkillDescription.Invoke(null, new object[] { Skill });

        private TipSignal? GetTooltip()
        {
            var builder = new StringBuilder();

            builder.AppendLine(GetSkillDescription());
            if (Skill.TotallyDisabled) { return new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId); }
            builder.AppendLine();

            if (Def == SkillDefOf.Shooting)
            {
                BuildStatString(builder, StatDefOf.ShootingAccuracyPawn);
                BuildStatString(builder, StatDefOf.AimingDelayFactor);
            }
            else if (Def == SkillDefOf.Melee)
            {
                BuildStatString(builder, StatDefOf.MeleeDPS);
                BuildStatString(builder, StatDefOf.MeleeHitChance);
                BuildStatString(builder, StatDefOf.MeleeDodgeChance);
            }
            else if (!Skill.TotallyDisabled)
            {
                BuildStatString(builder, StatDefOf.WorkSpeedGlobal);
                BuildStatString(builder, StatDefOf.UnskilledLaborSpeed);
                builder.AppendLine();

                if (Def == SkillDefOf.Construction)
                {
                    BuildStatString(builder, StatDefOf.ConstructSuccessChance);
                    BuildStatString(builder, StatDefOf.ConstructionSpeedFactor);
                    BuildStatString(builder, StatDefOf.FixBrokenDownBuildingSuccessChance);
                    BuildStatString(builder, StatDefOf.SmoothingSpeed);
                }
                else if (Def == SkillDefOf.Mining)
                {
                    BuildStatString(builder, StatDefOf.MiningSpeed);
                    BuildStatString(builder, StatDefOf.MiningYield);
                }
                else if (Def == SkillDefOf.Cooking)
                {
                    BuildStatString(builder, CookSpeed);
                    BuildStatString(builder, StatDefOf.FoodPoisonChance);
                    BuildStatString(builder, ButcheryFleshSpeed);
                    BuildStatString(builder, ButcheryFleshEfficiency);
                    BuildStatString(builder, DrugCookingSpeed);
                }
                else if (Def == SkillDefOf.Plants)
                {
                    BuildStatString(builder, StatDefOf.PlantWorkSpeed);
                    BuildStatString(builder, StatDefOf.PlantHarvestYield);
                }
                else if (Def == SkillDefOf.Animals)
                {
                    BuildStatString(builder, StatDefOf.AnimalGatherSpeed);
                    BuildStatString(builder, StatDefOf.AnimalGatherYield);
                    BuildStatString(builder, StatDefOf.TameAnimalChance);
                    BuildStatString(builder, StatDefOf.TrainAnimalChance);
                    BuildStatString(builder, StatDefOf.HuntingStealth);
                }
                else if (Def == SkillDefOf.Crafting)
                {
                    BuildStatString(builder, SmeltingSpeed);
                    BuildStatString(builder, SmithingSpeed);
                    BuildStatString(builder, TailoringSpeed);
                    BuildStatString(builder, ButcheryMechanoidSpeed);
                    BuildStatString(builder, ButcheryMechanoidEfficiency);
                }
                else if (Def == SkillDefOf.Artistic) { BuildStatString(builder, SculptingSpeed); }
                else if (Def == SkillDefOf.Medicine)
                {
                    BuildStatString(builder, MedicalOperationSpeed);
                    BuildStatString(builder, StatDefOf.MedicalSurgerySuccessChance);
                    BuildStatString(builder, StatDefOf.MedicalTendSpeed);
                    BuildStatString(builder, StatDefOf.MedicalTendQuality);
                }
                else if (Def == SkillDefOf.Social)
                {
                    BuildStatString(builder, StatDefOf.NegotiationAbility);
                    BuildStatString(builder, StatDefOf.TradePriceImprovement);
                    BuildStatString(builder, StatDefOf.SocialImpact);
                }
                else if (Def == SkillDefOf.Intellectual) { BuildStatString(builder, StatDefOf.ResearchSpeedFactor); }
            }

            return builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
        }
    }
}
