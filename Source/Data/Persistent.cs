using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Integration;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using Verse;

namespace RimHUD.Data
{
    internal static class Persistent
    {
        private const string ConfigFileName = "Config.xml";
        private const string FixedPresetsDirectoryName = "RimHUD\\Presets";
        private const string UserPresetsDirectoryName = "Presets";
        private const string PresetExtension = ".xml";

        private static bool VersionNeedsNewConfig { get; } = Mod.VersionNeedsNewConfig;
        public static bool IsLoaded { get; private set; }

        private static bool _configWasReset;

        private static readonly FileInfo ConfigFile = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, ConfigFileName));
        private static readonly DirectoryInfo UserPresetDirectory = new DirectoryInfo(Path.Combine(Mod.ConfigDirectory.FullName, UserPresetsDirectoryName));

        private static readonly Regex ValidFilenameRegex = new Regex("^(?:[\\p{L}\\p{N}_\\-]|[\\p{L}\\p{N}_\\-]+[\\p{L}\\p{N}_\\- ]*[\\p{L}\\p{N}_\\-]+)$");
        public static bool IsValidFilename(string name) => !name.NullOrEmpty() && (name.Length <= 250 - UserPresetDirectory.FullName.Length) && ValidFilenameRegex.IsMatch(name);

        public static void CheckAlerts()
        {
            if (!_configWasReset) { return; }

            var alert = Lang.Get("Alert.ConfigReset", Mod.Version);
            Mod.Message(alert);
        }

        private static IEnumerable<Type> GetIntegrations() => Assembly.GetExecutingAssembly().GetTypes().Where(type => type.HasAttribute<Attributes.IntegratedOptions>());

        public static void AllToDefault()
        {
            SetToDefault(typeof(Theme));

            foreach (var integration in GetIntegrations()) { SetToDefault(integration); }

            HudLayout.LoadDefaultAndSave();
        }

        private static void SetToDefault(object subject)
        {
            var type = GetSubjectType(subject);
            foreach (var property in type.GetProperties())
            {
                var attribute = property.TryGetAttribute<Attributes.Option>();
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

        private static bool NeedsNewConfig(string version)
        {
            if (version == Mod.Version) { return false; }
            Mod.Warning($"Loaded config version ({version ?? "NULL"}) is different from the current mod version");

            return VersionNeedsNewConfig || (version != Mod.LastVersion);
        }

        public static XElement LoadXml(FileInfo file, bool errorOnFail = false)
        {
            try
            {
                return XDocument.Load(file.FullName).Root;
            }
            catch (Exception ex)
            {
                var message = $"Failed to load xml file '{file.FullName}' due to exception '{ex.Message}'";
                if (errorOnFail) { Mod.Error(message); }
                else { Mod.Warning(message); }

                return null;
            }
        }

        public static void Load()
        {
            LoadAll();
            IsLoaded = true;
        }

        private static void LoadAll()
        {
            if (!ConfigFile.ExistsNow())
            {
                Save();
                return;
            }

            var xml = LoadXml(ConfigFile);
            if (xml == null)
            {
                Save();
                return;
            }

            var versionAttribute = xml.Attribute("Version");
            var loadedVersion = versionAttribute?.Value;
            if (NeedsNewConfig(loadedVersion))
            {
                AllToDefault();
                Save();
                LoadLayouts(true);
                Mod.Warning($"Updating to version {Mod.Version} required your RimHUD config to be reset to default.");
                _configWasReset = true;
                return;
            }

            if (versionAttribute == null) { xml.Add(new XAttribute("Version", Mod.Version)); }
            else { versionAttribute.Value = Mod.Version; }

            LoadElements(typeof(Theme), xml);

            foreach (var integration in GetIntegrations()) { LoadClassElements(integration, xml); }

            LoadLayouts(false);

            xml.Save(ConfigFile.FullName);
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
                var option = propertyInfo.TryGetAttribute<Attributes.Option>();
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

            SaveLayouts();
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
                var option = propertyInfo.TryGetAttribute<Attributes.Option>();
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

        public static LayoutPreset[] BuildFixedPresetsList()
        {
            var list = new List<LayoutPreset>();
            foreach (var mod in LoadedModManager.RunningMods)
            {
                var directory = new DirectoryInfo(Path.Combine(mod.RootDir, FixedPresetsDirectoryName));
                if (!directory.Exists) { continue; }

                list.AddRange(directory.GetFiles("*" + PresetExtension).Select(file => LayoutPreset.Prepare(mod, file)).Where(preset => preset != null));
            }

            return list.ToArray();
        }

        public static LayoutPreset[] BuildUserPresetsList()
        {
            var directory = new DirectoryInfo(Path.Combine(Mod.ConfigDirectory.FullName, UserPresetsDirectoryName));
            return directory.Exists ? directory.GetFiles("*" + PresetExtension).Select(file => LayoutPreset.Prepare(null, file)).Where(preset => preset != null).ToArray() : new LayoutPreset[] { };
        }

        private static void LoadLayouts(bool reset)
        {
            var docked = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.DockedFileName));
            var floating = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.FloatingFileName));

            if (!reset && docked.Exists && LoadXml(docked) is XElement dockedXe) { HudLayout.Docked = HudLayout.FromXml(dockedXe); }
            else { HudLayout.Docked.ToXml().Save(docked.FullName); }

            if (!reset && floating.Exists && LoadXml(floating) is XElement floatingXe) { HudLayout.Floating = HudLayout.FromXml(floatingXe); }
            else { HudLayout.Floating.ToXml().Save(floating.FullName); }
        }

        public static void SaveLayouts()
        {
            var docked = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.DockedFileName));
            var floating = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, HudLayout.FloatingFileName));

            HudLayout.Docked.ToXml().Save(docked.FullName);
            HudLayout.Floating.ToXml().Save(floating.FullName);
        }

        public static void SaveLayoutPreset(string name, XElement xe)
        {
            if (!UserPresetDirectory.ExistsNow()) { UserPresetDirectory.Create(); }
            xe.Save(Path.Combine(UserPresetDirectory.FullName, name + PresetExtension));
        }

        public static void DeleteLayoutPreset(LayoutPreset preset)
        {
            if (!preset.File.ExistsNow()) { return; }
            preset.File.Delete();
        }

        public static void OpenConfigFolder() => Process.Start(Mod.ConfigDirectory.FullName);
    }
}
