using System.Collections.Generic;
using System.Linq;
using RimHUD.Access;
using RimHUD.Integration.PawnRules;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models.Bars;
using RimHUD.Interface.Hud.Models.Selectors;
using RimHUD.Interface.Hud.Models.Values;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud
{
  public static class HudContent
  {
    public const string NeedDefType = "Need";
    public const string SkillDefType = "Skill";
    public const string TrainableDefType = "Trainable";
    public const string StatDefType = "Stat";
    public const string RecordDefType = "Record";

    public const string ExternalWidgetType = "CustomWidget";
    public const string ExternalValueType = "CustomValue";
    public const string ExternalBarType = "CustomBar";
    public const string ExternalSelectorType = "CustomSelector";
    public const string ExternalNeedType = "CustomNeed";

    private static readonly HudWidget[] CommonWidgets =
    {
      HudWidget.Blank(),
      HudWidget.Separator(),

      HudWidget.FromModel<NameHeaderValue>("NameHeader"),

      HudWidget.FromModel<OutfitSelector>("Outfit"),
      HudWidget.FromEitherModel<Mod_PawnRules.RulesSelector, FoodSelector>("Food", Mod_PawnRules.IsAvailable),
      HudWidget.FromModel<TimetableSelector>("Timetable"),
      HudWidget.FromModel<AreaSelector>("Area"),

      HudWidget.FromModel<RelationKindAndFactionValue>("RelationKindAndFaction"),
      HudWidget.FromModel<GenderRaceAndAgeValue>("GenderRaceAndAge"),

      HudWidget.FromModel<HealthConditionValue>("HealthCondition"),
      HudWidget.FromModel<MentalConditionValue>("MentalCondition"),

      HudWidget.FromModel<AnimalMasterValue>("Master"),

      HudWidget.FromModel<ActivityValue>("Activity"),
      HudWidget.FromModel<QueuedValue>("Queued"),
      HudWidget.FromModel<EquippedValue>("Equipped"),
      HudWidget.FromModel<CarryingValue>("Carrying"),
      HudWidget.FromModel<CompInfoValue>("CompInfo"),
      HudWidget.FromModel<PrisonerInfoValue>("PrisonerInfo")
    };

    private static readonly HudWidget[] BarWidgets =
    {
      HudWidget.FromModel<HealthBar>("Health"),

      HudWidget.FromModel<NeedMoodBar>("NeedMood", Defs.NeedMood),
      HudWidget.FromModel<NeedFoodBar>("NeedFood", NeedDefOf.Food),
      HudWidget.FromModel<NeedSleepBar>("NeedSleep", NeedDefOf.Rest),
      HudWidget.FromModel<NeedEnergyBar>("NeedEnergy", Defs.NeedEnergy),
      HudWidget.FromModel<NeedRecreationBar>("NeedRecreation", NeedDefOf.Joy),
      HudWidget.FromModel<NeedSuppressionBar>("NeedSuppression", Defs.NeedSuppression)
    };

    private static readonly HudWidget[] ValueWidgets =
    {
      HudWidget.FromModel<SkillShootingValue>("SkillShooting", SkillDefOf.Shooting),
      HudWidget.FromModel<SkillMeleeValue>("SkillMelee", SkillDefOf.Melee),
      HudWidget.FromModel<SkillConstructionValue>("SkillConstruction", SkillDefOf.Construction),
      HudWidget.FromModel<SkillMiningValue>("SkillMining", SkillDefOf.Mining),
      HudWidget.FromModel<SkillCookingValue>("SkillCooking", SkillDefOf.Cooking),
      HudWidget.FromModel<SkillPlantsValue>("SkillPlants", SkillDefOf.Plants),
      HudWidget.FromModel<SkillAnimalsValue>("SkillAnimals", SkillDefOf.Animals),
      HudWidget.FromModel<SkillCraftingValue>("SkillCrafting", SkillDefOf.Crafting),
      HudWidget.FromSkillModel("SkillArtistic", SkillDefOf.Artistic),
      HudWidget.FromModel<SkillMedicineValue>("SkillMedicine", SkillDefOf.Medicine),
      HudWidget.FromModel<SkillSocialValue>("SkillSocial", SkillDefOf.Social),
      HudWidget.FromModel<SkillIntellectualValue>("SkillIntellectual", SkillDefOf.Intellectual),

      HudWidget.FromTrainableModel("TrainingTameness", TrainableDefOf.Tameness),
      HudWidget.FromTrainableModel("TrainingObedience", TrainableDefOf.Obedience),
      HudWidget.FromTrainableModel("TrainingRelease", TrainableDefOf.Release),
      HudWidget.FromTrainableModel("TrainingRescue", Defs.TrainableRescue),
      HudWidget.FromTrainableModel("TrainingHaul", Defs.TrainableHaul)
    };

    private static readonly HudWidget[] DefWidgets =
    {
      HudWidget.FromNeedDef(),
      HudWidget.FromSkillDef(),
      HudWidget.FromTrainableDef(),
      HudWidget.FromStatDef(),
      HudWidget.FromRecordDef()
    };

    private static readonly HudWidget[] IntegratedWidgets =
    {
      HudWidget.FromCustomWidgetDef(),
      HudWidget.FromCustomValueDef(),
      HudWidget.FromCustomBarDef(),
      HudWidget.FromCustomSelectorDef(),
      HudWidget.FromCustomNeedDef()
    };

    private static readonly Dictionary<string, HudWidget> Widgets = CommonWidgets.Concat(BarWidgets).Concat(ValueWidgets).Concat(DefWidgets).Concat(IntegratedWidgets).ToDictionary(static entry => entry.Id, static entry => entry);

    public static readonly LayoutElement[] CommonElements = BuildElements(CommonWidgets).ToArray();
    public static readonly LayoutElement[] BarElements = BuildElements(BarWidgets).Concat(BuildElements<NeedDef>(NeedDefType)).ToArray();
    public static readonly LayoutElement[] SkillAndTrainableElements = BuildElements(ValueWidgets).Concat(BuildElements<SkillDef>(SkillDefType)).Concat(BuildElements<TrainableDef>(TrainableDefType)).ToArray();
    public static readonly LayoutElement[] StatElements = BuildElements<StatDef>(StatDefType).ToArray();
    public static readonly LayoutElement[] RecordElements = BuildElements<RecordDef>(RecordDefType).ToArray();

    public static readonly LayoutElement[] ExternalElements = BuildExternalElements<CustomWidgetDef>(ExternalWidgetType).Concat(BuildExternalElements<CustomValueDef>(ExternalValueType)).Concat(BuildExternalElements<CustomBarDef>(ExternalBarType)).Concat(BuildExternalElements<CustomNeedDef>(ExternalNeedType)).ToArray();

    public static bool IsValidId(string id, string? defName) => Widgets.TryGetValue(id, out var widget) && widget!.DefNameIsValid(defName);

    public static bool IsExternalType(string id) => id is ExternalWidgetType or ExternalValueType or ExternalBarType or ExternalNeedType;

    public static IWidget GetWidget(string id, HudArgs args) => Widgets.TryGetValue(id, out var widget) && widget!.DefNameIsValid(args.DefName) ? widget.Build(args) : MissingWidget.Get(id, args);

    private static IEnumerable<LayoutElement> BuildElements(IEnumerable<HudWidget> widgets) => widgets.Select(static widget => widget.LayoutElement);
    private static IEnumerable<LayoutElement> BuildElements<T>(string id) where T : Def => DefDatabase<T>.AllDefs.Where(static def => Widgets.Values.All(widget => widget.Def != def)).Select(def => new LayoutElement(LayoutElementType.Widget, id, def));
    private static IEnumerable<LayoutElement> BuildExternalElements<T>(string id) where T : ExternalWidgetDef => DefDatabase<T>.AllDefs.Where(static def => def.Initialized).Select(def => new LayoutElement(LayoutElementType.Widget, id, def));
  }
}
