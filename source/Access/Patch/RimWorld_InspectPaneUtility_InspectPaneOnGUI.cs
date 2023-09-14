using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface.Screen;
using RimWorld;
using UnityEngine;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "InspectPaneOnGUI")]
  public static class RimWorld_InspectPaneUtility_InspectPaneOnGUI
  {
    private static bool Prefix(Rect inRect, IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      InspectPanePlus.DrawPane(inRect, pane);

      return false;
    }
  }
}
