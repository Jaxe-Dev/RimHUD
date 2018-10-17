using System;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class TimetableModel : ButtonModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Texture2D Texture { get; }
        public override Action OnClick { get; }
        public override Action OnHover { get; }

        public TimetableModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.timetable == null))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("InspectPane.TimetableFormat", model.Base.timetable.CurrentAssignment.LabelCap);
            Tooltip = null;
            var assignment = model.Base.timetable.CurrentAssignment;
            Texture = assignment == TimeAssignmentDefOf.Anything ? Textures.InspectPaneButtonGreyTex : assignment.ColorTexture;

            OnClick = () => Find.MainTabsRoot.SetCurrentTab(Access.MainButtonDefOfRestrict);
            OnHover = null;
        }
    }
}
