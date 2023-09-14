using System;
using RimHUD.Engine;
using RimWorld;
using Verse;

namespace RimHUD.Integration.Multiplayer
{
  public sealed class Mod_Multiplayer : IntegratedMod
  {
    public Mod_Multiplayer() : base("Multiplayer", "0MultiplayerAPI", "Multiplayer.API.MP")
    {
      try
      {
        if (!Traverse.Field("enabled")!.GetValue<bool>()) { return; }

        var registerSyncMethod = Traverse.Method("RegisterSyncMethod");
        if (registerSyncMethod is null) { throw new Exception("Failed to find RegisterSyncMethod."); }

        registerSyncMethod.GetValue(typeof(Mod_Multiplayer), nameof(SetSelfTend), null);

        registerSyncMethod.GetValue(typeof(Mod_Multiplayer), nameof(SetFoodRestriction), null);
        registerSyncMethod.GetValue(typeof(Mod_Multiplayer), nameof(SetAssignment), null);
        registerSyncMethod.GetValue(typeof(Mod_Multiplayer), nameof(SetArea), null);
        registerSyncMethod.GetValue(typeof(Mod_Multiplayer), nameof(SetOutfit), null);

        Report.Log($"Multiplayer ready with API {Traverse.Field("API")!.GetValue<string>()}");
      }
      catch (Exception exception) { DisableFrom(exception); }
    }

    public static void SetSelfTend(Pawn pawn, bool value) => pawn.playerSettings!.selfTend = value;

    public static void SetFoodRestriction(Pawn pawn, FoodRestriction value) => pawn.foodRestriction!.CurrentFoodRestriction = value;

    public static void SetAssignment(Pawn pawn, int hour, TimeAssignmentDef value) => pawn.timetable!.SetAssignment(hour, value);

    public static void SetArea(Pawn pawn, Area? value) => pawn.playerSettings!.AreaRestriction = value;

    public static void SetOutfit(Pawn pawn, Outfit value) => pawn.outfits!.CurrentOutfit = value;
  }
}
