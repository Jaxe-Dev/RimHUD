using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Compatibility;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public static class HudContent
  {
    public const string StatTypeName = "Stat";
    public const string RecordTypeName = "Record";
    public const string NeedTypeName = "Need";
    public const string SkillTypeName = "Skill";
    public const string TrainingTypeName = "Training";

    private static readonly string[] StandardNeedDefs =
    {
      "Food",
      "Joy",
      "Mood",
      "Rest",
      "Suppression"
    };

    private static readonly string[] StandardSkillDefs =
    {
      "Shooting",
      "Melee",
      "Construction",
      "Mining",
      "Cooking",
      "Plants",
      "Animals",
      "Crafting",
      "Artistic",
      "Medicine",
      "Social",
      "Intellectual"
    };

    private static readonly string[] StandardTrainingDefs =
    {
      "Tameness",
      "Obedience",
      "Release",
      "Rescue",
      "Haul"
    };

    private static Dictionary<string, Func<PawnModel, object[], IWidget>> BaseWidgets { get; } = new Dictionary<string, Func<PawnModel, object[], IWidget>>
    {
      { BlankWidget.Id, (owner, parameters) => BlankWidget.GetEmpty },
      { SeparatorWidget.Id, (owner, parameters) => SeparatorWidget.Get() },

      { "NameHeader", (owner, parameters) => ValueWidget.FromModel(owner.Name, Theme.LargeTextStyle) },

      { "Outfit", (owner, parameters) => SelectorWidget.FromSelectorModel(owner.OutfitSelector, Theme.SmallTextStyle) },
      { "Food", (owner, parameters) => SelectorWidget.FromSelectorModel(owner.FoodSelector, Theme.SmallTextStyle) },
      { "Timetable", (owner, parameters) => SelectorWidget.FromSelectorModel(owner.TimetableSelector, Theme.SmallTextStyle) },
      { "Area", (owner, parameters) => SelectorWidget.FromSelectorModel(owner.AreaSelector, Theme.SmallTextStyle) },

      { "RelationKindAndFaction", (owner, parameters) => ValueWidget.FromModel(owner.RelationKindFaction, Theme.SmallTextStyle) },
      { "GenderRaceAndAge", (owner, parameters) => ValueWidget.FromModel(owner.GenderRaceAndAge, Theme.SmallTextStyle) },
      { "SimpleGenderRaceAndAge", (owner, parameters) => ValueWidget.FromModel(owner.SimpleGenderRaceAndAge, Theme.SmallTextStyle) },

      { "Health", (owner, parameters) => BarWidget.FromModel(owner.Health.Bar, Theme.RegularTextStyle, parameters[0] as string) },
      { "HealthCondition", (owner, parameters) => ValueWidget.FromModel(owner.Health.Condition, Theme.SmallTextStyle) },
      { "MindCondition", (owner, parameters) => ValueWidget.FromModel(owner.Mind.Condition, Theme.SmallTextStyle) },

      { "Master", (owner, parameters) => ValueWidget.FromModel(owner.Master, Theme.RegularTextStyle) },

      { "Activity", (owner, parameters) => ValueWidget.FromModel(owner.Activity, Theme.SmallTextStyle) },
      { "Queued", (owner, parameters) => ValueWidget.FromModel(owner.Queued, Theme.SmallTextStyle) },
      { "Equipped", (owner, parameters) => ValueWidget.FromModel(owner.Equipped, Theme.SmallTextStyle) },
      { "Carrying", (owner, parameters) => ValueWidget.FromModel(owner.Carrying, Theme.SmallTextStyle) },
      { "CompInfo", (owner, parameters) => ValueWidget.FromModel(owner.CompInfo, Theme.SmallTextStyle) },
      { "PrisonerInfo", (owner, parameters) => ValueWidget.FromModel(owner.PrisonerInfo, Theme.RegularTextStyle) }
    };

    private static Dictionary<string, Func<PawnModel, object[], IWidget>> DefWidgets { get; } = new Dictionary<string, Func<PawnModel, object[], IWidget>>(BaseWidgets)
    {
      { "NeedMood", (owner, parameters) => BarWidget.FromModel(owner.Mood, Theme.RegularTextStyle, parameters[0] as string) },
      { "NeedFood", (owner, parameters) => BarWidget.FromModel(owner.Food, Theme.RegularTextStyle, parameters[0] as string) },
      { "NeedRest", (owner, parameters) => BarWidget.FromModel(owner.Rest, Theme.RegularTextStyle, parameters[0] as string) },
      { "NeedEnergy", (owner, parameters) => BarWidget.FromModel(owner.Energy, Theme.RegularTextStyle, parameters[0] as string) },
      { "NeedRecreation", (owner, parameters) => BarWidget.FromModel(owner.Recreation, Theme.RegularTextStyle, parameters[0] as string) },
      { "NeedSuppression", (owner, parameters) => BarWidget.FromModel(owner.Suppression, Theme.RegularTextStyle, parameters[0] as string) },

      { "SkillShooting", (owner, parameters) => ValueWidget.FromModel(owner.Shooting, Theme.RegularTextStyle) },
      { "SkillMelee", (owner, parameters) => ValueWidget.FromModel(owner.Melee, Theme.RegularTextStyle) },
      { "SkillConstruction", (owner, parameters) => ValueWidget.FromModel(owner.Construction, Theme.RegularTextStyle) },
      { "SkillMining", (owner, parameters) => ValueWidget.FromModel(owner.Mining, Theme.RegularTextStyle) },
      { "SkillCooking", (owner, parameters) => ValueWidget.FromModel(owner.Cooking, Theme.RegularTextStyle) },
      { "SkillPlants", (owner, parameters) => ValueWidget.FromModel(owner.Plants, Theme.RegularTextStyle) },
      { "SkillAnimals", (owner, parameters) => ValueWidget.FromModel(owner.Animals, Theme.RegularTextStyle) },
      { "SkillCrafting", (owner, parameters) => ValueWidget.FromModel(owner.Crafting, Theme.RegularTextStyle) },
      { "SkillArtistic", (owner, parameters) => ValueWidget.FromModel(owner.Artistic, Theme.RegularTextStyle) },
      { "SkillMedicine", (owner, parameters) => ValueWidget.FromModel(owner.Medicine, Theme.RegularTextStyle) },
      { "SkillSocial", (owner, parameters) => ValueWidget.FromModel(owner.Social, Theme.RegularTextStyle) },
      { "SkillIntellectual", (owner, parameters) => ValueWidget.FromModel(owner.Intellectual, Theme.RegularTextStyle) },

      { "TrainingTameness", (owner, parameters) => ValueWidget.FromModel(owner.Tameness, Theme.RegularTextStyle) },
      { "TrainingObedience", (owner, parameters) => ValueWidget.FromModel(owner.Obedience, Theme.RegularTextStyle) },
      { "TrainingRelease", (owner, parameters) => ValueWidget.FromModel(owner.Release, Theme.RegularTextStyle) },
      { "TrainingRescue", (owner, parameters) => ValueWidget.FromModel(owner.Rescue, Theme.RegularTextStyle) },
      { "TrainingHaul", (owner, parameters) => ValueWidget.FromModel(owner.Haul, Theme.RegularTextStyle) }
    };

    public static Dictionary<string, Func<PawnModel, object[], IWidget>> Widgets { get; } = BaseWidgets.Concat(DefWidgets).Concat(IntegrationManager.ThirdPartyWidgets.Select(item => new KeyValuePair<string, Func<PawnModel, object[], IWidget>>(item.Key, (model, parameters) => item.Value.Item2.Invoke(model.Base)))).ToDictionary();

    private static readonly LayoutElement[] StandardNeedElements =
    {
      new LayoutElement(LayoutElementType.Widget, "NeedMood"),
      new LayoutElement(LayoutElementType.Widget, "NeedFood"),
      new LayoutElement(LayoutElementType.Widget, "NeedRest"),
      new LayoutElement(LayoutElementType.Widget, "NeedEnergy"),
      new LayoutElement(LayoutElementType.Widget, "NeedRecreation"),
      new LayoutElement(LayoutElementType.Widget, "NeedSuppression")
    };

    private static readonly LayoutElement[] StandardSkillElements =
    {
      new LayoutElement(LayoutElementType.Widget, "SkillShooting"),
      new LayoutElement(LayoutElementType.Widget, "SkillMelee"),
      new LayoutElement(LayoutElementType.Widget, "SkillConstruction"),
      new LayoutElement(LayoutElementType.Widget, "SkillMining"),
      new LayoutElement(LayoutElementType.Widget, "SkillCooking"),
      new LayoutElement(LayoutElementType.Widget, "SkillPlants"),
      new LayoutElement(LayoutElementType.Widget, "SkillAnimals"),
      new LayoutElement(LayoutElementType.Widget, "SkillCrafting"),
      new LayoutElement(LayoutElementType.Widget, "SkillArtistic"),
      new LayoutElement(LayoutElementType.Widget, "SkillMedicine"),
      new LayoutElement(LayoutElementType.Widget, "SkillSocial"),
      new LayoutElement(LayoutElementType.Widget, "SkillIntellectual")
    };

    private static readonly LayoutElement[] StandardTrainingElements =
    {
      new LayoutElement(LayoutElementType.Widget, "TrainingTameness"),
      new LayoutElement(LayoutElementType.Widget, "TrainingObedience"),
      new LayoutElement(LayoutElementType.Widget, "TrainingRelease"),
      new LayoutElement(LayoutElementType.Widget, "TrainingRescue"),
      new LayoutElement(LayoutElementType.Widget, "TrainingHaul")
    };

    public static readonly LayoutElement[] BaseElements = BaseWidgets.Where(widget => widget.Key != "Health").Select(widget => new LayoutElement(LayoutElementType.Widget, widget.Key)).ToArray();
    public static readonly LayoutElement[] StatElements = DefDatabase<StatDef>.AllDefs.Select(stat => new LayoutElement(LayoutElementType.Widget, StatTypeName, stat)).OrderBy(item => item.Label).ToArray();
    public static readonly LayoutElement[] RecordElements = DefDatabase<RecordDef>.AllDefs.Select(record => new LayoutElement(LayoutElementType.Widget, RecordTypeName, record)).OrderBy(item => item.Label).ToArray();

    public static readonly LayoutElement[] NeedElements = StandardNeedElements.Concat(DefDatabase<NeedDef>.AllDefs.Where(def => !StandardNeedDefs.Contains(def.defName)).Select(def => new LayoutElement(LayoutElementType.DefWidget, NeedTypeName, def)).OrderBy(item => item.Label)).Prepend(new LayoutElement(LayoutElementType.Widget, "Health")).ToArray();
    public static readonly LayoutElement[] SkillElements = StandardSkillElements.Concat(DefDatabase<SkillDef>.AllDefs.Where(def => !StandardSkillDefs.Contains(def.defName)).Select(def => new LayoutElement(LayoutElementType.DefWidget, SkillTypeName, def)).OrderBy(item => item.Label)).ToArray();
    public static readonly LayoutElement[] TrainingElements = StandardTrainingElements.Concat(DefDatabase<TrainableDef>.AllDefs.Where(def => !StandardTrainingDefs.Contains(def.defName)).Select(def => new LayoutElement(LayoutElementType.DefWidget, TrainingTypeName, def)).OrderBy(item => item.Label)).ToArray();
    public static readonly LayoutElement[] SkillAndTrainingElements = SkillElements.Concat(TrainingElements).ToArray();

    public static readonly LayoutElement[] ThirdPartyElements = IntegrationManager.ThirdPartyWidgets.Select(item => new LayoutElement(LayoutElementType.ThirdPartyWidget, item.Key, label: item.Value.Item1)).ToArray();

    public static bool AllowXenotypeButton(Pawn pawn) => ModsConfig.BiotechActive && pawn.genes?.Xenotype != null;
    public static bool AllowFactionIconButton(Pawn pawn) => Theme.ShowFactionIcon.Value && pawn.Faction != null;
    public static bool AllowIdeoButton(Pawn pawn) => Theme.ShowIdeoligionIcon.Value && ModsConfig.IdeologyActive && pawn.Ideo != null;
    public static bool AllowResponseButton(Pawn pawn) => pawn.playerSettings.UsesConfigurableHostilityResponse;
    public static bool AllowMedicalButton(Pawn pawn) => pawn.RaceProps?.IsFlesh ?? true;
    public static bool AllowRenameButton(Pawn pawn) => !AllowMedicalButton(pawn);
    public static bool AllowSelfTendButton(Pawn pawn) => pawn.IsColonist;
  }
}
