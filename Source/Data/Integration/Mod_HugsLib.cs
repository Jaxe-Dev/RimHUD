using System;
using System.Reflection;
using Harmony;

namespace RimHUD.Data.Integration
{
    internal static class Mod_HugsLib
    {
        public static IntegratedMod Instance { get; } = new IntegratedMod("HugsLib", "HugsLib", "HugsLib.HugsLibController");

        public static void TryRegisterUpdateFeature()
        {
            try
            {
                if (!Instance.IsActive) { return; }
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Traverse.Create(Instance.Integrator).Field("instance")?.Property("UpdateFeatures")?.Method("InspectActiveMod", Mod.Id, version).GetValue();
            }
            catch (Exception exception) { Instance.FailInitialization(exception); }
        }
    }
}
