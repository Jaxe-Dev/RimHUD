using Harmony;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(Root), "Start")]
    internal static class Verse_PlayDataLoader_DoPlayLoad
    {
        private static void Postfix() => Mod.OnStartup();
    }
}
