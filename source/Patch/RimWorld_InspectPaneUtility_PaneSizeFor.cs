using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "PaneSizeFor")]
  public static class RimWorld_InspectPaneUtility_PaneSizeFor
  {
    private static bool Prefix(ref Vector2 __result, IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      __result = new Vector2(InspectPaneUtility.PaneWidthFor(pane), Theme.InspectPaneHeight.Value - 35f);
      return false;
    }
  }
}
