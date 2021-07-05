using HarmonyLib;
using RimHUD.Interface;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MapInterface), "Notify_SwitchedMap")]
    internal static class Verse_MapInterface_Notify_SwitchedMap
    {
        private static void Postfix() => InspectPanePlus.ClearCache();
    }
}
