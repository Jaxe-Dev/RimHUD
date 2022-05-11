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
  internal class SkillModel : IValueModel
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
    public Func<string> Tooltip { get; }
    public Action OnHover { get; }
    public Action OnClick { get; }

    public SkillDef Def { get; }
    public SkillRecord Skill { get; }

    public SkillModel(PawnModel model, SkillDef def)
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

      var isActive = model.Base.jobs?.curDriver?.ActiveSkill == def;
      var isSaturated = skill.LearningSaturatedToday;

      Label = (def.GetLabelCap() + passionIndicator).Colorize(skill.TotallyDisabled ? Theme.DisabledColor.Value : isSaturated ? Theme.SkillSaturatedColor.Value : isActive ? Theme.SkillActiveColor.Value : GetSkillColor(skill));

      Value = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(Math.Max(0, Math.Min(99, skill.XpProgressPercent.ToPercentageInt())));

      Tooltip = GetTooltip;

      OnClick = InspectPanePlus.ToggleBioTab;
    }

    private static Color GetSkillColor(SkillRecord skill)
    {
      if (skill.passion == Passion.Minor) { return Theme.SkillMinorPassionColor.Value; }
      if (skill.passion == Passion.Major) { return Theme.SkillMajorPassionColor.Value; }

      return Theme.MainTextColor.Value;
    }

    private string GetSkillDescription() => (string) Access.Method_RimWorld_SkillUI_GetSkillDescription.Invoke(null, Skill);

    private string GetTooltip()
    {
      var builder = new StringBuilder();

      builder.AppendLine(GetSkillDescription());
      builder.AppendLine();

      if (Def == SkillDefOf.Shooting)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.ShootingAccuracyPawn);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.AimingDelayFactor);
      }
      else if (Def == SkillDefOf.Melee)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MeleeDPS);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MeleeHitChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MeleeDodgeChance);
      }
      else if (!Skill.TotallyDisabled)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.WorkSpeedGlobal);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.GeneralLaborSpeed);
        builder.AppendLine();
      }
      if (Def == SkillDefOf.Construction)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.ConstructSuccessChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.ConstructionSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.FixBrokenDownBuildingSuccessChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.SmoothingSpeed);
      }
      else if (Def == SkillDefOf.Mining)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MiningYield);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MiningSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.DeepDrillingSpeed);
      }
      else if (Def == SkillDefOf.Cooking)
      {
        HudModel.BuildStatString(Model.Base, builder, CookSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.FoodPoisonChance);
        HudModel.BuildStatString(Model.Base, builder, ButcheryFleshEfficiency);
        HudModel.BuildStatString(Model.Base, builder, ButcheryFleshSpeed);
        HudModel.BuildStatString(Model.Base, builder, DrugCookingSpeed);
      }
      else if (Def == SkillDefOf.Plants)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.PlantWorkSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.PlantHarvestYield);
        if (ModsConfig.IdeologyActive) { HudModel.BuildStatString(Model.Base, builder, StatDefOf.PruningSpeed); }
      }
      else if (Def == SkillDefOf.Animals)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.AnimalGatherSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.AnimalGatherYield);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.TameAnimalChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.TrainAnimalChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.HuntingStealth);
      }
      else if (Def == SkillDefOf.Crafting)
      {
        HudModel.BuildStatString(Model.Base, builder, SmeltingSpeed);
        HudModel.BuildStatString(Model.Base, builder, ButcheryMechanoidSpeed);
        HudModel.BuildStatString(Model.Base, builder, ButcheryMechanoidEfficiency);
      }
      else if (Def == SkillDefOf.Artistic) { }
      else if (Def == SkillDefOf.Medicine)
      {
        HudModel.BuildStatString(Model.Base, builder, MedicalOperationSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MedicalSurgerySuccessChance);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MedicalTendSpeed);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.MedicalTendQuality);
      }
      else if (Def == SkillDefOf.Social)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.NegotiationAbility);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.TradePriceImprovement);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.Beauty);
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.SocialImpact);
        if (ModsConfig.RoyaltyActive) { HudModel.BuildStatString(Model.Base, builder, StatDefOf.ConversionPower); }
        if (ModsConfig.IdeologyActive) { HudModel.BuildStatString(Model.Base, builder, StatDefOf.SuppressionPower); }
      }
      else if (Def == SkillDefOf.Intellectual)
      {
        HudModel.BuildStatString(Model.Base, builder, StatDefOf.ResearchSpeed);
        if (ModsConfig.IdeologyActive) { HudModel.BuildStatString(Model.Base, builder, StatDefOf.HackingSpeed); }
      }

      return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
    }
  }
}
