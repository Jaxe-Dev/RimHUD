using Harmony;
using RimHUD.Data;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MainTabWindow_Inspect), "DoInspectPaneButtons")]
    internal static class RimWorld_MainTabWindow_Inspect_DoInspectPaneButtons
    {
        public static void Postfix(Rect rect, ref float lineEndWidth)
        {
            if (!State.AltInspectPane || !State.PawnSelected) { return; }

            InspectPanePlus.DrawSettingsButtons(rect, ref lineEndWidth);
        }
    }
}
