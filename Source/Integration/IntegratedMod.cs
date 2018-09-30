using System;
using Harmony;
using RimHUD.Data;

namespace RimHUD.Integration
{
    internal class IntegratedMod : ExternalMod
    {
        public Type Integrator { get; }

        public IntegratedMod(string name, string assemblyName, string integratorName, Version versionExpected = null) : base(name, assemblyName, versionExpected)
        {
            if (!IsActive) { return; }

            Integrator = MainAssembly.GetType(assemblyName + "." + integratorName);
            if (Integrator == null)
            {
                Mod.Warning($"Integrator '{integratorName}' not found for '{Name}'");
                return;
            }

            Mod.Log($"Integrated with {Name}");
        }

        public void SetValue(string name, object value)
        {
            if (!IsActive) { return; }
            AccessTools.Property(Integrator, name)?.SetValue(null, value, null);
        }

        public T GetValue<T>(string name)
        {
            if (!IsActive) { return default(T); }
            return (T) AccessTools.Property(Integrator, name)?.GetValue(null, null);
        }

        public void InvokeMethod(string name, params object[] parameters)
        {
            if (!IsActive) { return; }
            AccessTools.Method(Integrator, name)?.Invoke(null, parameters);
        }

        public T InvokeMethod<T>(string name, params object[] parameters)
        {
            if (!IsActive) { return default(T); }
            return (T) AccessTools.Method(Integrator, name)?.Invoke(null, parameters);
        }
    }
}
