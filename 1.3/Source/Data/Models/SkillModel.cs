using System;
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
    internal struct SkillModel : IValueModel
    {
        private static readonly StatDef SmeltingSpeed = DefDatabase<StatDef>.GetNamed("SmeltingSpeed");
        private static readonly StatDef ButcheryMechanoidSpeed = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidSpeed");
        private static readonly StatDef ButcheryMechanoidEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidEfficiency");

        private static readonly StatDef MedicalOperationSpeed = DefDatabase<StatDef>.GetNamed("MedicalOperationSpeed");

        private static readonly StatDef CookSpeed = DefDatabase<StatDef>.GetNamed("CookSpeed");
        private static readonly StatDef ButcheryFleshSpeed = DefDatabase<StatDef>.GetNamed("ButcheryFleshSpeed");
        private static readonly StatDef ButcheryFleshEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryFleshEfficiency");
        private static readonly StatDef DrugCookingSpeed = DefDatabase<StatDef>.GetNamed("DrugCookingSpeed");

        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public string Value { get; }
        public Color? Color { get; }
        public TipSignal? Tooltip => GetTooltip();
        public Action OnHover { get; }
        public Action OnClick { get; }

        public SkillDef Def { get; }
        public SkillRecord Skill { get; }

        public SkillModel(PawnModel model, SkillDef def) : this()
        {
            Model = model;
            Def = def;

            var skill = model.Base.skills?.GetSkill(def);

            if (skill == null)
            {
                Hidden = true;
                return;
            }

            Skill = skill;

            var passionIndicator = new StringBuilder().Insert(0, Lang.Get("Model.Component.Skill.PassionIndicator"), (int) skill.passion).ToString();
            Label = def.LabelCap + passionIndicator;

            var isActive = model.Base.jobs?.curDriver?.ActiveSkill == def;
            var isSaturated = skill.LearningSaturatedToday;

            Value = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(skill.XpProgressPercent.ToPercentageInt());
            Color = skill.TotallyDisabled ? Theme.DisabledColor.Value : isSaturated ? Theme.SkillSaturatedColor.Value : isActive ? Theme.SkillActiveColor.Value : GetSkillColor(skill);

            OnClick = InspectPanePlus.ToggleBioTab;
        }

        private static Color GetSkillColor(SkillRecord skill)
        {
            if (skill.passion == Passion.Minor) { return Theme.SkillMinorPassionColor.Value; }
            if (skill.passion == Passion.Major) { return Theme.SkillMajorPassionColor.Value; }

            return Theme.MainTextColor.Value;
        }

        private string GetSkillDescription() => (string) Access.Method_RimWorld_SkillUI_GetSkillDescription.Invoke(null, Skill);

        private TipSignal? GetTooltip()
        {
            var builder = new StringBuilder();

            builder.AppendLine(GetSkillDescription());
            if (Skill.TotallyDisabled) { return new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId); }
            builder.AppendLine();

            if (Def == SkillDefOf.Shooting)
            {
                HudModel.BuildStatString(this, builder, StatDefOf.ShootingAccuracyPawn);
                HudModel.BuildStatString(this, builder, StatDefOf.AimingDelayFactor);
            }
            else if (Def == SkillDefOf.Melee)
            {
                HudModel.BuildStatString(this, builder, StatDefOf.MeleeDPS);
                HudModel.BuildStatString(this, builder, StatDefOf.MeleeHitChance);
                HudModel.BuildStatString(this, builder, StatDefOf.MeleeDodgeChance);
            }
            else if (!Skill.TotallyDisabled)
            {
                HudModel.BuildStatString(this, builder, StatDefOf.WorkSpeedGlobal);
                HudModel.BuildStatString(this, builder, StatDefOf.GeneralLaborSpeed);
                builder.AppendLine();

                if (Def == SkillDefOf.Construction)
                {
                    HudModel.BuildStatString(this, builder, StatDefOf.ConstructSuccessChance);
                    HudModel.BuildStatString(this, builder, StatDefOf.ConstructionSpeedFactor);
                    HudModel.BuildStatString(this, builder, StatDefOf.FixBrokenDownBuildingSuccessChance);
                    HudModel.BuildStatString(this, builder, StatDefOf.SmoothingSpeed);
                }
                else if (Def == SkillDefOf.Mining)
                {
                    HudModel.BuildStatString(this, builder, StatDefOf.MiningSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.MiningYield);
                }
                else if (Def == SkillDefOf.Cooking)
                {
                    HudModel.BuildStatString(this, builder, CookSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.FoodPoisonChance);
                    HudModel.BuildStatString(this, builder, ButcheryFleshSpeed);
                    HudModel.BuildStatString(this, builder, ButcheryFleshEfficiency);
                    HudModel.BuildStatString(this, builder, DrugCookingSpeed);
                }
                else if (Def == SkillDefOf.Plants)
                {
                    HudModel.BuildStatString(this, builder, StatDefOf.PlantWorkSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.PlantHarvestYield);
                }
                else if (Def == SkillDefOf.Animals)
                {
                    HudModel.BuildStatString(this, builder, StatDefOf.AnimalGatherSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.AnimalGatherYield);
                    HudModel.BuildStatString(this, builder, StatDefOf.TameAnimalChance);
                    HudModel.BuildStatString(this, builder, StatDefOf.TrainAnimalChance);
                    HudModel.BuildStatString(this, builder, StatDefOf.HuntingStealth);
                }
                else if (Def == SkillDefOf.Crafting)
                {
                    HudModel.BuildStatString(this, builder, SmeltingSpeed);
                    HudModel.BuildStatString(this, builder, ButcheryMechanoidSpeed);
                    HudModel.BuildStatString(this, builder, ButcheryMechanoidEfficiency);
                }
                else if (Def == SkillDefOf.Artistic) { }
                else if (Def == SkillDefOf.Medicine)
                {
                    HudModel.BuildStatString(this, builder, MedicalOperationSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.MedicalSurgerySuccessChance);
                    HudModel.BuildStatString(this, builder, StatDefOf.MedicalTendSpeed);
                    HudModel.BuildStatString(this, builder, StatDefOf.MedicalTendQuality);
                }
                else if (Def == SkillDefOf.Social)
                {
                    HudModel.BuildStatString(this, builder, StatDefOf.NegotiationAbility);
                    HudModel.BuildStatString(this, builder, StatDefOf.TradePriceImprovement);
                    HudModel.BuildStatString(this, builder, StatDefOf.SocialImpact);
                }
                else if (Def == SkillDefOf.Intellectual) { HudModel.BuildStatString(this, builder, StatDefOf.ResearchSpeedFactor); }
            }

            return builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
        }
    }
}
