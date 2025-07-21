using System;
using RimHUD.Engine;
using RimHUD.Integration.Multiplayer;
using RimHUD.Integration.PawnRules;

namespace RimHUD.Integration;

public static class Integrations
{
  private static readonly Type[] Types =
  [
    typeof(Mod_Multiplayer),
    typeof(Mod_PawnRules)
  ];

  public static void Load()
  {
    foreach (var type in Types)
    {
      try { Activator.CreateInstance(type); }
      catch (Exception exception) { Report.Error($"Integrated mod '{type.Name}' failed to register due to the following error:\n{exception.Message}."); }
    }
  }
}
