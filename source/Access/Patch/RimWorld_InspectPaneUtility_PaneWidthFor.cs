using System.Linq;
using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(InspectPaneUtility), "PaneWidthFor")]
public static class RimWorld_InspectPaneUtility_PaneWidthFor
{
  private static bool Prefix(ref float __result, IInspectPane? pane)
  {
    if (!State.ModifyPane || pane is null) { return true; }

    __result = Theme.InspectPaneTabWidth.Value * Mathf.Max(Theme.InspectPaneMinTabs.Value, pane.CurTabs?.Count(static tab => tab.IsVisible && !tab.Hidden) ?? 0f);

    return false;
  }
}
