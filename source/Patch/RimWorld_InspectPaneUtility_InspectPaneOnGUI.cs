using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "InspectPaneOnGUI")]
  public static class RimWorld_InspectPaneUtility_InspectPaneOnGUI
  {
    private static bool Prefix(Rect inRect, IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      InspectPanePlus.OnGUI(inRect, pane);
      return false;
    }
  }
}
