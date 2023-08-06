using System;
using System.Collections.Generic;
using RimHUD.Compatibility.Multiplayer;
using RimHUD.Compatibility.PawnRules;
using RimHUD.Configuration;
using RimHUD.Interface.Hud.Widgets;
using Verse;

namespace RimHUD.Compatibility
{
  public static class IntegrationManager
  {
    public static Dictionary<string, (string, Func<Pawn, IWidget>)> ThirdPartyWidgets { get; } = new Dictionary<string, (string, Func<Pawn, IWidget>)>();

    public static void EarlyInitialization()
    { }

    public static void FinalInitialization()
    {
      Persistent.FinalizeIntegratedOptions();

      Mod_Multiplayer.TryRegister();
      Mod_PawnRules.TryRegister();
    }

    public static void RegisterThirdPartyWidget(string id, string label, Func<Pawn, IWidget> getter)
    {
      ThirdPartyWidgets["ThirdParty_" + id] = (label, getter);
      Mod.Log($"Registered third-party widget '{id}'");
    }
  }
}
