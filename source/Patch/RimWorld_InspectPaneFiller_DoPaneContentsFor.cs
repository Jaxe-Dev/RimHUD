using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsFor")]
  public static class RimWorld_InspectPaneFiller_DoPaneContentsFor
  {
    public static bool Prefix(ISelectable sel, Rect rect)
    {
      if (!State.ModifyPane || !(sel is Pawn pawn)) { return true; }

      InspectPanePlus.DrawContent(rect, null, pawn);

      return false;
    }
  }
}
