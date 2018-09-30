using Harmony;
using RimHUD.Data;
using RimHUD.Interface.HUD;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MapInterface), "MapInterfaceOnGUI_AfterMainTabs")]
    internal static class Verse_MapInterface_MapInterfaceOnGUI_AfterMainTabs
    {
        private static void Prefix()
        {
            if (!State.HudFloatingVisible) { return; }

            HudFloating.OnGUI();
        }
    }
}
