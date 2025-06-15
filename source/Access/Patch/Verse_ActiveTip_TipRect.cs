using HarmonyLib;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(ActiveTip), "TipRect", MethodType.Getter)]
public static class Verse_ActiveTip_TipRect
{
  public static bool Prefix(ActiveTip __instance, ref Rect __result)
  {
    if (!TooltipsPlus.IsFromHud(__instance.signal.uniqueId)) { return true; }

    __result = TooltipsPlus.GetRect(Traverse.Create(__instance)!.Property("FinalText")!.GetValue<string>());

    return false;
  }
}
