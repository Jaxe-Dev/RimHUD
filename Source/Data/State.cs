using RimHUD.Data.Configuration;
using RimWorld;
using Verse;

namespace RimHUD.Data
{
    internal static class State
    {
        public static bool Activated { get; set; } = true;
        public static bool ModifyPane => ShowPane && Theme.InspectPaneTabModify.Value;
        public static bool CompressLetters => Active && Theme.LetterCompress.Value;
        public static bool HudFloatingVisible => !Theme.HudDocked.Value && ShowPane;

        public static Pawn SelectedPawn => GetSelectedPawn();

        private static bool Active => Activated && (Current.ProgramState == ProgramState.Playing);
        private static bool ShowPane => Active && MainButtonDefOf.Inspect.TabWindow.IsOpen && (SelectedPawn != null);

        private static Pawn GetSelectedPawn()
        {
            var thing = Find.Selector.SingleSelectedThing;
            return thing is Pawn pawn ? pawn : null;
        }
    }
}
