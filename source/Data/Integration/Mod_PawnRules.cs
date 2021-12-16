using System;
using Verse;

namespace RimHUD.Data.Integration
{
  [Attributes.IntegratedOptions]
  internal static class Mod_PawnRules
  {
    public static IntegratedMod Instance { get; } = new IntegratedMod("Pawn Rules", "PawnRules", "PawnRules.Integration.RimHUD");

    public static void TryRegister()
    {
      if (!Instance.IsActive) { return; }

      try
      {
        Instance.RegisterMethod("GetRules");
        Instance.RegisterMethod("OpenRules");
        Instance.RegisterMethod("CanHaveRules");
        Instance.SetProperty("ReplaceFoodSelector", true);
        Instance.SetProperty("HideGizmo", true);
      }
      catch (Exception exception) { Instance.FailInitialization(exception); }
    }

    public static string GetRules(Pawn pawn) => Instance.InvokeMethod<string>("GetRules", pawn);

    public static void OpenRules(Pawn pawn) => Instance.InvokeMethod("OpenRules", pawn);

    public static bool CanHaveRules(Pawn pawn) => Instance.InvokeMethod<bool>("CanHaveRules", pawn);
  }
}
