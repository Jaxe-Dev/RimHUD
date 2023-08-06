using HarmonyLib;
using RimHUD.Interface;
using UnityEngine;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(ActiveTip), "TipRect", MethodType.Getter)]
  public static class Verse_ActiveTip_TipRect
  {
    public static bool Prefix(ActiveTip __instance, ref Rect __result)
    {
      if (__instance.signal.uniqueId != WidgetsPlus.HudTooltipId) { return true; }

      __result = WidgetsPlus.GetTooltipRect(Traverse.Create(__instance).Property("FinalText").GetValue<string>());

      return false;
    }
  }
}
