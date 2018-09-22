using System.Linq;
using System.Reflection;
using Verse;

namespace RimHUD.Integration
{
    internal static class Union
    {
        public static bool PawnRules => Integration.PawnRules.IsLoaded;
        public static bool Bubbles => Integration.Bubbles.IsLoaded;

        public static Assembly GetModAssembly(string modName, string assemblyName)
        {
            var found = LoadedModManager.RunningModsListForReading.FirstOrDefault(mod => mod.Name == modName)?.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
            if (found != null) { Mod.Log($"Integrated with {modName}"); }

            return found;
        }
    }
}
