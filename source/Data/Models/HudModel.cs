using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.HUD;
using RimWorld;
using Verse;

namespace RimHUD.Data.Models
{
  internal static class HudModel
  {
    public const string StatTypeName = "Stat";
    public const string RecordTypeName = "Record";
    public const string NeedTypeName = "Need";
    public const string SkillTypeName = "Skill";
    public const string TrainingTypeName = "Training";

    public static readonly LayoutItem[] StackComponents =
    {
      new LayoutItem(LayoutItemType.Stack, HudVStack.Name),
      new LayoutItem(LayoutItemType.Stack, HudHStack.Name)
    };

    private static Dictionary<string, Func<PawnModel, HudWidgetBase>> StandardElementComponents { get; } = new Dictionary<string, Func<PawnModel, HudWidgetBase>>
    {
      { HudBlank.Name, _ => HudBlank.GetEmpty },
      { HudSeparator.Name, _ => HudSeparator.Get() },

      { "NameHeader", model => HudValue.FromTextModel(model.Name, Theme.LargeTextStyle) },

      { "Outfit", model => HudSelector.FromSelectorModel(model.OutfitSelector, Theme.SmallTextStyle) },
      { "Food", model => HudSelector.FromSelectorModel(model.FoodSelector, Theme.SmallTextStyle) },
      { "Timetable", model => HudSelector.FromSelectorModel(model.TimetableSelector, Theme.SmallTextStyle) },
      { "Area", model => HudSelector.FromSelectorModel(model.AreaSelector, Theme.SmallTextStyle) },

      { "RelationKindAndFaction", model => HudValue.FromTextModel(model.RelationKindFaction, Theme.SmallTextStyle) },
      { "GenderRaceAndAge", model => HudValue.FromTextModel(model.GenderRaceAndAge, Theme.SmallTextStyle) },
      { "SimpleGenderRaceAndAge", model => HudValue.FromTextModel(model.SimpleGenderRaceAndAge, Theme.SmallTextStyle) },

      { "Health", model => HudBar.FromModel(model.Health.Bar, Theme.RegularTextStyle) },
      { "HealthCondition", model => HudValue.FromTextModel(model.Health.Condition, Theme.SmallTextStyle) },
      { "MindCondition", model => HudValue.FromTextModel(model.Mind.Condition, Theme.SmallTextStyle) },

      { "Master", model => HudValue.FromTextModel(model.Master, Theme.RegularTextStyle) },

      { "Activity", model => HudValue.FromTextModel(model.Activity, Theme.SmallTextStyle) },
      { "Queued", model => HudValue.FromTextModel(model.Queued, Theme.SmallTextStyle) },
      { "Equipped", model => HudValue.FromTextModel(model.Equipped, Theme.SmallTextStyle) },
      { "Carrying", model => HudValue.FromTextModel(model.Carrying, Theme.SmallTextStyle) },
      { "CompInfo", model => HudValue.FromTextModel(model.CompInfo, Theme.SmallTextStyle) },
      { "PrisonerInfo", model => HudValue.FromTextModel(model.PrisonerInfo, Theme.RegularTextStyle) }
    };

    private static Dictionary<string, Func<PawnModel, HudWidgetBase>> Widgets { get; } = new Dictionary<string, Func<PawnModel, HudWidgetBase>>(StandardElementComponents)
    {
      { "NeedMood", model => HudBar.FromModel(model.Mood, Theme.RegularTextStyle) },
      { "NeedFood", model => HudBar.FromModel(model.Food, Theme.RegularTextStyle) },
      { "NeedRest", model => HudBar.FromModel(model.Rest, Theme.RegularTextStyle) },
      { "NeedEnergy", model => HudBar.FromModel(model.Energy, Theme.RegularTextStyle) },
      { "NeedRecreation", model => HudBar.FromModel(model.Recreation, Theme.RegularTextStyle) },
      { "NeedSuppression", model => HudBar.FromModel(model.Suppression, Theme.RegularTextStyle) },

      { "SkillShooting", model => HudValue.FromValueModel(model.Shooting, Theme.RegularTextStyle) },
      { "SkillMelee", model => HudValue.FromValueModel(model.Melee, Theme.RegularTextStyle) },
      { "SkillConstruction", model => HudValue.FromValueModel(model.Construction, Theme.RegularTextStyle) },
      { "SkillMining", model => HudValue.FromValueModel(model.Mining, Theme.RegularTextStyle) },
      { "SkillCooking", model => HudValue.FromValueModel(model.Cooking, Theme.RegularTextStyle) },
      { "SkillPlants", model => HudValue.FromValueModel(model.Plants, Theme.RegularTextStyle) },
      { "SkillAnimals", model => HudValue.FromValueModel(model.Animals, Theme.RegularTextStyle) },
      { "SkillCrafting", model => HudValue.FromValueModel(model.Crafting, Theme.RegularTextStyle) },
      { "SkillArtistic", model => HudValue.FromValueModel(model.Artistic, Theme.RegularTextStyle) },
      { "SkillMedicine", model => HudValue.FromValueModel(model.Medicine, Theme.RegularTextStyle) },
      { "SkillSocial", model => HudValue.FromValueModel(model.Social, Theme.RegularTextStyle) },
      { "SkillIntellectual", model => HudValue.FromValueModel(model.Intellectual, Theme.RegularTextStyle) },

      { "TrainingTameness", model => HudValue.FromValueModel(model.Tameness, Theme.RegularTextStyle) },
      { "TrainingObedience", model => HudValue.FromValueModel(model.Obedience, Theme.RegularTextStyle) },
      { "TrainingRelease", model => HudValue.FromValueModel(model.Release, Theme.RegularTextStyle) },
      { "TrainingRescue", model => HudValue.FromValueModel(model.Rescue, Theme.RegularTextStyle) },
      { "TrainingHaul", model => HudValue.FromValueModel(model.Haul, Theme.RegularTextStyle) }
    };

