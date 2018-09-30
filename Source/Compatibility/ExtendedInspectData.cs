using System;
using System.Linq;
using System.Reflection;
using Harmony;
using RimHUD.Data;
using RimWorld;
using Verse;

namespace RimHUD.Compatibility
{
    internal class ExtendedInspectData : CompatibilityPatch
    {
        public ExtendedInspectData() : base("ExtendedInspectData [B19]", "ExtendedInspectData", new Version("1.0.0.0"), true)
        { }

        public override bool OnStartup()
        {
            if (!IsActive) { return false; }

            var type = MainAssembly?.GetType("ZoneInspectData.MainTabWindow_InspectZone_Stockpile");
            var method = type?.GetMethod("ExtraOnGUI");
            if (method == null) { return false; }

            if (Mod.Harmony.GetPatchedMethods().All(patch => patch != method)) { Mod.Harmony.Patch(method, new HarmonyMethod(typeof(ExtendedInspectData).GetMethod(nameof(Prefix_ExtraOnGui), BindingFlags.NonPublic | BindingFlags.Static))); }
            return true;
        }

        private static bool Prefix_ExtraOnGui(MainTabWindow_Inspect __instance)
        {
            if (!State.HudDockedVisible) { return true; }

            InspectPaneUtility.ExtraOnGUI(__instance);
            if (__instance.AnythingSelected && (Find.DesignatorManager.SelectedDesignator != null)) { Find.DesignatorManager.SelectedDesignator.DoExtraGuiControls(0f, __instance.PaneTopY); }

            return false;
        }
    }
}
