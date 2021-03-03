using System;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal struct FoodModel : ISelectorModel
    {
        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public Color? Color { get; }
        public TipSignal? Tooltip { get; }

        public Action OnHover { get; }
        public Action OnClick { get; }

        public FoodModel(PawnModel model) : this()
        {
            Model = model;

            if (!model.IsPlayerManaged || model.Base.foodRestriction == null || !model.Base.foodRestriction.Configurable)
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.FoodFormat", model.Base.foodRestriction.CurrentFoodRestriction?.label);

            OnClick = DrawFloatMenu;
        }

        private void DrawFloatMenu()
        {
            var model = Model;

            var options = (from food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions select new FloatMenuOption(food.label, () => Mod_Multiplayer.SetFoodRestriction(model.Base, food))).ToList();
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(model.Base.foodRestriction.CurrentFoodRestriction))));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
