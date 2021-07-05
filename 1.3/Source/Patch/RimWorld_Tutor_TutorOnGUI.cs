using HarmonyLib;
using RimHUD.Data;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(Tutor), "TutorOnGUI")]
    internal static class RimWorld_Tutor_TutorOnGUI
    {
        private static bool Prefix() => !State.HudFloatingVisible;
    }
}
