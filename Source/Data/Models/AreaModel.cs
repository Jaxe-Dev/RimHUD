using System;
using System.Linq;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class AreaModel : ButtonModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Texture2D Texture { get; }
        public override Action OnClick { get; }
        public override Action OnHover { get; }

        public AreaModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.playerSettings == null) || (!model.Base.IsColonist && !model.Base.playerSettings.RespectsAllowedArea))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("InspectPane.AreaFormat", AreaUtility.AreaAllowedLabel(model.Base));
            Tooltip = null;
            Texture = model.Base.playerSettings?.EffectiveAreaRestriction == null ? Textures.InspectPaneButtonGreyTex : model.Base.playerSettings.EffectiveAreaRestriction.ColorTexture;

            OnClick = DrawFloatMenu;
            OnHover = () => model.Base.playerSettings.EffectiveAreaRestriction?.MarkForDraw();
        }

        private void DrawFloatMenu()
        {
            var options = (from area in Find.CurrentMap.areaManager.AllAreas.Where(area => area.AssignableAsAllowed()) select new FloatMenuOption(area.Label, () => Model.Base.playerSettings.AreaRestriction = area)).ToList();
            options.Add(new FloatMenuOption(Lang.Get("InspectPane.ManageSelector").Italic(), () => Find.WindowStack.Add(new Dialog_ManageAreas(Find.CurrentMap))));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
