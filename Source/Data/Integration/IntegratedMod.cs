using System;
using Harmony;

namespace RimHUD.Data.Integration
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
            if (!IsActive) { return default; }
            return (T) AccessTools.Property(Integrator, name)?.GetValue(null, null);
        }

        public void InvokeMethod(string name, params object[] parameters) => InvokeMethod<object>(name, parameters);

        public T InvokeMethod<T>(string name, params object[] parameters)
        {
            if (!IsActive) { return default; }
            try { return (T) AccessTools.Method(Integrator, name)?.Invoke(null, parameters); }
            catch (Exception exception) { throw new Mod.Exception($"Exception while invoking method '{name}' for external mod '{Name}'", exception); }
        }
    }
}
