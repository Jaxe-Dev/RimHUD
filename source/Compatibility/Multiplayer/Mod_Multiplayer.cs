using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimHUD.Compatibility.Multiplayer
{
  public static class Mod_Multiplayer
  {
    public static IntegratedMod Instance { get; } = new IntegratedMod("Multiplayer", "0MultiplayerAPI", "Multiplayer.API.MP");

    public static void TryRegister()
    {
      if (!Instance.IsActive) { return; }

      try
      {
        if (!Instance.GetField<bool>("enabled")) { return; }

        var registerSyncMethod = AccessTools.FirstMethod(Instance.Integrator, method => method.Name == "RegisterSyncMethod" && method.GetParameters()[0].ParameterType == typeof(Type));
        registerSyncMethod.Invoke(Instance.MainAssembly, new object[] { typeof(Mod_Multiplayer), nameof(SetSelfTend), null });
        registerSyncMethod.Invoke(Instance.MainAssembly, new object[] { typeof(Mod_Multiplayer), nameof(SetFoodRestriction), null });
        registerSyncMethod.Invoke(Instance.MainAssembly, new object[] { typeof(Mod_Multiplayer), nameof(SetAssignment), null });
        registerSyncMethod.Invoke(Instance.MainAssembly, new object[] { typeof(Mod_Multiplayer), nameof(SetArea), null });
        registerSyncMethod.Invoke(Instance.MainAssembly, new object[] { typeof(Mod_Multiplayer), nameof(SetOutfit), null });

        Mod.Log("Multiplayer ready with API " + Instance.GetField<string>("API"));
      }
      catch (Exception exception) { Instance.FailInitialization(exception); }
    }

    public static void SetSelfTend(Pawn pawn, bool value) => pawn.playerSettings.selfTend = value;

    public static void SetFoodRestriction(Pawn pawn, FoodRestriction value) => pawn.foodRestriction.CurrentFoodRestriction = value;

    public static void SetAssignment(Pawn pawn, int hour, TimeAssignmentDef value) => pawn.timetable.SetAssignment(hour, value);

    public static void SetArea(Pawn pawn, Area value) => pawn.playerSettings.AreaRestriction = value;

    public static void SetOutfit(Pawn pawn, Outfit value) => pawn.outfits.CurrentOutfit = value;
  }
}
