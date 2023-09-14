using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "DoTabs")]
  public static class RimWorld_InspectPaneUtility_DoTabs
  {
    private static bool Prefix(IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      InspectPaneTabs.Draw(pane);

      return false;
    }
  }
}
