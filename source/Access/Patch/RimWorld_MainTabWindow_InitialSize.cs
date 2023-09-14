using HarmonyLib;
using RimHUD.Engine;
using RimWorld;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(MainTabWindow), "InitialSize", MethodType.Getter)]
  public static class RimWorld_MainTabWindow_InitialSize
  {
    private static void Prefix(MainTabWindow __instance) => State.ForceModifyPane = State.ModifyPane && __instance is MainTabWindow_Inspect && State.SelectedPawn is not null;

    private static void Postfix() => State.ForceModifyPane = false;
  }
}
