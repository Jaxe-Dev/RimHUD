using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using RimHUD.Interface;
using RimHUD.Patch;
using Verse;

namespace RimHUD.Data
{
    internal static class Persistent
    {
        private const string ConfigFileName = "Config.xml";

        private static bool VersionNeedsNewConfig { get; } = false;

        private static readonly FileInfo ConfigFile = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, ConfigFileName));

        public static void Save()
        {
            var doc = new XDocument();

            var theme = new XElement("RimHUD", new XAttribute("Version", Mod.Version));
            SaveElements(theme, typeof(Theme));
            doc.Add(theme);

            doc.Save(ConfigFile.FullName);
        }

        public static void Load()
        {
            if (!HasConfigFile())
            {
                Save();
                return;
            }

            var doc = XDocument.Load(ConfigFile.FullName);
            var loadedVersion = doc.Root?.Attribute("Version")?.Value;
            if (NeedsNewConfig(loadedVersion))
            {
                Save();
                return;
            }

            LoadElements(doc.Root, typeof(Theme));
        }

        private static bool NeedsNewConfig(string version)
        {
            if (version == Mod.Version) { return false; }
            Mod.Warning($"Loaded config version ({version ?? "NULL"}) is different from the current mod version{(VersionNeedsNewConfig ? ". A new config is required and has been applied" : null)}");

            return true;
        }

        private static bool HasConfigFile()
        {
            ConfigFile.Refresh();
            return ConfigFile.Exists;
        }

        private static void LoadElements(XElement current, object subject)
        {
            var isStatic = subject is Type;
            var type = isStatic ? (Type) subject : subject.GetType();
            var instance = isStatic ? null : subject;

            foreach (var propertyInfo in type.GetProperties())
            {
                var option = propertyInfo.TryGetAttribute<Option>();
                if (option == null) { continue; }

                var propertyValue = propertyInfo.GetValue(instance, null);

                if (option.Label.NullOrEmpty())
                {
                    if (option.Type == null) { return; }

                    var value = option.Convert(current.Value);
                    if (value != null) { propertyInfo.SetValue(instance, value, null); }
                    return;
                }

                var name = option.Label;
                var category = option.Category == null ? current : current.Element(option.Category);
                var element = category?.Element(name);

                if (element == null) { continue; }

                LoadElements(element, propertyValue);
            }
        }

        private static void SaveElements(XElement current, object subject)
        {
            var isStatic = subject is Type;
            var type = isStatic ? (Type) subject : subject.GetType();
            var instance = isStatic ? null : subject;

            var categories = new Dictionary<string, XElement>();

            foreach (var propertyInfo in type.GetProperties())
            {
                var option = propertyInfo.TryGetAttribute<Option>();
                if (option == null) { continue; }

                var propertyValue = propertyInfo.GetValue(instance, null);

                if (option.Label.NullOrEmpty())
                {
                    if (option.Type != null) { current.Value = propertyValue.ToString(); }
                    return;
                }

                var element = new XElement(option.Label);

                if (option.Type != null) { element.Value = propertyValue.ToString(); }
                else { SaveElements(element, propertyValue); }

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

            public object Convert(string text)
            {
                if (Type == null) { return null; }

                object value;
                if (Type == typeof(string)) { value = text; }
                else if (Type == typeof(bool)) { value = text.ToBool(); }
                else if (Type == typeof(int)) { value = text.ToInt(); }
                else { return null; }

                return value;
            }
        }
    }
}
