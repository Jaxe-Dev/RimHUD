using System;
using System.Linq;
using RimHUD.Data.Compatibility;
using RimHUD.Data.Extensions;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class TimetableModel : SelectorModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Color? Color { get; }
        public override Action OnHover { get; }

        public TimetableModel(PawnModel model) : base(model)
        {
            if ((!model.Base.Faction?.IsPlayer ?? true) || (model.Base.timetable == null))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.TimetableFormat", model.Base.timetable.CurrentAssignment.LabelCap);
            Tooltip = null;
            var assignment = model.Base.timetable.CurrentAssignment;
            Color = assignment == TimeAssignmentDefOf.Anything ? (Color?) null : assignment.color;

            OnClick = DrawFloatMenu;
            OnHover = null;
        }

        private void DrawFloatMenu()
        {
            var hour = GenLocalDate.HourOfDay(Model.Base);
            var options = DefDatabase<TimeAssignmentDef>.AllDefs.Select(timeAssignment => new FloatMenuOption(Lang.Get("Model.Selector.SetTimeAssignment", hour, timeAssignment.LabelCap), () => MultiplayerCompatibility.SetAssignment(Model.Base, hour, timeAssignment))).ToList();
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.MainTabsRoot.SetCurrentTab(Access.MainButtonDefOfRestrict)));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