    private static readonly LayoutItem[] StandardNeedComponents =
    {
      new LayoutItem(LayoutItemType.Element, "NeedFood"),
      new LayoutItem(LayoutItemType.Element, "NeedRecreation"),
      new LayoutItem(LayoutItemType.Element, "NeedMood"),
      new LayoutItem(LayoutItemType.Element, "NeedRest"),
      new LayoutItem(LayoutItemType.Element, "NeedSuppression")
    };

    private static readonly LayoutItem[] StandardSkillComponents =
    {
      new LayoutItem(LayoutItemType.Element, "SkillShooting"),
      new LayoutItem(LayoutItemType.Element, "SkillMelee"),
      new LayoutItem(LayoutItemType.Element, "SkillConstruction"),
      new LayoutItem(LayoutItemType.Element, "SkillMining"),
      new LayoutItem(LayoutItemType.Element, "SkillCooking"),
      new LayoutItem(LayoutItemType.Element, "SkillPlants"),
      new LayoutItem(LayoutItemType.Element, "SkillAnimals"),
      new LayoutItem(LayoutItemType.Element, "SkillCrafting"),
      new LayoutItem(LayoutItemType.Element, "SkillArtistic"),
      new LayoutItem(LayoutItemType.Element, "SkillMedicine"),
      new LayoutItem(LayoutItemType.Element, "SkillSocial"),
      new LayoutItem(LayoutItemType.Element, "SkillIntellectual")
    };

    private static readonly LayoutItem[] StandardTrainingComponents =
    {
      new LayoutItem(LayoutItemType.Element, "TrainingTameness"),
      new LayoutItem(LayoutItemType.Element, "TrainingObedience"),
      new LayoutItem(LayoutItemType.Element, "TrainingRelease"),
      new LayoutItem(LayoutItemType.Element, "TrainingRescue"),
      new LayoutItem(LayoutItemType.Element, "TrainingHaul")
    };

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

    public static readonly LayoutItem PanelComponent = new LayoutItem(LayoutItemType.Panel, HudPanel.Name);
    public static readonly LayoutItem RowComponent = new LayoutItem(LayoutItemType.Row, HudRow.Name);
    public static readonly LayoutItem[] ElementComponents = StandardElementComponents.Select(widget => new LayoutItem(LayoutItemType.Element, widget.Key)).ToArray();
    public static readonly LayoutItem[] StatComponents = DefDatabase<StatDef>.AllDefs.Select(stat => new LayoutItem(LayoutItemType.Element, StatTypeName, stat)).OrderBy(item => item.Label).ToArray();
    public static readonly LayoutItem[] RecordComponents = DefDatabase<RecordDef>.AllDefs.Select(record => new LayoutItem(LayoutItemType.Element, RecordTypeName, record)).OrderBy(item => item.Label).ToArray();

