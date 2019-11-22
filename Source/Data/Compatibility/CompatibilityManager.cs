using System;
using System.Collections.Generic;

namespace RimHUD.Data.Compatibility
{
    internal static class CompatibilityManager
    {
        private static readonly List<CompatibilityPatch> List = new List<CompatibilityPatch>();
        public static CompatibilityPatch[] Patches => List.ToArray();

        public static void Initialize()
        {
            List.Clear();
            foreach (var type in Mod.Assembly.GetTypes())
            {
                if ((type.BaseType != typeof(CompatibilityPatch)) || type.IsAbstract) { continue; }

                var compatibility = Activator.CreateInstance(type) as CompatibilityPatch;
                if (!compatibility?.OnStartup() ?? true) { continue; }

                List.Add(compatibility);
                compatibility.ReportApplied();
            }
        }
    }
}
