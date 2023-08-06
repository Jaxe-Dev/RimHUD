using System;
using System.Linq;
using RimHUD.Compatibility.Multiplayer;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class TimetableModel : IModelSelector
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }
    public Func<string> Tooltip => null;

    public Action OnClick { get; }
    public Action OnHover => null;

    public TimetableModel(PawnModel owner)
    {
      try
      {
        Owner = owner;

        if ((!owner.Base.Faction?.IsPlayer ?? true) || owner.Base.timetable == null)
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.TimetableFormat", owner.Base.timetable.CurrentAssignment.LabelCap);

        var assignment = owner.Base.timetable.CurrentAssignment;
        Color = assignment == TimeAssignmentDefOf.Anything ? (Color?)null : assignment.color;

        OnClick = DrawFloatMenu;
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        Hidden = true;
      }
    }

    private void DrawFloatMenu()
    {
      try
      {
        var model = Owner;

        var hour = GenLocalDate.HourOfDay(Owner.Base);
        var options = DefDatabase<TimeAssignmentDef>.AllDefs.Select(timeAssignment => new FloatMenuOption(Lang.Get("Model.Selector.SetTimeAssignment", hour, timeAssignment.LabelCap), () => Mod_Multiplayer.SetAssignment(model.Base, hour, timeAssignment))).ToList();
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.MainTabsRoot.SetCurrentTab(Access.MainButtonDefOfRestrict)));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }
    }
  }
}
