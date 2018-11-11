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
        private const string CustomNeedType = "Need";
        private const string CustomSkillType = "Skill";

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

        public static bool IsValidType(string id) => Widgets.ContainsKey(id) || (id == CustomNeedType) || (id == CustomSkillType);

        public static LayoutItem[] GetLayoutItems()
        {
            var list = new List<LayoutItem>
            {
                        new LayoutItem(LayoutItemType.Stack, HudVStack.Name),
                        new LayoutItem(LayoutItemType.Stack, HudHStack.Name),
                        new LayoutItem(LayoutItemType.Panel, HudPanel.Name),
                        new LayoutItem(LayoutItemType.Row, HudRow.Name)
            };
            list.AddRange(Widgets.Select(widget => new LayoutItem(LayoutItemType.Element, widget.Key)));
            list.AddRange(DefDatabase<SkillDef>.AllDefs.Select(skill => new LayoutItem(LayoutItemType.Element, CustomSkillType, skill)));
            list.AddRange(DefDatabase<NeedDef>.AllDefs.Select(need => new LayoutItem(LayoutItemType.Element, CustomSkillType, need)));

            return list.ToArray();
        }

        public static HudWidget GetWidget(PawnModel model, string id, string defName)
        {
            if (id == "Need") { return GetNeedWidget(model, defName); }
            if (id == "Skill") { return GetSkillWidget(model, defName); }

            var widget = Widgets.TryGetValue(id);
            if (widget == null) { throw new Mod.Exception($"Invalid HUD Widget, type '{id}' is not recognized"); }

            return widget.Invoke(model) ?? HudBlank.GetEmpty;
        }

        private static HudWidget GetNeedWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<NeedDef>.GetNamed(defName, false);
            if (def != null) { return (HudWidget) HudBar.FromModel(new NeedModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

            Mod.ErrorOnce($"Invalid HUD Widget, Need def '{defName}' not found", "InvalidNeedDefName" + defName);
            return HudBlank.GetEmpty;
        }

        private static HudWidget GetSkillWidget(PawnModel model, string defName)
        {
            var def = DefDatabase<SkillDef>.GetNamed(defName, false);
            if (def != null) { return (HudWidget) HudValue.FromValueModel(new SkillModel(model, def), Theme.RegularTextStyle) ?? HudBlank.GetEmpty; }

            Mod.ErrorOnce($"Invalid HUD Widget, Skill def '{defName}' not found", "InvalidSkillDefName" + defName);
            return HudBlank.GetEmpty;
        }
    }
}
