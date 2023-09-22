using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(InspectPaneUtility), "PaneSizeFor")]
  public static class RimWorld_InspectPaneUtility_PaneSizeFor
  {
    private static bool Prefix(ref Vector2 __result, IInspectPane? pane)
    {
      if (!State.ModifyPane || pane is null) { return true; }

      __result = new Vector2(InspectPaneUtility.PaneWidthFor(pane), Theme.InspectPaneHeight.Value - WidgetsPlus.MainButtonHeight);

      return false;
    }
  }
}
