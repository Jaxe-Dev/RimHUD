using HarmonyLib;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
  internal static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
  {
    private static void Postfix(WidgetRow row, bool worldView)
    {
      if (worldView || row == null) { return; }

      var showHud = State.Activated;
      row.ToggleableIcon(ref showHud, Textures.ToggleIcon, Lang.Get("ToggleHud"), SoundDefOf.Mouseover_ButtonToggle);
      State.Activated = showHud;
    }
  }
}
