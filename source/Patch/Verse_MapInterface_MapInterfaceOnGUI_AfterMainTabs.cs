using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layout;
using RimWorld;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(MapInterface), "MapInterfaceOnGUI_AfterMainTabs")]
  public static class Verse_MapInterface_MapInterfaceOnGUI_AfterMainTabs
  {
    private static void Prefix()
    {
      if (!State.HudFloatingVisible) { return; }

      HudLayout.DrawFloating();
    }
  }
}
