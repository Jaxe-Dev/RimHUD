using System;
using System.Linq;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class FoodModel : ButtonModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Texture2D Texture { get; }
        public override Action OnClick { get; }
        public override Action OnHover { get; }

        public FoodModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.foodRestriction?.CurrentFoodRestriction == null))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("InspectPane.FoodFormat", model.Base.foodRestriction.CurrentFoodRestriction.label);
            Tooltip = null;
            Texture = Textures.InspectPaneButtonGreyTex;

            OnClick = DrawFloatMenu;
            OnHover = null;
        }

        private void DrawFloatMenu()
        {
            var options = (from food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions select new FloatMenuOption(food.label, () => Model.Base.foodRestriction.CurrentFoodRestriction = food)).ToList();
            options.Add(new FloatMenuOption(Lang.Get("InspectPane.ManageSelector").Italic(), () => Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(Model.Base.foodRestriction.CurrentFoodRestriction))));
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
