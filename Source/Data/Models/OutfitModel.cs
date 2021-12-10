using System;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class OutfitModel : ISelectorModel
    {
        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public Func<string> Tooltip { get; }
        public Color? Color { get; }

        public Action OnHover { get; }
        public Action OnClick { get; }

        public OutfitModel(PawnModel model)
        {
            try
            {
                Model = model;

                if (!model.IsPlayerFaction || model.Base?.outfits?.CurrentOutfit == null)
                {
                    Hidden = true;
                    return;
                }

                Label = Lang.Get("Model.Selector.OutfitFormat", model.Base.outfits?.CurrentOutfit.label);

                OnClick = DrawFloatMenu;
            }
            catch (Exception exception)
            {
                Mod.HandleWarning(exception);
                Hidden = true;
            }
        }

        private void DrawFloatMenu()
        {
            try
            {
                var model = Model;

                var options = (from outfit in Current.Game.outfitDatabase.AllOutfits select new FloatMenuOption(outfit.label, () => Mod_Multiplayer.SetOutfit(model.Base, outfit))).ToList();
                options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageOutfits(model.Base.outfits.CurrentOutfit))));

                Find.WindowStack.Add(new FloatMenu(options));
            }
            catch (Exception exception) { Mod.HandleError(exception); }
        }
    }
}
