using HarmonyLib;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MainTabWindow_Inspect), "PaneTopY", MethodType.Getter)]
    internal static class RimWorld_MainTabWindow_Inspect_PaneTopY
    {
        public static bool Prefix(ref float __result)
        {
            if (!State.ModifyPane) { return true; }

            __result = (float) UI.screenHeight - Theme.InspectPaneHeight.Value;

            return false;
        }
    }
}
