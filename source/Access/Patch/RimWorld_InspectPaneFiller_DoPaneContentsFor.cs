using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface.Screen;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsFor")]
  public static class RimWorld_InspectPaneFiller_DoPaneContentsFor
  {
    public static bool Prefix(ISelectable sel, Rect rect)
    {
      if (!State.ModifyPane || sel is not Pawn) { return true; }

      InspectPanePlus.DrawContent(rect);

      return false;
    }
  }
}
