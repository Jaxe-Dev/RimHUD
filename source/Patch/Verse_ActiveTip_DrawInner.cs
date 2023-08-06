using HarmonyLib;
using RimHUD.Interface;
using UnityEngine;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(ActiveTip), "DrawInner")]
  public static class Verse_ActiveTip_DrawInner
  {
    public static bool Prefix(ActiveTip __instance, Rect bgRect, string label)
    {
      if (__instance.signal.uniqueId != WidgetsPlus.HudTooltipId) { return true; }

      WidgetsPlus.DrawTooltipInner(bgRect, label);

      return false;
    }
  }
}
