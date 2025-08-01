using System;
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

  public FileInfo File { get; }

  public string Name { get; }

  public string? Source { get; }

  public string Label => $"{Name} [{(Source ?? Lang.Get("Layout.UserPreset")).SmallSize()}]";

  private LayoutPreset(FileInfo file, string name, string? source)
  {
    File = file;
    Name = name;
    Source = source;
  }

  public static LayoutPreset? FromFile(FileInfo file, ModContentPack? mod)
  {
    var name = file.NameWithoutExtension();
    var isCore = Presets.CoreNames.Contains(name);
    var isIntegrated = mod == Mod.ContentPack;
    var source = isCore ? Mod.Name : isIntegrated ? Lang.Get("Layout.IntegratedPreset") : mod?.Name;

    var xml = Persistent.LoadXml(file);
    if (xml is null)
    {
      Report.Warning($"'{file.FullName}' is an invalid {Mod.Name} preset file.");
      return null;
    }

    var requires = xml.GetAttribute(RequiresAttributeName);
    if (requires is not null && !requires.Split('|', StringSplitOptions.RemoveEmptyEntries).Any(static require => ModLister.GetActiveModWithIdentifier(require) is not null)) { return null; }

    var preset = new LayoutPreset(file, name, source);
    if (isIntegrated) { return preset; }

    var versionText = xml.GetAttribute(VersionAttributeName);
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
    Presets.RefreshList();
  }

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

    Presets.Active = Name;

    Persistent.Save();

    return true;
  }
}
