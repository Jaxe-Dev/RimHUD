using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models;

public static class Active
{
  public static Pawn Pawn => State.SelectedPawn ?? throw new Exception("Tried to get active pawn when none selected.");

  public static string Name => Pawn.Name?.ToStringFull.CapitalizeFirst() ?? Pawn.LabelCap;

  public static LayerTarget Target
  {
    get
    {
      if (Pawn.IsPlayerManaged()) { return Pawn.RaceProps!.Humanlike ? LayerTarget.PlayerHumanlike : LayerTarget.PlayerCreature; }
      return Pawn.RaceProps!.Humanlike ? LayerTarget.OtherHumanlike : LayerTarget.OtherCreature;
    }
  }

  public static Color FactionRelationColor
  {
    get
    {
      if (Pawn.Faction is null) { return Pawn.IsHumanlike() ? Theme.FactionIndependentColor.Value : Theme.FactionWildColor.Value; }

      if (Pawn.IsPrisonerOfColony) { return Theme.FactionPrisonerColor.Value; }
      if (Pawn.IsSlaveOfColony) { return Theme.FactionSlaveColor.Value; }

      if (Pawn.Faction.IsPlayer) { return Theme.FactionOwnColor.Value; }

      return Pawn.Faction.PlayerRelationKind switch { FactionRelationKind.Hostile => Theme.FactionHostileColor.Value, FactionRelationKind.Ally => Theme.FactionAlliedColor.Value, _ => Theme.FactionIndependentColor.Value };
    }
  }
}
