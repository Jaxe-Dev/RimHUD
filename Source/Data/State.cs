using RimHUD.Interface;
using RimWorld.Planet;
using Verse;

namespace RimHUD.Data
{
    internal static class State
    {
        public static bool Activated { get; set; } = true;
        public static bool AltInspectPane => Activated && !IsWorldView && Theme.InspectPaneTabModify.Value;
        public static bool AltLetters => Activated && !IsWorldView && Theme.LetterCompress.Value;
        public static bool PawnSelected => Activated && (Current.ProgramState == ProgramState.Playing) && (SelectedPawn != null);
        public static bool ValidSelected => PawnSelected && !IsWorldView;
        public static bool HudDockedVisible => Activated && ValidSelected && Theme.HudDocked.Value;
        public static bool HudFloatingVisible => Activated && ValidSelected && !Theme.HudDocked.Value;

        public static Pawn SelectedPawn => GetSelectedPawn();
        public static bool IsWorldView => WorldRendererUtility.WorldRenderedNow;

        private static Pawn GetSelectedPawn()
        {
            var thing = Find.Selector.SingleSelectedThing;

            if (thing is Pawn pawn) { return pawn; }
            if (thing is Corpse corpse) { return corpse.InnerPawn; }

            return null;
        }
    }
}
