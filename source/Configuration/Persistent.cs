using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using HarmonyLib;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog.Tabs;
using RimHUD.Interface.Hud.Layers;
using Verse;

namespace RimHUD.Configuration
{
  public static class Persistent
  {
    private const string ConfigRootName = "Config";

    private const int FilenameLengthMax = 250;

    public static readonly DirectoryInfo ConfigDirectory = new(Path.Combine(GenFilePaths.ConfigFolderPath, Mod.Id));
    private static readonly FileInfo ConfigFile = new(Path.Combine(ConfigDirectory.FullName, "Config.xml"));

    private static bool _firstTimeUser;

    private static bool _alertReset;

    public static void Initialize()
    {
      _firstTimeUser = !ConfigDirectory.Exists;
      ConfigDirectory.Create();
    }

    private static bool NeedsNewConfig(string? loadedVersion)
    {
      if (loadedVersion is Mod.Version) { return false; }
      Report.Warning($"Loaded config version ({loadedVersion ?? "NULL"}) is different from the current mod version.");

      return loadedVersion.NullOrWhitespace() || Mod.AcceptedConfigVersions.All(version => !loadedVersion.StartsWith(version));
    }

    public static void Load()
    {
      Credits.Load();

      if (!ConfigFile.ExistsNow())
      {
        Save();
        return;
      }

      var root = LoadXml(ConfigFile);
      if (root is null)
      {
        Save();
        return;
      }

      Tutorial.Initialize(root);

      var versionAttribute = root.Attribute("Version");
      var loadedVersion = versionAttribute?.Value;

      if (NeedsNewConfig(loadedVersion))
      {
        Reset(true);

        if (!_firstTimeUser) { Report.Warning($"Updating to version {Mod.Version} requires your {Mod.Name} config to be reset to default."); }

        _alertReset = true;
        return;
      }

      if (versionAttribute is null) { root.Add(new XAttribute("Version", Mod.Version)); }
      else { versionAttribute.Value = Mod.Version; }

      LoadElements(root, typeof(Theme));

      Presets.Load(false);

      root.Save(ConfigFile.FullName);
    }

    public static XElement? LoadXml(FileInfo file, bool resetOnFail = false)
    {
      try { return XDocument.Load(file.FullName).Root; }
      catch (Exception exception)
      {
        Report.Warning($"Failed to load xml file '{file.FullName}' due to exception '{exception.Message}'{(resetOnFail ? " and your config will be reset to default." : null)}");
        if (resetOnFail) { Reset(true); }

        return null;
      }
    }

    private static void LoadElements(XElement xml, object subject)
    {
      var type = subject as Type ?? subject.GetType();
      var instance = subject is Type ? null : subject;

      foreach (var propertyInfo in type.GetProperties())
      {
        var settings = propertyInfo.TryGetAttribute<SettingAttribute>();
        if (settings is null) { continue; }

        var propertyValue = propertyInfo.GetValue(instance, null);

        if (settings.Label.NullOrEmpty())
        {
          if (settings.Type is null) { return; }

          var value = settings.ConvertFromXml(xml.Value);
          if (value is not null) { propertyInfo.SetValue(instance, value, null); }
          return;
        }

        var label = settings.Label;
        var category = settings.Category is null ? xml : xml.Element(settings.Category);
        var element = category?.Element(label);

        if (element is null) { continue; }

        LoadElements(element, propertyValue);
      }
    }

    public static void Save()
    {
      SaveXml();

      Presets.Save();
    }

    private static void SaveXml()
    {
      var doc = new XDocument();

      var xml = SaveSettings(typeof(Theme), ConfigRootName);
      xml.Add(new XAttribute("Version", Mod.Version));

      if (Tutorial.IsComplete) { xml.Add(new XElement("TutorialComplete")); }

      doc.Add(xml);

      doc.Save(ConfigFile.FullName);
    }

    private static XElement SaveSettings(Type type, string? name)
    {
      var element = new XElement(name);
      SaveElements(type, element);
      return element;
    }

    private static void SaveElements(object subject, XElement xml)
    {
      var type = subject as Type ?? subject.GetType();
      var instance = subject is Type ? null : subject;

      var categories = new Dictionary<string, XElement>();

      foreach (var propertyInfo in type.GetProperties())
      {
        var settings = propertyInfo.TryGetAttribute<SettingAttribute>();
        if (settings is null) { continue; }

        var propertyValue = propertyInfo.GetValue(instance, null);

        if (settings.Label.NullOrEmpty())
        {
          if (settings.Type is not null) { xml.Value = settings.ConvertToXml(propertyValue); }
          return;
        }

        var element = new XElement(settings.Label);

        if (settings.Type is not null) { element.Value = settings.ConvertToXml(propertyValue); }
        else { SaveElements(propertyValue, element); }

        if (element.IsEmpty) { continue; }
        if (settings.Category is null) { xml.Add(element); }
        else
        {
          if (!categories.ContainsKey(settings.Category)) { categories[settings.Category] = new XElement(settings.Category); }
          categories[settings.Category]?.Add(element);
        }
      }

      if (categories.Count > 0) { xml.Add(categories.Values); }
    }

    public static void Reset(bool initializationStage = false)
    {
      SettingsToDefault();
      Save();

      Presets.Load(true);
      if (!initializationStage) { Tab_ConfigContent.RefreshEditor(); }
    }

    public static void SettingsToDefault()
    {
      Theme.Settings.Do(static settings => settings.ToDefault());

      LayoutLayer.LoadDefaultAndSave();
    }

    public static void OpenConfigFolder() => Process.Start(ConfigDirectory.FullName);

    public static bool IsValidFilename(string? name) => !name.NullOrWhitespace() && name.Length <= FilenameLengthMax - Presets.UserPresetsDirectory.FullName.Length && Presets.ValidFilenameRegex.IsMatch(name);

    public static void ReportIfReset()
    {
      if (!_alertReset) { return; }

      var alert = Lang.Get("Interface.Alert.ConfigReset", Mod.Version);
      Report.Alert(alert);
    }
  }
}
