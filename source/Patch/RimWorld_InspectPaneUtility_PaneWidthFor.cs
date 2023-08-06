using System.Linq;
using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "PaneWidthFor")]
  public static class RimWorld_InspectPaneUtility_PaneWidthFor
  {
    private static bool Prefix(ref float __result, IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      var count = pane.CurTabs.Count(tab => tab.IsVisible && !tab.Hidden);
      __result = Theme.InspectPaneTabWidth.Value * (float)Mathf.Max(Theme.InspectPaneMinTabs.Value, count);
      return false;
    }
  }
}
