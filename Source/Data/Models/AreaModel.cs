using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class AreaModel : SelectorModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Color? Color { get; }
        public override Action OnHover { get; }

        public AreaModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.playerSettings == null) || (!model.Base.IsColonist && !model.Base.playerSettings.RespectsAllowedArea))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.AreaFormat", AreaUtility.AreaAllowedLabel(model.Base));
            Tooltip = null;
            Color = model.Base.playerSettings?.EffectiveAreaRestriction?.Color;

            OnClick = DrawFloatMenu;
            OnHover = () => model.Base.playerSettings.EffectiveAreaRestriction?.MarkForDraw();
        }

        private void DrawFloatMenu()
        {
            var options = new List<FloatMenuOption> { new FloatMenuOption("NoAreaAllowed".Translate(), () => Mod_Multiplayer.SetArea(Model.Base, null)) };
            options.AddRange(from area in Find.CurrentMap.areaManager.AllAreas.Where(area => area.AssignableAsAllowed()) select new FloatMenuOption(area.Label, () => Mod_Multiplayer.SetArea(Model.Base, area)));
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageAreas(Find.CurrentMap))));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
