using Harmony;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(Root), "Start")]
    internal static class Verse_Root_Start
    {
        private static void Postfix() => Mod.OnStartup();
    }
}
