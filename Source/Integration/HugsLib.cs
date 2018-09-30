using System.Reflection;
using Harmony;

namespace RimHUD.Integration
{
    internal static class HugsLib
    {
        public static IntegratedMod Instance { get; } = new IntegratedMod("HugsLib", "HugsLib", "HugsLibController");

        public static void RegisterUpdateFeature()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Traverse.Create(Instance.Integrator).Field("instance")?.Property("UpdateFeatures")?.Method("InspectActiveMod", Mod.Id, version).GetValue();
        }
    }
}
