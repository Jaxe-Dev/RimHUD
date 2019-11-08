using System;
using System.Linq;
using RimHUD.Data.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class OutfitModel : SelectorModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Color? Color { get; }
        public override Action OnHover { get; }

        public OutfitModel(PawnModel model) : base(model)
        {
            if (!model.IsPlayerFaction || (model.Base.outfits?.CurrentOutfit == null))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.OutfitFormat", model.Base.outfits?.CurrentOutfit.label);
            Tooltip = null;
            Color = null;

            OnClick = DrawFloatMenu;
            OnHover = null;
        }

        private void DrawFloatMenu()
        {
            var options = (from outfit in Current.Game.outfitDatabase.AllOutfits select new FloatMenuOption(outfit.label, () => Model.Base.outfits.CurrentOutfit = outfit)).ToList();
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageOutfits(Model.Base.outfits.CurrentOutfit))));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
