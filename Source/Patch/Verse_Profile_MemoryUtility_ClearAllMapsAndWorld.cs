using Harmony;
using RimHUD.Interface;
using Verse.Profile;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
    internal static class Verse_Profile_MemoryUtility_ClearAllMapsAndWorld
    {
        private static void Prefix() => InspectPanePlus.ClearCache();
    }
}
