using System;
using Harmony;
using RimHUD.Data;
using Verse;

namespace RimHUD.Integration
{
    [Persistent.IntegratedOptions]
    internal static class PawnRules
    {
        public const string Url = "https://steamcommunity.com/sharedfiles/filedetails/?id=1499843448";
        public const string Description = "Pawn Rules is a mod that allows custom rules to be assigned individually to your colonists, animals, guests and prisoners:\n\n- Disallow certain foods\n- Disallow bonding with certain animals\n- Disallow new romances\n- Disallow constructing items that have a quality level";
        private const string VersionExpected = "1.1.6";

        public static IntegratedMod Instance { get; } = new IntegratedMod("Pawn Rules", "PawnRules", "Integration.RimHUD", new Version(VersionExpected));

        public static string RequiredAlert => "This requires the mod <b>Pawn Rules</b> to be loaded.\n\n" + Description + "\n\n\nWould you like to open the Steam page for this mod?\n<i>The rules button can be disabled in the options dialog.</i>";

        private static readonly Type RulesDialog = Instance.MainAssembly?.GetType("PawnRules.Interface.Dialog_Rules");

        [Persistent.Option("Options", "HideGizmo")] public static BoolOption HideGizmo { get; } = new BoolOption(false, Lang.Get("Theme.PawnRules.HideGizmo"), Lang.Get("Theme.PawnRules.HideGizmoDesc"), option => Instance.SetValue("HideGizmo", option.Object));

        public static string GetRulesInfo(Pawn pawn) => Instance.InvokeMethod<string>("GetRulesInfo", pawn);

        public static void OpenRulesDialog(Pawn pawn)
        {
            // TODO Remove in next update
            if (Instance.VersionCompare < 0)
            {
                OpenRulesDialogOld(pawn);
                return;
            }

            Instance.InvokeMethod("OpenRulesDialog", pawn);
        }

        // TODO Remove in next update
        private static void OpenRulesDialogOld(Pawn pawn)
        {
            if (RulesDialog == null) { return; }
            Traverse.Create(RulesDialog)?.Method("Open", pawn)?.GetValue();
        }
    }
}
