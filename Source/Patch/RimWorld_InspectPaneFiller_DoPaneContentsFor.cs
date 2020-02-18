using HarmonyLib;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsFor")]
    internal static class RimWorld_InspectPaneFiller_DoPaneContentsFor
    {
        public static bool Prefix(ISelectable sel, Rect rect)
        {
            if (!State.AltInspectPane || !(sel is Pawn pawn)) { return true; }

            InspectPanePlus.DrawContent(rect, null, pawn);

            return false;
        }
    }
}
