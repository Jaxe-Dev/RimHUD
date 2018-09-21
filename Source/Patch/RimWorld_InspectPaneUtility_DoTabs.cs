using Harmony;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(InspectPaneUtility), "DoTabs")]
    internal static class RimWorld_InspectPaneUtility_DoTabs
    {
        private static bool Prefix(IInspectPane pane)
        {
            if (!State.AltInspectPane || !State.PawnSelected) { return true; }
            return InspectPanePlus.DrawTabs(pane);
        }
    }
}
