using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal static class Persistent
    {
        private const string ConfigFileName = "Config.xml";

        private static bool VersionNeedsNewConfig { get; } = false;
        private static bool _configWasReset;

        private static readonly FileInfo ConfigFile = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, ConfigFileName));

        private static bool HasConfigFile()
        {
            ConfigFile.Refresh();
            return ConfigFile.Exists;
        }

        private static bool NeedsNewConfig(string version)
        {
            if (version == Mod.Version) { return false; }
            Mod.Warning($"Loaded config version ({version ?? "NULL"}) is different from the current mod version");

            return VersionNeedsNewConfig || (version != Mod.LastVersion);
        }

        public static void CheckAlerts()
        {
            if (!_configWasReset) { return; }

            var alert = Lang.Get("Alert.ConfigReset", Mod.Version);
            Mod.Message(alert);
        }

        private static IEnumerable<Type> GetIntegrations() => Assembly.GetExecutingAssembly().GetTypes().Where(type => type.HasAttribute<IntegratedOptions>());

        public static void AllToDefault()
        {
            SetToDefault(typeof(Theme));

            foreach (var integration in GetIntegrations()) { SetToDefault(integration); }
        }

        private static void SetToDefault(object subject)
        {
            var type = GetSubjectType(subject);
            foreach (var property in type.GetProperties())
            {
                var attribute = property.TryGetAttribute<Option>();
                if (attribute == null) { continue; }

                var propertyValue = property.GetValue(null, null);
                if (propertyValue == null) { continue; }

                if (!(propertyValue is IDefaultable option)) { continue; }
                option.ToDefault();
            }
        }

        private static Type GetSubjectType(object subject)
        {
            if (subject is Type type) { return type; }
            return subject.GetType();
        }

        private static string GetIntegrationName(object subject) => "Integration." + GetSubjectType(subject).Name;

        public static void Load()
        {
            if (!HasConfigFile())
            {
                Save();
                return;
            }

            var doc = XDocument.Load(ConfigFile.FullName);
            var versionAttribute = doc.Root?.Attribute("Version");
            var loadedVersion = versionAttribute?.Value;
            if (NeedsNewConfig(loadedVersion))
            {
                Save();
                Mod.Warning($"Updating to version {Mod.Version} required your RimHUD config to be reset to default.");
                _configWasReset = true;
                return;
            }

            if (versionAttribute == null) { doc.Root?.Add(new XAttribute("Version", Mod.Version)); }
            else { versionAttribute.Value = Mod.Version; }

            LoadElements(typeof(Theme), doc.Root);

            foreach (var integration in GetIntegrations()) { LoadClassElements(integration, doc.Root); }

            LoadLayouts();

            doc.Save(ConfigFile.FullName);
        }

        private static void LoadClassElements(object subject, XElement root)
        {
            var element = root.Element(GetIntegrationName(subject));
            if (element == null) { return; }

            LoadElements(subject, element);
        }

        private static void LoadElements(object subject, XElement current)
        {
            var type = GetSubjectType(subject);
            var instance = subject is Type ? null : subject;

            foreach (var propertyInfo in type.GetProperties())
            {
                var option = propertyInfo.TryGetAttribute<Option>();
                if (option == null) { continue; }

                var propertyValue = propertyInfo.GetValue(instance, null);

                if (option.Label.NullOrEmpty())
                {
                    if (option.Type == null) { return; }

                    var value = option.ConvertFromXml(current.Value);
                    if (value != null) { propertyInfo.SetValue(instance, value, null); }
                    return;
                }

                var name = option.Label;
                var category = option.Category == null ? current : current.Element(option.Category);
                var element = category?.Element(name);

                if (element == null) { continue; }

                LoadElements(propertyValue, element);
            }
        }

        public static void Save()
        {
            var doc = new XDocument();

            var theme = SaveClassElements(typeof(Theme), "Theme");
            theme.Add(new XAttribute("Version", Mod.Version));

            foreach (var integration in GetIntegrations()) { theme.Add(SaveClassElements(integration)); }

            doc.Add(theme);

            doc.Save(ConfigFile.FullName);
        }

        private static XElement SaveClassElements(object subject, string name = null)
        {
            var element = new XElement(name ?? GetIntegrationName(subject));
            SaveElements(subject, element);
            return element;
        }

        private static void SaveElements(object subject, XElement current)
        {
            var type = GetSubjectType(subject);
            var instance = subject is Type ? null : subject;

            var categories = new Dictionary<string, XElement>();

            foreach (var propertyInfo in type.GetProperties())
            {
                var option = propertyInfo.TryGetAttribute<Option>();
                if (option == null) { continue; }

                var propertyValue = propertyInfo.GetValue(instance, null);

                if (option.Label.NullOrEmpty())
                {
                    if (option.Type != null) { current.Value = option.ConvertToXml(propertyValue); }
                    return;
                }

                var element = new XElement(option.Label);

                if (option.Type != null) { element.Value = option.ConvertToXml(propertyValue); }
                else { SaveElements(propertyValue, element); }

                if (element.IsEmpty) { continue; }
                if (option.Category == null) { current.Add(element); }
                else
                {
                    if (!categories.ContainsKey(option.Category)) { categories[option.Category] = new XElement(option.Category); }
                    categories[option.Category].Add(element);
                }
            }

            if (categories.Count > 0) { current.Add(categories.Values); }
        }

        private static void LoadLayouts()
        {
            var docked = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.DockedFileName));
            var floating = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.FloatingFileName));

            if (docked.Exists) { HudLayout.Docked = HudLayout.FromXml(XDocument.Load(docked.FullName)); }
            else { HudLayout.Docked.AsXDocument().Save(docked.FullName); }

            if (floating.Exists) { HudLayout.Floating = HudLayout.FromXml(XDocument.Load(floating.FullName)); }
            else { HudLayout.Floating.AsXDocument().Save(floating.FullName); }
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class IntegratedOptions : Attribute
        { }

        [AttributeUsage(AttributeTargets.Property)]
        public class Option : Attribute
        {
            public Type Type { get; }
            public string Category { get; }
            public string Label { get; }

            public Option(string label, Type type = null) : this(null, label, type)
            { }
            public Option(Type type) : this(null, null, type)
            { }
            public Option(string category, string label, Type type = null)
            {
                Category = category;
                Label = label;
                Type = type;
            }

            public string ConvertToXml(object value)
            {
                if (Type == null) { return null; }

                return Type == typeof(Color) ? ((Color) value).ToHex() : value.ToString();
            }

            public object ConvertFromXml(string text)
            {
                if (Type == null) { return null; }

                object value;
                if (Type == typeof(string)) { value = text; }
                else if (Type == typeof(bool)) { value = text.ToBool(); }
                else if (Type == typeof(int)) { value = text.ToInt(); }
                else if (Type == typeof(Color)) { value = GUIPlus.HexToColor(text); }
                else { return null; }

                return value;
            }
        }
    }
}
