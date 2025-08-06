using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using HarmonyLib;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog.Tabs;
using RimHUD.Interface.Hud.Layers;
using Verse;

namespace RimHUD.Configuration;

public static class Persistent
{
  public const int FilenameLengthMax = 250;

  private const string ConfigRootName = "Config";

  private const string VersionAttributeName = "Version";
  private const string PresetElementName = "Preset";

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

    return loadedVersion.NullOrWhitespace() || new Version(loadedVersion) < new Version(Mod.MinConfigVersion);
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

    var versionAttribute = root.Attribute(VersionAttributeName);
    var loadedVersion = versionAttribute?.Value;

    if (NeedsNewConfig(loadedVersion))
    {
      Reset(true);

      if (!_firstTimeUser) { Report.Warning($"Updating to version {Mod.Version} requires your {Mod.Name} config to be reset to default."); }

      _alertReset = true;
      return;
    }

    if (versionAttribute is null) { root.Add(new XAttribute(VersionAttributeName, Mod.Version)); }
    else { versionAttribute.Value = Mod.Version; }

    LoadElements(root, typeof(Theme));

    var preset = root.Element(PresetElementName)?.Value;
    if (preset is null) { Presets.LoadSavedOrDefault(); }
    else { Presets.Load(preset); }

    Save();
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
    var doc = new XDocument();

    var xml = SaveSettings(typeof(Theme), ConfigRootName);
    xml.Add(new XAttribute(VersionAttributeName, Mod.Version));

    if (Tutorial.IsComplete) { xml.Add(new XElement(Tutorial.CompleteElementName)); }

    if (Presets.Current is not null && !Presets.Current.IsDefault) { xml.Add(new XElement(PresetElementName, Presets.Current.Name)); }

    doc.Add(xml);

    doc.Save(ConfigFile.FullName);

    Presets.Save();
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
      var attribute = propertyInfo.TryGetAttribute<SettingAttribute>();
      if (attribute is null) { continue; }

      var propertyValue = propertyInfo.GetValue(instance, null);

      if (attribute.Label.NullOrEmpty())
      {
        if (attribute.Type is not null) { xml.Value = attribute.ConvertToXml(propertyValue); }
        return;
      }

      var element = new XElement(attribute.Label);

      if (attribute.Type is not null) { element.Value = attribute.ConvertToXml(propertyValue); }
      else
      {
        if (propertyValue is BaseSetting setting && setting.IsSaved()) { continue; }

        SaveElements(propertyValue, element);
      }

      if (element.IsEmpty) { continue; }
      if (attribute.Category is null) { xml.Add(element); }
      else
      {
        if (!categories.ContainsKey(attribute.Category)) { categories[attribute.Category] = new XElement(attribute.Category); }
        categories[attribute.Category]?.Add(element);
      }
    }

    if (categories.Count > 0) { xml.Add(categories.Values); }
  }

  public static void EnsureFilenameCase(string file)
  {
    if (!File.Exists(file)) { return; }

    var temp = file + "_TEMP";
    File.Move(file, temp);
    File.Move(temp, file);
  }

  public static void Reset(bool initializationStage = false)
  {
    SettingsToDefault();
    Save();

    Presets.LoadSavedOrDefault(true);
    if (!initializationStage) { Tab_ConfigContent.RefreshEditor(); }
  }

  public static void SettingsToDefault()
  {
    Theme.Settings.Do(static settings => settings.ToDefault());

    LayoutLayer.ResetToDefault();
  }

  public static void OpenConfigFolder() => Process.Start(ConfigDirectory.FullName);

  public static void ReportIfReset()
  {
    if (!_alertReset) { return; }
    _alertReset = false;

    var alert = Lang.Get("Interface.Alert.ConfigReset", Mod.Version);
    Report.Alert(alert);
  }
}
