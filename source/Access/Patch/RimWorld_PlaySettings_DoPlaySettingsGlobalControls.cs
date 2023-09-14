using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface;
using RimHUD.Interface.Dialog;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
  public static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
  {
    private static void Postfix(WidgetRow? row, bool worldView)
    {
      if (worldView || row is null) { return; }

      var showHud = State.Activated;
      row.ToggleableIcon(ref showHud, TexturesPlus.ToggleIcon, Lang.Get("ToggleHud"), SoundDefOf.Mouseover_ButtonToggle);

      if (Event.current!.shift && showHud != State.Activated) { Dialog_Config.Open(); }
      else { State.Activated = showHud; }
    }
  }
}
