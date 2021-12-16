using RimHUD.Data.Storage;

namespace RimHUD.Data.Integration
{
  internal static class IntegrationManager
  {
    public static void Initialize()
    {
      Persistent.FinalizeIntegrations();

      Mod_Multiplayer.TryRegister();
      Mod_PawnRules.TryRegister();
    }
  }
}
