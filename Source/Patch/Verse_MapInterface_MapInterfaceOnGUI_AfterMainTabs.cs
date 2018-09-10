using System;
using Harmony;
using RimHUD.Interface;
using RimWorld;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(MapInterface), nameof(MapInterface.MapInterfaceOnGUI_BeforeMainTabs), new Type[] { })]
    internal static class Verse_MapInterface_MapInterfaceOnGUI_BeforeMainTabs
    {
        private static void Postfix() => Hud.Draw();
    }
}
