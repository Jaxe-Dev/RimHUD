using System;
using System.Linq;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class OutfitModel : ButtonModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Texture2D Texture { get; }
        public override Action OnClick { get; }
        public override Action OnHover { get; }

        public OutfitModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.outfits?.CurrentOutfit == null))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("InspectPane.OutfitFormat", model.Base.outfits.CurrentOutfit.label);
            Tooltip = null;
            Texture = Textures.InspectPaneButtonGreyTex;

            OnClick = DrawFloatMenu;
            OnHover = null;
        }

        private void DrawFloatMenu()
        {
            var options = (from outfit in Current.Game.outfitDatabase.AllOutfits select new FloatMenuOption(outfit.label, () => Model.Base.outfits.CurrentOutfit = outfit)).ToList();
            options.Add(new FloatMenuOption(Lang.Get("InspectPane.ManageSelector").Italic(), () => Find.WindowStack.Add(new Dialog_ManageOutfits(Model.Base.outfits.CurrentOutfit))));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
