using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Layout;
using Verse;
using Verse.Steam;

namespace RimHUD.Configuration
{
  public static class Persistent
  {
    private const string ConfigFileName = "Config.xml";
    private const string ModPresetsDirectoryName = "RimHUD\\Presets";
    private const string PresetsDirectoryName = "Presets";
    private const string PresetExtension = ".xml";

    private const string AboutDirectoryName = "About";
    private const string CreditsFileName = "Credits.xml";

    private static readonly Regex ValidFilenameRegex = new Regex("^(?:[\\p{L}\\p{N}_\\-]|[\\p{L}\\p{N}_\\-]+[\\p{L}\\p{N}_\\- ]*[\\p{L}\\p{N}_\\-]+)$");
    private static readonly DirectoryInfo UserPresetDirectory = new DirectoryInfo(Path.Combine(Mod.ConfigDirectory.FullName, PresetsDirectoryName));

    public static XElement Credits { get; private set; }

    public static bool HasCredits => Credits != null;

    public static bool IsLoaded { get; private set; }

    public static bool TutorialComplete { get; private set; }

    private static bool _configWasReset;

    private static readonly FileInfo ConfigFile = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, ConfigFileName));

    public static bool IsValidFilename(string name) => !name.NullOrEmpty() && name.Length <= 250 - UserPresetDirectory.FullName.Length && ValidFilenameRegex.IsMatch(name);

    public static void OpenConfigFolder() => Process.Start(Mod.ConfigDirectory.FullName);

    private static bool NeedsNewConfig(string loadedVersion)
    {
      if (loadedVersion == Mod.Version) { return false; }
      Mod.Warning($"Loaded config version ({loadedVersion ?? "NULL"}) is different from the current mod version");

      return string.IsNullOrEmpty(loadedVersion) || Mod.SameConfigVersions.All(version => !loadedVersion.StartsWith(version));
    }

    public static void CheckAlerts()
    {
      if (!_configWasReset) { return; }

      var alert = Lang.Get("Interface.Alert.ConfigReset", Mod.Version);
      Mod.Message(alert);
    }

    public static void AllToDefault()
    {
      SetToDefault(typeof(Theme));

      foreach (var integration in GetIntegratedOptions()) { SetToDefault(integration); }

      LayoutLayer.LoadDefaultAndSave();
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

    public static void Load()
    {
      LoadAll();
      IsLoaded = true;
    }

    private static void LoadAll()
    {
      LoadCredits();

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

      TutorialComplete = xml.Element("TutorialComplete") != null;

      var versionAttribute = xml.Attribute("Version");
      var loadedVersion = versionAttribute?.Value;
      if (NeedsNewConfig(loadedVersion))
      {
        ResetToDefault();

        if (!Mod.FirstTimeUser) { Mod.Warning($"Updating to version {Mod.Version} requires your RimHUD config to be reset to default."); }

        _configWasReset = true;
        return;
      }

      if (versionAttribute == null) { xml.Add(new XAttribute("Version", Mod.Version)); }
      else { versionAttribute.Value = Mod.Version; }

      LoadElements(typeof(Theme), xml);

      foreach (var integration in GetIntegratedOptions()) { LoadClassElements(integration, xml); }

      LoadLayouts(false);

      xml.Save(ConfigFile.FullName);
    }

    private static void ResetToDefault()
    {
      AllToDefault();
      Save();
      LoadLayouts(true);
    }

    public static XElement LoadXml(FileInfo file, bool resetOnFail = false)
    {
      try { return XDocument.Load(file.FullName).Root; }
      catch (Exception ex)
      {
        var message = $"Failed to load xml file '{file.FullName}' due to exception '{ex.Message}'";
        if (resetOnFail)
        {
          Mod.Warning(message + " and your config will be reset to default.");
          ResetToDefault();
        }
        else { Mod.Warning(message); }

        return null;
      }
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

      if (TutorialComplete) { theme.Add(new XElement("TutorialComplete")); }

      foreach (var integration in GetIntegratedOptions())
      {
        var integrationElement = SaveClassElements(integration);
        if (!integrationElement.IsEmpty) { theme.Add(integrationElement); }
      }

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

    public static void LoadLayouts(bool reset)
    {
      var docked = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
      var floating = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

      if (!reset && docked.Exists && LoadXml(docked) is XElement dockedXe) { LayoutLayer.Docked = LayoutLayer.FromXml(dockedXe); }
      else { LayoutLayer.Docked.ToXml().Save(docked.FullName); }

      if (!reset && floating.Exists && LoadXml(floating) is XElement floatingXe) { LayoutLayer.Floating = LayoutLayer.FromXml(floatingXe); }
      else { LayoutLayer.Floating.ToXml().Save(floating.FullName); }
    }

    public static void SaveLayouts()
    {
      var docked = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
      var floating = new FileInfo(Path.Combine(Mod.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

      LayoutLayer.Docked.ToXml().Save(docked.FullName);
      LayoutLayer.Floating.ToXml().Save(floating.FullName);
    }

    public static void SaveCurrentLayouts(string name, bool includeDocked, bool includeFloating, bool includeWidth, bool includeHeight, bool includeTabs)
    {
      var xe = new XElement(LayoutPreset.RootElementName);

      xe.Add(new XAttribute(LayoutPreset.VersionAttributeName, Mod.Version));

      if (includeDocked) { xe.Add(LayoutLayer.Docked.ToXml(LayoutLayer.DockedElementName, includeWidth ? Theme.InspectPaneTabWidth.Value : -1, includeHeight ? Theme.InspectPaneHeight.Value : -1, includeTabs ? Theme.InspectPaneMinTabs.Value : -1)); }
      if (includeFloating) { xe.Add(LayoutLayer.Floating.ToXml(LayoutLayer.FloatingElementName, includeWidth ? Theme.HudWidth.Value : -1, includeHeight ? Theme.HudHeight.Value : -1)); }

      if (!UserPresetDirectory.ExistsNow()) { UserPresetDirectory.Create(); }
      xe.Save(Path.Combine(UserPresetDirectory.FullName, name + PresetExtension));
      LayoutPreset.RefreshUserPresets();
    }

    public static void DeleteLayoutPreset(LayoutPreset preset)
    {
      if (!preset.File.ExistsNow()) { return; }
      preset.File.Delete();
    }

    public static LayoutPreset[] GetFixedPresets()
    {
      var list = new List<LayoutPreset>();
      foreach (var mod in LoadedModManager.RunningMods)
      {
        var directory = new DirectoryInfo(Path.Combine(mod.RootDir, mod == Mod.ContentPack ? PresetsDirectoryName : ModPresetsDirectoryName));
        if (!directory.Exists) { continue; }

        list.AddRange(directory.GetFiles("*" + PresetExtension).Select(file => LayoutPreset.Prepare(mod, file)).Where(preset => preset != null));
      }

      return list.ToArray();
    }

    public static LayoutPreset[] GetUserPresets()
    {
      var directory = new DirectoryInfo(Path.Combine(Mod.ConfigDirectory.FullName, PresetsDirectoryName));
      return directory.Exists ? directory.GetFiles("*" + PresetExtension).Select(file => LayoutPreset.Prepare(null, file)).Where(preset => preset != null).ToArray() : new LayoutPreset[] { };
    }

    private static void LoadCredits()
    {
      var file = new FileInfo(Path.Combine(Mod.ContentPack.RootDir, AboutDirectoryName, CreditsFileName));

      try { Credits = XDocument.Load(file.FullName).Root; }
      catch
      {
        if (SteamManager.Initialized) { Mod.Warning("Unable to load credits. This may be an unofficial version of the mod"); }
      }
    }

    private static Type GetSubjectType(object subject)
    {
      if (subject is Type type) { return type; }
      return subject.GetType();
    }

    private static IEnumerable<Type> GetIntegratedOptions() => Assembly.GetExecutingAssembly().GetTypes().Where(type => type.HasAttribute<Attributes.IntegratedOptions>());

    private static string GetIntegrationName(object subject) => "Integration." + GetSubjectType(subject).Name;

    public static void FinalizeIntegratedOptions()
    {
      foreach (var integration in GetIntegratedOptions())
      {
        foreach (var property in integration.GetProperties())
        {
          var attribute = property.TryGetAttribute<Attributes.Option>();
          if (attribute == null) { continue; }

          var option = property.GetValue(integration, null) as ThemeOption;
          option?.Refresh();
        }
      }
    }

    public static void CompleteTutorial()
    {
      TutorialComplete = true;
      Save();
    }

    public static void ResetTutorial()
    {
      Theme.HudDocked.Value = true;
      TutorialComplete = false;
      Save();
    }
  }
}
