using System;
using System.Linq;
using System.Reflection;
using Harmony;

namespace RimHUD.Integration
{
    internal static class HugsLib
    {
        public static void RegisterUpdateFeature()
        {
            var hugsLib = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Name == "HugsLibController" select type).FirstOrDefault();
            if (hugsLib == null) { return; }

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Traverse.Create(hugsLib).Field("instance")?.Property("UpdateFeatures")?.Method("InspectActiveMod", Mod.Id, version).GetValue();
        }
    }
}
