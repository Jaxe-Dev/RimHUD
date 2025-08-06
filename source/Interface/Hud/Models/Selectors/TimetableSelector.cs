using System;
using System.Linq;
using RimHUD.Access;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Integration.Multiplayer;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models.Selectors;

public sealed class TimetableSelector : SelectorModel
{
  protected override string? Label { get; }

  protected override Action? OnClick { get; }

  protected override Color? Color { get; }

  public TimetableSelector()
  {
    if (!Active.Pawn.IsPlayerFaction() || Active.Pawn.timetable?.CurrentAssignment is null) { return; }

    Label = Lang.Get("Model.Selector.TimetableFormat").WithValue(Active.Pawn.timetable.CurrentAssignment.LabelCap);

    var assignment = Active.Pawn.timetable.CurrentAssignment;
    Color = assignment == TimeAssignmentDefOf.Anything ? null : assignment.color;

    OnClick = DrawFloatMenu;
  }

  private static void DrawFloatMenu()
  {
    var hour = GenLocalDate.HourOfDay(Active.Pawn);
    var options = DefDatabase<TimeAssignmentDef>.AllDefs.Select(timeAssignment => new FloatMenuOption(Lang.Get("Model.Selector.SetTimeAssignment", hour, timeAssignment.LabelCap), () => Mod_Multiplayer.SetAssignment(Active.Pawn, hour, timeAssignment))).ToList();
    options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), static () => Find.MainTabsRoot!.SetCurrentTab(Defs.MainButtonRestrict)));

    Find.WindowStack!.Add(new FloatMenu(options));
  }
}
