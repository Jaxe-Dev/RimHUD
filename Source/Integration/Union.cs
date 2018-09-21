using System.Linq;
using System.Reflection;
using Verse;

namespace RimHUD.Integration
{
    internal static class Union
    {
        public static bool PawnRules => Integration.PawnRules.IsLoaded;
        public static bool Bubbles => Integration.Bubbles.IsLoaded;

        public static Assembly GetModAssembly(string name, string id = null)
        {
            var found = LoadedModManager.RunningModsListForReading.FirstOrDefault(mod => mod.Identifier == (id ?? name))?.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == name);
            if (found != null) { Mod.Log($"Integrated with {id ?? name}"); }

            return found;
        }
    }
}
