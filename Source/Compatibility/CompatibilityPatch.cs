using System;
using RimHUD.Data;

namespace RimHUD.Compatibility
{
    internal abstract class CompatibilityPatch : ExternalMod
    {
        public bool IsExperimental { get; }
        protected CompatibilityPatch(string name, string assemblyName, Version versionExpected, bool isExperimental) : base(name, assemblyName, versionExpected) => IsExperimental = isExperimental;

        public abstract bool OnStartup();

        public void ReportApplied()
        {
            var message = $"Compatibility fix required and applied for '{Name}'. ";
            if (IsExperimental) { Mod.Warning(message + "This patch is experimental and may not work as intended if that mod has been updated recently"); }
            else { Mod.Log(message + "This may not work as intended if that mod has been updated recently"); }
        }
    }
}
