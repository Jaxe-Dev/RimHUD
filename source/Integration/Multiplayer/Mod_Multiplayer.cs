using System;
using HarmonyLib;
using RimHUD.Engine;
using RimWorld;
using Verse;

namespace RimHUD.Integration.Multiplayer;

public sealed class Mod_Multiplayer : IntegratedMod
{
  public Mod_Multiplayer() : base("Multiplayer", "0MultiplayerAPI", "Multiplayer.API.MP")
  {
    try
    {
      if (!Traverse.Field("enabled")!.GetValue<bool>()) { return; }

      var registerSyncMethod = Traverse.Method("RegisterSyncMethod", [typeof(Type), typeof(string), AccessTools.TypeByName("Multiplayer.API.SyncType")?.MakeArrayType() ?? throw new Exception("Failed to find RegisterSyncMethod argument types.")]);
      if (registerSyncMethod is null || !registerSyncMethod.MethodExists()) { throw new Exception("Failed to find RegisterSyncMethod."); }

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

  public static void SetFoodRestriction(Pawn pawn, FoodPolicy value) => pawn.foodRestriction!.CurrentFoodPolicy = value;

  public static void SetAssignment(Pawn pawn, int hour, TimeAssignmentDef value) => pawn.timetable!.SetAssignment(hour, value);

  public static void SetArea(Pawn pawn, Area? value) => pawn.playerSettings!.AreaRestrictionInPawnCurrentMap = value;

  public static void SetOutfit(Pawn pawn, ApparelPolicy value) => pawn.outfits!.CurrentApparelPolicy = value;
}
