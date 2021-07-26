using HarmonyLib;
using RimHUD.Data;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MainTabWindow), "InitialSize", MethodType.Getter)]
    internal static class RimWorld_MainTabWindow_InitialSize
    {
        private static void Prefix(MainTabWindow __instance) => State.ResizePane = State.Active && __instance.GetType() == typeof(MainTabWindow_Inspect) && State.SelectedPawn != null;

        private static void Postfix() => State.ResizePane = false;
    }
}