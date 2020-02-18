using HarmonyLib;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(InspectPaneUtility), "PaneSizeFor")]
    internal static class RimWorld_InspectPaneUtility_PaneSizeFor
    {
        private static bool Prefix(ref Vector2 __result, IInspectPane pane)
        {
            if (!State.AltInspectPane || !State.PawnSelected) { return true; }

            __result = new Vector2(InspectPaneUtility.PaneWidthFor(pane), Theme.InspectPaneHeight.Value - 35f);
            return false;
        }
    }
}
