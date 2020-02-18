using HarmonyLib;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(InspectPaneUtility), "InspectPaneOnGUI")]
    internal static class RimWorld_InspectPaneUtility_InspectPaneOnGUI
    {
        private static bool Prefix(Rect inRect, IInspectPane pane)
        {
            if (!State.AltInspectPane || !State.PawnSelected) { return true; }

            InspectPanePlus.OnGUI(inRect, pane);
            return false;
        }
    }
}
