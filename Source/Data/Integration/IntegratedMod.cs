using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimHUD.Patch;

namespace RimHUD.Data.Integration
{
    internal class IntegratedMod : ExternalMod
    {
        public Type Integrator { get; }

        public override bool IsActive => !Disabled && base.IsActive && (Integrator != null);
        public bool Disabled { get; set; }

        private readonly Dictionary<string, Access.StaticMethodHandler> _methods = new Dictionary<string, Access.StaticMethodHandler>();
        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();

        public IntegratedMod(string name, string assemblyName, string integratorName, Version versionExpected = null) : base(name, assemblyName, versionExpected)
        {
            if (!base.IsActive) { return; }

            Integrator = MainAssembly.GetType(integratorName);
            if (Integrator == null)
            {
                Mod.Warning($"Integrator '{integratorName}' not found for '{Name}'");
                return;
            }

            Mod.Log($"Integrated with {Name}");
        }

        public T GetField<T>(string name)
        {
            if (!IsActive) { return default; }
            return (T) GetCachedField(name)?.GetValue(null);
        }

        public void SetField(string name, object value)
        {
            if (!IsActive) { return; }
            GetCachedField(name)?.SetValue(null, value);
        }

        public T GetProperty<T>(string name)
        {
            if (!IsActive) { return default; }
            return (T) GetCachedProperty(name)?.GetValue(null, null);
        }

        public void SetProperty(string name, object value)
        {
            if (!IsActive) { return; }
            GetCachedProperty(name)?.SetValue(null, value, null);
        }

        public T InvokeMethod<T>(string id, params object[] parameters) => !IsActive ? default : GetCachedMethod(id).Invoke<T>(parameters);
        public void InvokeMethod(string id, params object[] parameters) => InvokeMethod<object>(id, parameters);

        private PropertyInfo GetCachedProperty(string name)
        {
            if (_properties.ContainsKey(name)) { return _properties[name]; }

            try { _properties[name] = AccessTools.Property(Integrator, name); }
            catch { _properties[name] = null; }

            return _properties[name];
        }

        private FieldInfo GetCachedField(string name)
        {
            if (_fields.ContainsKey(name)) { return _fields[name]; }

            try { _fields[name] = AccessTools.Field(Integrator, name); }
            catch { _fields[name] = null; }

            return _fields[name];
        }

        private Access.StaticMethodHandler GetCachedMethod(string id) => _methods[id];

        public void RegisterMethod(string id, string name = null, Type[] parameters = null, Type[] generics = null) => _methods[id] = new Access.StaticMethodHandler(Integrator, name ?? id, parameters, generics);

        public void FailInitialization(Exception exception)
        {
            Disabled = true;
            var info = new Mod.ExceptionInfo(exception);
            Mod.Warning($"Integrated mod '{Name}' failed initialization due to the following error:\n{info.Text}");
        }
    }
}
