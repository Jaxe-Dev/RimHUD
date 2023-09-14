using HarmonyLib;
using RimHUD.Interface.Screen;
using RimWorld;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(MapInterface), "Notify_SwitchedMap")]
  public static class Verse_MapInterface_Notify_SwitchedMap
  {
    private static void Postfix() => InspectPaneLog.ClearCache();
  }
}
