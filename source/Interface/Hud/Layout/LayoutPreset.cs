using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using Verse;

namespace RimHUD.Interface.Hud.Layout;

public sealed class LayoutPreset
{
  public const string DefaultName = "Default";

  private const string RootElementName = "Preset";
  private const string VersionAttributeName = "Version";
  private const string RequiresAttributeName = "Requires";
  private const string TextSizesAttributeName = "TextSizes";

  public static string? Active { get; set; }

  public static IEnumerable<LayoutPreset> IntegratedList { get; } = Presets.GetIntegratedList();
  public static IEnumerable<LayoutPreset> UserList { get; private set; } = Presets.GetUserList();

  public FileInfo File { get; }

  public string Name { get; }

  private readonly string? _source;

  public string Label => $"{Name} [{_source?.SmallSize()}]";

  public bool IsUserMade => _source is null;

  private LayoutPreset(FileInfo file, string name, string? source)
  {
    File = file;
    Name = name;
    _source = source;
  }

  public static LayoutPreset? FromFile(ModContentPack? mod, FileInfo file)
  {
    var isIntegrated = mod == Mod.ContentPack;
    var source = isIntegrated ? Lang.Get("Layout.IntegratedPreset") : mod?.Name ?? Lang.Get("Layout.UserPreset");
    var name = file.NameWithoutExtension();

    var xml = Persistent.LoadXml(file);
    if (xml is null)
    {
      Report.Warning($"'{file.FullName}' is an invalid {Mod.Name} preset file.");
      return null;
    }

    var versionText = xml.GetAttribute(VersionAttributeName);
    var requires = xml.GetAttribute(RequiresAttributeName);

    if (requires is not null && !requires.Split('|', StringSplitOptions.RemoveEmptyEntries).Any(static require => ModLister.GetActiveModWithIdentifier(require) is not null)) { return null; }

    var preset = new LayoutPreset(file, name, source);
    if (isIntegrated) { return preset; }

    if (versionText is null)
    {
      Report.Warning($"{name} {source} is does not contain a version check.");
      return preset;
    }
    var version = new Version(versionText);
    if (new Version(Mod.Version).ComparePartial(version) is 1) { Report.Warning($"{name} {source} was built for an older version of {Mod.Name} ({versionText})."); }

    return preset;
  }

  public static void SaveCurrent(string name, bool includeDocked, bool includeFloating, bool includeWidth, bool includeHeight, bool includeTabs, bool includeTextSizes)
  {
    var xml = new XElement(RootElementName);

    xml.Add(new XAttribute(VersionAttributeName, Mod.Version));
    if (includeTextSizes) { xml.AddAttribute(TextSizesAttributeName, TextStyle.GetSizesString()); }

    if (includeDocked) { xml.Add(LayoutLayer.Docked.ToXml(LayoutLayer.DockedElementName, includeWidth ? Theme.InspectPaneTabWidth.Value : -1, includeHeight ? Theme.InspectPaneHeight.Value : -1, includeTabs ? Theme.InspectPaneMinTabs.Value : -1)); }
    if (includeFloating) { xml.Add(LayoutLayer.Floating.ToXml(LayoutLayer.FloatingElementName, includeWidth ? Theme.FloatingWidth.Value : -1, includeHeight ? Theme.FloatingHeight.Value : -1)); }

    Presets.Save(name, xml);
    RefreshList();
  }

  public static void RefreshList() => UserList = Presets.GetUserList();

  public bool Load()
  {
    if (!File.ExistsNow())
    {
      Report.Error($"Preset file '{Label}' not found.");
      return false;
    }

    var xml = Persistent.LoadXml(File);
    if (xml is null)
    {
      Report.Error($"Unable to load preset '{Label}'.");
      return false;
    }

    TextStyle.SetFromString(xml.GetAttribute(TextSizesAttributeName));

    var docked = xml.Element(LayoutLayer.DockedElementName);
    var floating = xml.Element(LayoutLayer.FloatingElementName);

    if (docked is not null) { LayoutLayer.Docked = LayoutLayer.FromXml(docked); }
    if (floating is not null) { LayoutLayer.Floating = LayoutLayer.FromXml(floating); }

    Active = Name;

    Persistent.Save();

    return true;
  }
}
