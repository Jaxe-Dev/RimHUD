using System.Linq;
using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "PaneWidthFor")]
  public static class RimWorld_InspectPaneUtility_PaneWidthFor
  {
    private static bool Prefix(ref float __result, IInspectPane pane)
    {
      if (!State.ModifyPane) { return true; }

      __result = Theme.InspectPaneTabWidth.Value * (float)Mathf.Max(Theme.InspectPaneMinTabs.Value, pane.CurTabs.Count(static tab => tab.IsVisible && !tab.Hidden));

      return false;
    }
  }
}
