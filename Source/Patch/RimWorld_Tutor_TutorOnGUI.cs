using System;
using Harmony;
using RimHUD.Interface;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(Tutor), "TutorOnGUI", new Type[] { })]
    internal static class RimWorld_Tutor_TutorOnGUI
    {
        private static bool Prefix() => !Hud.Visible;
    }
}