    public static readonly LayoutItem[] NeedComponents = StandardNeedComponents.Concat(DefDatabase<NeedDef>.AllDefs.Where(def => !StandardNeedDefs.Contains(def.defName)).Select(def => new LayoutItem(LayoutItemType.CustomElement, NeedTypeName, def)).OrderBy(item => item.Label)).ToArray();
    public static readonly LayoutItem[] SkillComponents = StandardSkillComponents.Concat(DefDatabase<SkillDef>.AllDefs.Where(def => !StandardSkillDefs.Contains(def.defName)).Select(def => new LayoutItem(LayoutItemType.CustomElement, SkillTypeName, def)).OrderBy(item => item.Label)).ToArray();
    public static readonly LayoutItem[] TrainingComponents = StandardTrainingComponents.Concat(DefDatabase<TrainableDef>.AllDefs.Where(def => !StandardTrainingDefs.Contains(def.defName)).Select(def => new LayoutItem(LayoutItemType.CustomElement, TrainingTypeName, def)).OrderBy(item => item.Label)).ToArray();
    public static readonly LayoutItem[] SkillAndTrainingComponents = SkillComponents.Concat(TrainingComponents).ToArray();

    public static bool IsValidType(string id) => Widgets.ContainsKey(id) || id == StatTypeName || id == RecordTypeName || id == NeedTypeName || id == SkillTypeName || id == TrainingTypeName;

    public static HudWidgetBase GetWidget(PawnModel model, string id, string defName)
    {
      if (id == "Stat") { return GetStatWidget(model, defName); }
      if (id == "Record") { return GetRecordWidget(model, defName); }
      if (id == "Need") { return GetNeedWidget(model, defName); }
      if (id == "Skill") { return GetSkillWidget(model, defName); }
      if (id == "Training") { return GetTrainingWidget(model, defName); }

      var widget = Widgets.TryGetValue(id);
      if (widget == null) { throw new Mod.Exception($"Invalid HUD Widget, type '{id}' is not recognized"); }

      return widget.Invoke(model) ?? HudBlank.GetEmpty;
    }

    private static HudWidgetBase GetStatWidget(PawnModel model, string defName)
    {
      var def = DefDatabase<StatDef>.GetNamed(defName, false);
      if (def != null)
      {
        try
        {
          if (def.Worker?.IsDisabledFor(model.Base) ?? true) { return HudBlank.GetEmpty; }

          var text = $"{def.GetLabelCap()}: {def.ValueToString(model.Base.GetStatValue(def))}";

          return (HudWidgetBase) HudValue.FromTextModel(TextModel.Create(text), Theme.RegularTextStyle) ?? HudBlank.GetEmpty;
        }
        catch { return HudBlank.GetEmpty; }
      }

      Mod.Warning($"Invalid HUD Widget, Stat def '{defName}' not found, resetting layout to default");
      RequiredReset();

      return HudBlank.GetEmpty;
    }

    private static HudWidgetBase GetRecordWidget(PawnModel model, string defName)
    {
      var def = DefDatabase<RecordDef>.GetNamed(defName, false);
      if (def != null)
      {
        var text = $"{def.LabelCap}: {(def.type == RecordType.Time ? model.Base.records.GetAsInt(def).ToStringTicksToPeriod() : model.Base.records.GetValue(def).ToString("0.##"))}";
        return (HudWidgetBase) HudValue.FromTextModel(TextModel.Create(text), Theme.RegularTextStyle) ?? HudBlank.GetEmpty;
      }

      Mod.Warning($"Invalid HUD Widget, Record def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return HudBlank.GetEmpty;
    }

    private static HudWidgetBase GetNeedWidget(PawnModel model, string defName)
    {
      var def = DefDatabase<NeedDef>.GetNamed(defName, false);
      if (def != null) { return (HudWidgetBase) HudBar.FromModel(new NeedModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Need def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return HudBlank.GetEmpty;
    }

    private static HudWidgetBase GetSkillWidget(PawnModel model, string defName)
    {
      var def = DefDatabase<SkillDef>.GetNamed(defName, false);
      if (def != null) { return (HudWidgetBase) HudValue.FromValueModel(new SkillModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Skill def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return HudBlank.GetEmpty;
    }

    private static HudWidgetBase GetTrainingWidget(PawnModel model, string defName)
    {
      var def = DefDatabase<TrainableDef>.GetNamed(defName, false);
      if (def != null) { return (HudWidgetBase) HudValue.FromValueModel(new TrainingModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Trainable def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return HudBlank.GetEmpty;
    }

    private static void RequiredReset()
    {
      Dialog_Alert.Open(Lang.Get("Alert.InvalidLayout"));
      HudLayout.LoadDefaultAndSave();
    }

    public static void BuildStatString(Pawn pawn, StringBuilder builder, StatDef def) => builder.TryAppendLine(GetStatString(pawn, def));

    public static string GetStatString(Pawn pawn, StatDef def)
    {
      try
      {
        if (def.Worker?.IsDisabledFor(pawn) ?? true) { return null; }

        return $"{def.LabelCap}: {def.ValueToString(pawn.GetStatValue(def))}";
      }
      catch { return null; }
    }
  }
}
