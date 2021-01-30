using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Data.Configuration;
using RimHUD.Interface;
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

        private static Dictionary<string, Func<PawnModel, HudWidget>> StandardElementComponents { get; } = new Dictionary<string, Func<PawnModel, HudWidget>>
        {
            { HudBlank.Name, _ => HudBlank.GetEmpty },
            { HudSeparator.Name, _ => HudSeparator.Get() },

            { "NameHeader", model => HudValue.FromText(model.Name, model.BioTooltip, Theme.LargeTextStyle, InspectPanePlus.ToggleSocialTab) },

            { "Outfit", model => HudSelector.FromModel(model.OutfitSelector, Theme.SmallTextStyle) },
            { "Food", model => HudSelector.FromModel(model.FoodSelector, Theme.SmallTextStyle) },
            { "Rules", model => HudSelector.FromModel(model.RulesSelector, Theme.SmallTextStyle) },
            { "Timetable", model => HudSelector.FromModel(model.TimetableSelector, Theme.SmallTextStyle) },
            { "Area", model => HudSelector.FromModel(model.AreaSelector, Theme.SmallTextStyle) },

            { "RelationKindAndFaction", model => HudValue.FromTextModel(model.RelationKindAndFaction, Theme.SmallTextStyle) },
            { "GenderAndAge", model => HudValue.FromTextModel(model.GenderAndAge, Theme.SmallTextStyle) },

            { "Health", model => HudBar.FromModel(model.Health.Bar, Theme.RegularTextStyle) },
            { "HealthCondition", model => HudValue.FromTextModel(model.Health.Condition, Theme.SmallTextStyle) },
            { "MindCondition", model => HudValue.FromTextModel(model.Mind.Condition, Theme.SmallTextStyle) },

            { "Master", model => HudValue.FromTextModel(model.Master, Theme.RegularTextStyle) },

            { "Activity", model => HudValue.FromText(model.Activity, null, Theme.SmallTextStyle) },
            { "Queued", model => HudValue.FromText(model.Queued, null, Theme.SmallTextStyle) },
            { "Equipped", model => HudValue.FromText(model.Equipped, null, Theme.SmallTextStyle, InspectPanePlus.ToggleGearTab) },
            { "Carrying", model => HudValue.FromText(model.Carrying, null, Theme.SmallTextStyle, InspectPanePlus.ToggleGearTab) },
            { "CompInfo", model => HudValue.FromText(model.CompInfo, null, Theme.SmallTextStyle) }
        };

        private static Dictionary<string, Func<PawnModel, HudWidget>> Widgets { get; } = new Dictionary<string, Func<PawnModel, HudWidget>>(StandardElementComponents)
        {
            { "NeedMood", model => HudBar.FromModel(model.Mood, Theme.RegularTextStyle) },
            { "NeedFood", model => HudBar.FromModel(model.Food, Theme.RegularTextStyle) },
            { "NeedRest", model => HudBar.FromModel(model.Rest, Theme.RegularTextStyle) },
            { "NeedRecreation", model => HudBar.FromModel(model.Recreation, Theme.RegularTextStyle) },

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
            new LayoutItem(LayoutItemType.Element, "NeedRest")
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
            "Rest"
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

        public static HudWidget GetWidget(PawnModel model, string id, string defName)
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

        private static HudWidget GetStatWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<StatDef>.GetNamed(defName, false);
            if (def != null)
            {
                if (def.Worker?.IsDisabledFor(model.Base) ?? true) { return HudBlank.GetEmpty; }
                var text = $"{def.LabelCap}: {def.ValueToString(model.Base.GetStatValue(def))}";
                return (HudWidget) HudValue.FromText(text, null, Theme.RegularTextStyle) ?? HudBlank.GetEmpty;
            }

            Mod.Warning($"Invalid HUD Widget, Stat def '{defName}' not found, resetting layout to default");
            RequiredReset();
            return HudBlank.GetEmpty;
        }

        private static HudWidget GetRecordWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<RecordDef>.GetNamed(defName, false);
            if (def != null)
            {
                var text = $"{def.LabelCap}: {(def.type == RecordType.Time ? model.Base.records.GetAsInt(def).ToStringTicksToPeriod() : model.Base.records.GetValue(def).ToString("0.##"))}";
                return (HudWidget) HudValue.FromText(text, null, Theme.RegularTextStyle) ?? HudBlank.GetEmpty;
            }

            Mod.Warning($"Invalid HUD Widget, Record def '{defName}' not found, resetting layout to default");
            RequiredReset();
            return HudBlank.GetEmpty;
        }

        private static HudWidget GetNeedWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<NeedDef>.GetNamed(defName, false);
            if (def != null) { return (HudWidget) HudBar.FromModel(new NeedModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

            Mod.Warning($"Invalid HUD Widget, Need def '{defName}' not found, resetting layout to default");
            RequiredReset();
            return HudBlank.GetEmpty;
        }

        private static HudWidget GetSkillWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<SkillDef>.GetNamed(defName, false);
            if (def != null) { return (HudWidget) HudValue.FromValueModel(new SkillModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

            Mod.Warning($"Invalid HUD Widget, Skill def '{defName}' not found, resetting layout to default");
            RequiredReset();
            return HudBlank.GetEmpty;
        }

        private static HudWidget GetTrainingWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<TrainableDef>.GetNamed(defName, false);
            if (def != null) { return (HudWidget) HudValue.FromValueModel(new TrainingModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

            Mod.Warning($"Invalid HUD Widget, Trainable def '{defName}' not found, resetting layout to default");
            RequiredReset();
            return HudBlank.GetEmpty;
        }

        private static void RequiredReset()
        {
            Dialog_Alert.Open(Lang.Get("Alert.InvalidLayout"));
            HudLayout.LoadDefaultAndSave();
        }
    }
}
