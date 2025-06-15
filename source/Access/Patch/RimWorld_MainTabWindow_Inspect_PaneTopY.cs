using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using Verse;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(MainTabWindow_Inspect), "PaneTopY", MethodType.Getter)]
public static class RimWorld_MainTabWindow_Inspect_PaneTopY
{
  public static bool Prefix(ref float __result)
  {
    if (!State.ModifyPane) { return true; }

    __result = (float)UI.screenHeight - Theme.InspectPaneHeight.Value;

    return false;
  }
}
