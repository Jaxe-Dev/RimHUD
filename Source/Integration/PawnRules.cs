using System;
using System.Reflection;
using Harmony;
using Verse;

namespace RimHUD.Integration
{
    internal static class PawnRules
    {
        public const string Url = "https://steamcommunity.com/sharedfiles/filedetails/?id=1499843448";
        public const string Description = "Pawn Rules is a mod that allows custom rules to be assigned individually to your colonists, animals, guests and prisoners:\n\n- Disallow certain foods\n- Disallow bonding with certain animals\n- Disallow new romances\n- Disallow constructing items that have a quality level";
        public const string RequiredAlert = "This requires the mod <b>Pawn Rules</b> to be loaded.\n<i>The rules button can be removed in the options dialog.</i>\n\n" + Description + "\n\n\nWould you like to open the Steam page for this mod?";

        private static readonly Assembly Assembly = Union.GetModAssembly("PawnRules");
        private static readonly Type Integrator = Assembly?.GetType("PawnRules.Integration.RimHUD");
        private static readonly Type RulesDialog = Assembly?.GetType("PawnRules.Interface.Dialog_Rules");

        public static readonly bool IsLoaded = Integrator != null;

        public static string GetRulesInfo(Pawn pawn) => Traverse.Create(Integrator)?.Method("GetRulesInfo", pawn)?.GetValue<string>();
        public static void SetHideGizmo(bool value)
        {
            if (!IsLoaded) { return; }
            AccessTools.Property(Integrator, "HideGizmo").SetValue(null, value, null);
        }

        public static void OpenRulesDialog(Pawn pawn)
        {
            if (!IsLoaded) { return; }

            Traverse.Create(RulesDialog)?.Method("Open", pawn)?.GetValue();
        }
    }
}
