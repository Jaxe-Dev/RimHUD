using Harmony;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls", typeof(WidgetRow), typeof(bool))]
    internal static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
    {
        private static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView || (row == null)) { return; }

            var showHud = Hud.Activated;
            row.ToggleableIcon(ref showHud, Theme.ToggleIcon, Lang.Get("ToggleHud"), SoundDefOf.Mouseover_ButtonToggle);
            Hud.Activated = showHud;
        }
    }
}
