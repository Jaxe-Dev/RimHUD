using HarmonyLib;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(ActiveTip), "DrawInner")]
public static class Verse_ActiveTip_DrawInner
{
  private static bool Prefix(ActiveTip __instance, Rect bgRect, string label)
  {
    if (!TooltipsPlus.IsFromHud(__instance.signal.uniqueId)) { return true; }

    TooltipsPlus.DrawInner(bgRect, label);

    return false;
  }
}
