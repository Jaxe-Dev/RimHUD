using System;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal struct TimetableModel : ISelectorModel
    {
        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public Color? Color { get; }
        public TipSignal? Tooltip { get; }

        public Action OnClick { get; }
        public Action OnHover { get; }

        public TimetableModel(PawnModel model) : this()
        {
            Model = model;

            if ((!model.Base.Faction?.IsPlayer ?? true) || model.Base.timetable == null)
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Model.Selector.TimetableFormat", model.Base.timetable.CurrentAssignment.LabelCap);

            var assignment = model.Base.timetable.CurrentAssignment;
            Color = assignment == TimeAssignmentDefOf.Anything ? (Color?) null : assignment.color;

            OnClick = DrawFloatMenu;
        }

        private void DrawFloatMenu()
        {
            var model = Model;

            var hour = GenLocalDate.HourOfDay(Model.Base);
            var options = DefDatabase<TimeAssignmentDef>.AllDefs.Select(timeAssignment => new FloatMenuOption(Lang.Get("Model.Selector.SetTimeAssignment", hour, timeAssignment.LabelCap), () => Mod_Multiplayer.SetAssignment(model.Base, hour, timeAssignment))).ToList();
            options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.MainTabsRoot.SetCurrentTab(Access.MainButtonDefOfRestrict)));

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}
