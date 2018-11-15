using System;
using System.Linq;
using RimHUD.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class FoodModel : SelectorModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Color? Color { get; }
        public override Action OnClick { get; }
        public override Action OnHover { get; }

        public FoodModel(PawnModel model) : base(model)
        {
            if (!model.IsPlayerManaged || (model.Base.foodRestriction == null) || !model.Base.foodRestriction.Configurable)
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.FoodFormat", model.Base.foodRestriction.CurrentFoodRestriction.label);
            Tooltip = null;
            Color = null;

            OnClick = DrawFloatMenu;
            OnHover = null;
        }

        private void DrawFloatMenu()
        {
            var options = (from food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions select new FloatMenuOption(food.label, () => Model.Base.foodRestriction.CurrentFoodRestriction = food)).ToList();
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(Model.Base.foodRestriction.CurrentFoodRestriction))));
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
