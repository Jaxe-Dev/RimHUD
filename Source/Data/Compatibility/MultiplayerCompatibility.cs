using Multiplayer.API;
using RimWorld;
using Verse;

namespace RimHUD.Data.Compatibility
{
    internal static class MultiplayerCompatibility
    {
        [SyncMethod] public static void SetSelfTend(Pawn pawn, bool value) => pawn.playerSettings.selfTend = value;

        [SyncMethod] public static void SetFoodRestriction(Pawn pawn, FoodRestriction value) => pawn.foodRestriction.CurrentFoodRestriction = value;

        [SyncMethod] public static void SetAssignment(Pawn pawn, int hour, TimeAssignmentDef value) { pawn.timetable.SetAssignment(hour, value); }

        [SyncMethod] public static void SetArea(Pawn pawn, Area value) => pawn.playerSettings.AreaRestriction = value;

        [SyncMethod] public static void SetOutfit(Pawn pawn, Outfit value) => pawn.outfits.CurrentOutfit = value;
    }
}
