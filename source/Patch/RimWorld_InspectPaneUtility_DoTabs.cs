using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface;
using RimWorld;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "DoTabs")]
  public static class RimWorld_InspectPaneUtility_DoTabs
  {
    private static bool Prefix(IInspectPane pane) => !State.ModifyPane || InspectPanePlus.DrawTabs(pane);
  }
}
