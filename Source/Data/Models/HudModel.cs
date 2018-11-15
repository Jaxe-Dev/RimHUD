using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string CustomNeedTypeName = "Need";
        public const string CustomSkillTypeName = "Skill";

        private static readonly Dictionary<string, Func<PawnModel, HudWidget>> Widgets = new Dictionary<string, Func<PawnModel, HudWidget>>
        {
            { HudBlank.Name, _ => HudBlank.GetEmpty },
            { HudSeparator.Name, _ => HudSeparator.Get() },

            { "NameHeader", model => HudValue.FromText(model.Name, model.BioTooltip, Theme.LargeTextStyle) },

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
            { "TrainingHaul", model => HudValue.FromValueModel(model.Haul, Theme.RegularTextStyle) },

            { "Master", model => HudValue.FromTextModel(model.Master, Theme.RegularTextStyle) },

            { "Activity", model => HudValue.FromText(model.Activity, null, Theme.SmallTextStyle) },
            { "Queued", model => HudValue.FromText(model.Queued, null, Theme.SmallTextStyle) },
            { "Equipped", model => HudValue.FromText(model.Equipped, null, Theme.SmallTextStyle) },
            { "Carrying", model => HudValue.FromText(model.Carrying, null, Theme.SmallTextStyle) },
            { "CompInfo", model => HudValue.FromText(model.CompInfo, null, Theme.SmallTextStyle) }
        };

        private static readonly string[] StandardNeeds =
        {
            "Food",
            "Joy",
            "Mood",
            "Rest"
        };

        private static readonly string[] StandardSkills =
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

        public static readonly LayoutItem[] StackComponents =
        {
            new LayoutItem(LayoutItemType.Stack, HudVStack.Name),
            new LayoutItem(LayoutItemType.Stack, HudHStack.Name)
        };
        public static readonly LayoutItem PanelComponent = new LayoutItem(LayoutItemType.Panel, HudPanel.Name);
        public static readonly LayoutItem RowComponent = new LayoutItem(LayoutItemType.Row, HudRow.Name);
        public static readonly LayoutItem[] ElementComponents = Widgets.Select(widget => new LayoutItem(LayoutItemType.Element, widget.Key)).ToArray();
        public static readonly LayoutItem[] StatComponents = DefDatabase<StatDef>.AllDefs.Select(stat => new LayoutItem(LayoutItemType.Element, StatTypeName, stat)).ToArray();
        public static readonly LayoutItem[] RecordComponents = DefDatabase<RecordDef>.AllDefs.Select(record => new LayoutItem(LayoutItemType.Element, RecordTypeName, record)).ToArray();
        public static readonly LayoutItem[] CustomNeedComponents = DefDatabase<NeedDef>.AllDefs.Where(skill => !StandardNeeds.Contains(skill.defName)).Select(need => new LayoutItem(LayoutItemType.CustomElement, CustomNeedTypeName, need)).ToArray();
        public static readonly LayoutItem[] CustomSkillComponents = DefDatabase<SkillDef>.AllDefs.Where(skill => !StandardSkills.Contains(skill.defName)).Select(skill => new LayoutItem(LayoutItemType.CustomElement, CustomSkillTypeName, skill)).ToArray();

        public static bool IsValidType(string id) => Widgets.ContainsKey(id) || (id == StatTypeName) || (id == RecordTypeName) || (id == CustomNeedTypeName) || (id == CustomSkillTypeName);

        public static HudWidget GetWidget(PawnModel model, string id, string defName)
        {
            if (id == "Stat") { return GetStatWidget(model, defName); }
            if (id == "Record") { return GetRecordWidget(model, defName); }
            if (id == "Need") { return GetNeedWidget(model, defName); }
            if (id == "Skill") { return GetSkillWidget(model, defName); }

            var widget = Widgets.TryGetValue(id);
            if (widget == null) { throw new Mod.Exception($"Invalid HUD Widget, type '{id}' is not recognized"); }

            return widget.Invoke(model) ?? HudBlank.GetEmpty;
        }

        private static HudWidget GetStatWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<StatDef>.GetNamed(defName, false);
            if (def != null)
            {
                if (def.Worker.IsDisabledFor(model.Base)) { return HudBlank.GetEmpty; }
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

        private static void RequiredReset()
        {
            Dialog_Alert.Open(Lang.Get("Alert.InvalidLayout"));
            HudLayout.LoadDefaultAndSave();
        }
    }
}
