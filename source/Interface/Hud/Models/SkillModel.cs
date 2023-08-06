using System;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class SkillModel : IModelValue
  {
    private static readonly StatDef SmeltingSpeed = DefDatabase<StatDef>.GetNamed("SmeltingSpeed");
    private static readonly StatDef ButcheryMechanoidSpeed = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidSpeed");
    private static readonly StatDef ButcheryMechanoidEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryMechanoidEfficiency");

    private static readonly StatDef MedicalOperationSpeed = DefDatabase<StatDef>.GetNamed("MedicalOperationSpeed");

    private static readonly StatDef CookSpeed = DefDatabase<StatDef>.GetNamed("CookSpeed");
    private static readonly StatDef ButcheryFleshSpeed = DefDatabase<StatDef>.GetNamed("ButcheryFleshSpeed");
    private static readonly StatDef ButcheryFleshEfficiency = DefDatabase<StatDef>.GetNamed("ButcheryFleshEfficiency");
    private static readonly StatDef DrugCookingSpeed = DefDatabase<StatDef>.GetNamed("DrugCookingSpeed");

    public PawnModel Owner { get; }

    public bool Hidden { get; }

    public string Label { get; }
    public string Value { get; }
    public Func<string> Tooltip { get; }
    public Action OnHover => null;
    public Action OnClick { get; }

    private readonly Def _def;
    private readonly SkillRecord _skill;

    public SkillModel(PawnModel owner, SkillDef def)
    {
      Owner = owner;
      _def = def;

      var skill = owner.Base.skills?.GetSkill(def);

      if (skill == null)
      {
        Hidden = true;
        return;
      }

      _skill = skill;

      var passionIndicator = new StringBuilder().Insert(0, Lang.Get("Model.Skill.PassionIndicator"), (int)skill.passion).ToString();

      var isActive = owner.Base.jobs?.curDriver?.ActiveSkill == def;
      var isSaturated = skill.LearningSaturatedToday;

      var color = skill.TotallyDisabled ? Theme.DisabledColor.Value : isSaturated ? Theme.SkillSaturatedColor.Value : isActive ? Theme.SkillActiveColor.Value : GetSkillColor(skill);
      Label = (def.GetLabelCap() + passionIndicator).Colorize(color);

      Value = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(Math.Max(0, Math.Min(99, skill.XpProgressPercent.ToPercentageInt()))).Colorize(color);

      Tooltip = GetTooltip;

      OnClick = InspectPanePlus.ToggleBioTab;
    }

    private static Color GetSkillColor(SkillRecord skill)
    {
      switch (skill.passion)
      {
        case Passion.Minor:
          return Theme.SkillMinorPassionColor.Value;
        case Passion.Major:
          return Theme.SkillMajorPassionColor.Value;
        default:
          return Theme.MainTextColor.Value;
      }
    }

    private string GetSkillDescription() => (string)Access.Method_RimWorld_SkillUI_GetSkillDescription.Invoke(null, _skill);

    private string GetTooltip()
    {
      var builder = new StringBuilder();

      builder.AppendLine(GetSkillDescription());
      builder.AppendLine();

      if (_def == SkillDefOf.Shooting)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.ShootingAccuracyPawn);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.AimingDelayFactor);
      }
      else if (_def == SkillDefOf.Melee)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MeleeDPS);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MeleeHitChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MeleeDodgeChance);
      }
      else if (!_skill.TotallyDisabled)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.WorkSpeedGlobal);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.GeneralLaborSpeed);
        builder.AppendLine();
      }
      if (_def == SkillDefOf.Construction)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.ConstructSuccessChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.ConstructionSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.FixBrokenDownBuildingSuccessChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.SmoothingSpeed);
      }
      else if (_def == SkillDefOf.Mining)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MiningYield);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MiningSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.DeepDrillingSpeed);
      }
      else if (_def == SkillDefOf.Cooking)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, CookSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.FoodPoisonChance);
        HudBuilder.BuildStatString(Owner.Base, builder, ButcheryFleshEfficiency);
        HudBuilder.BuildStatString(Owner.Base, builder, ButcheryFleshSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, DrugCookingSpeed);
      }
      else if (_def == SkillDefOf.Plants)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.PlantWorkSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.PlantHarvestYield);
        if (ModsConfig.IdeologyActive) { HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.PruningSpeed); }
      }
      else if (_def == SkillDefOf.Animals)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.AnimalGatherSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.AnimalGatherYield);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.TameAnimalChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.TrainAnimalChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.HuntingStealth);
      }
      else if (_def == SkillDefOf.Crafting)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, SmeltingSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, ButcheryMechanoidSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, ButcheryMechanoidEfficiency);
      }
      else if (_def == SkillDefOf.Artistic) { }
      else if (_def == SkillDefOf.Medicine)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, MedicalOperationSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MedicalSurgerySuccessChance);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MedicalTendSpeed);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.MedicalTendQuality);
      }
      else if (_def == SkillDefOf.Social)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.NegotiationAbility);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.TradePriceImprovement);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.Beauty);
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.SocialImpact);
        if (ModsConfig.RoyaltyActive) { HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.ConversionPower); }
        if (ModsConfig.IdeologyActive) { HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.SuppressionPower); }
      }
      else if (_def == SkillDefOf.Intellectual)
      {
        HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.ResearchSpeed);
        if (ModsConfig.IdeologyActive) { HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.HackingSpeed); }
      }

      return builder.ToTooltip();
    }
  }
}
