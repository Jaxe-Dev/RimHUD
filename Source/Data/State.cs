using RimHUD.Data.Configuration;
using RimWorld.Planet;
using Verse;

namespace RimHUD.Data
{
    internal static class State
    {
        public static bool Activated { get; set; } = true;
        public static bool Available => Activated && (Current.ProgramState == ProgramState.Playing) && !IsWorldView;
        public static bool AltInspectPane => Available && Theme.InspectPaneTabModify.Value;
        public static bool AltLetters => Available && Theme.LetterCompress.Value;
        public static bool PawnSelected => SelectedPawn != null;
        public static bool HudDockedVisible => Theme.HudDocked.Value && Available && PawnSelected;
        public static bool HudFloatingVisible => !Theme.HudDocked.Value && Available && PawnSelected;

        public static Pawn SelectedPawn => GetSelectedPawn();
        public static bool IsWorldView => WorldRendererUtility.WorldRenderedNow;

        private static Pawn GetSelectedPawn()
        {
            var thing = Find.Selector.SingleSelectedThing;
            return thing is Pawn pawn ? pawn : null;
        }
    }
}
