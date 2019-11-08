using System;
using RimHUD.Data.Theme;
using Verse;

namespace RimHUD.Data.Integration
{
    [Attributes.IntegratedOptions]
    internal static class PawnRules
    {
        public const string Url = "https://steamcommunity.com/sharedfiles/filedetails/?id=1499843448";
        public const string Description = "Pawn Rules is a mod that allows custom rules to be assigned individually to your colonists, animals, guests and prisoners:\n\n- Disallow certain foods\n- Disallow bonding with certain animals\n- Disallow new romances\n- Disallow constructing items that have a quality level";
        private const string VersionExpected = "1.3.4";

        public static IntegratedMod Instance { get; } = new IntegratedMod("Pawn Rules", "PawnRules", "Integration.RimHUD", new Version(VersionExpected));

        [Attributes.Option("Options", "ReplaceFoodSelector")] public static BoolOption ReplaceFoodSelector { get; } = new BoolOption(true, Lang.Get("Integration.PawnRules.ReplaceFoodSelector"), Lang.Get("Integration.PawnRules.ReplaceFoodSelectorDesc"), option => Instance.SetValue("ReplaceFoodSelector", option.Object));
        [Attributes.Option("Options", "HideGizmo")] public static BoolOption HideGizmo { get; } = new BoolOption(true, Lang.Get("Integration.PawnRules.HideGizmo"), Lang.Get("Integration.PawnRules.HideGizmoDesc"), option => Instance.SetValue("HideGizmo", option.Object));

        public static string GetRules(Pawn pawn) => Instance.InvokeMethod<string>("GetRules", pawn);

        public static void OpenRules(Pawn pawn) => Instance.InvokeMethod("OpenRules", pawn);

        //public static void CanHaveRules(Pawn pawn) => Instance.InvokeMethod("CanHaveRules", pawn); // TODO Not yet implemented
        public static bool CanHaveRules(Pawn pawn) => (pawn != null) && !pawn.Dead && (((pawn.Faction != null) && pawn.Faction.IsPlayer && (pawn.IsColonist || pawn.RaceProps.Animal)) || ((pawn.HostFaction != null) && pawn.HostFaction.IsPlayer));
    }
}
