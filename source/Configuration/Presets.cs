using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Layout;
using Verse;

namespace RimHUD.Configuration;

public static class Presets
{
  private const string Extension = ".xml";

  private const string DirectoryName = "Presets";
  private const string ModDirectoryName = Mod.Id + "\\Presets";

  private static readonly DirectoryInfo UserPresetsDirectory = new(Path.Combine(Persistent.ConfigDirectory.FullName, DirectoryName));

  private static readonly Regex ValidFilenameRegex = new(@"^(?:[\p{L}\p{N}_\-]|[\p{L}\p{N}_\-]+[\p{L}\p{N}_\- ]*[\p{L}\p{N}_\-]+)$");

  public static readonly string[] CoreNames =
  [
    "Compact",
    "Classic"
  ];

  static Presets()
  {
    var packaged = GetPackagedList().ToLookup(static preset => preset.Source == Mod.Name);

    CoreList = packaged[true].ToArray();
    PackagedList = packaged[false].ToArray();

    UserList = GetUserList();
  }

  public static string? Active { get; set; }

  public static IEnumerable<LayoutPreset> CoreList { get; }
  public static IEnumerable<LayoutPreset> PackagedList { get; }
  public static IEnumerable<LayoutPreset> UserList { get; private set; }

  public static void Load(bool reset)
  {
    var docked = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
    var floating = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

    if (!reset && docked.Exists && Persistent.LoadXml(docked) is { } dockedXe) { LayoutLayer.Docked = LayoutLayer.FromXml(dockedXe); }
    else { LayoutLayer.Docked.ToXml().Save(docked.FullName); }

    if (!reset && floating.Exists && Persistent.LoadXml(floating) is { } floatingXe) { LayoutLayer.Floating = LayoutLayer.FromXml(floatingXe); }
    else { LayoutLayer.Floating.ToXml().Save(floating.FullName); }

    if (reset) { Active = LayoutPreset.DefaultName; }
  }

  public static void Load(string preset)
  {
    if (string.IsNullOrWhiteSpace(preset) || preset is LayoutPreset.DefaultName)
    {
      Load(true);
      return;
    }

    if (TryLoadPreset(preset, Mod.ContentPack))
    {
      Report.Log($"Integrated preset '{preset}' loaded");
      return;
    }
    if (TryLoadPreset(preset, null))
    {
      Report.Log($"User preset '{preset}' loaded");
      return;
    }

    foreach (var mod in LoadedModManager.RunningMods.Where(static mod => !mod.IsOfficialMod && mod != Mod.ContentPack))
    {
      if (TryLoadPreset(preset, mod)) { Report.Log($"Mod preset '{preset}' loaded"); }
      return;
    }

    Load(true);
  }

  private static bool TryLoadPreset(string name, ModContentPack? mod)
  {
    var folder = mod is null ? UserPresetsDirectory.FullName : Path.Combine(mod.RootDir, mod == Mod.ContentPack ? DirectoryName : ModDirectoryName);

    var file = new FileInfo(Path.Combine(folder, name + Extension));
    return file.Exists && (LayoutPreset.FromFile(file, mod)?.Load() ?? false);
  }

  public static void Save()
  {
    var docked = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
    var floating = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

    LayoutLayer.Docked.ToXml().Save(docked.FullName);
    LayoutLayer.Floating.ToXml().Save(floating.FullName);
  }

  public static void Save(string name, XElement xml)
  {
    if (!UserPresetsDirectory.ExistsNow()) { UserPresetsDirectory.Create(); }
    xml.Save(Path.Combine(UserPresetsDirectory.FullName, name + Extension));
  }

  public static void Delete(LayoutPreset preset)
  {
    if (preset.Source is not null || !preset.File.ExistsNow()) { return; }
    preset.File.Delete();
    RefreshList();
  }

  private static IEnumerable<LayoutPreset> GetPackagedList()
  {
    var list = new List<LayoutPreset>();

    foreach (var mod in LoadedModManager.RunningMods.OrderBy(static mod => mod == Mod.ContentPack).ThenBy(static mod => mod.Name))
    {
      var directory = new DirectoryInfo(Path.Combine(mod.RootDir, mod == Mod.ContentPack ? DirectoryName : ModDirectoryName));
      if (!directory.Exists) { continue; }

      list.AddRange(directory.GetFiles($"*{Extension}").Select(file => LayoutPreset.FromFile(file, mod)).WhereNotNull());
    }

    return list.ToArray();
  }

  private static IEnumerable<LayoutPreset> GetUserList()
  {
    var directory = new DirectoryInfo(Path.Combine(Persistent.ConfigDirectory.FullName, DirectoryName));
    return directory.Exists ? directory.GetFiles($"*{Extension}").Select(static file => LayoutPreset.FromFile(file, null)).WhereNotNull().ToArray() : [];
  }

  public static void RefreshList() => UserList = GetUserList();

  public static bool IsValidFilename(string? name) => !name.NullOrWhitespace() && name!.Length <= Persistent.FilenameLengthMax - UserPresetsDirectory.FullName.Length && ValidFilenameRegex.IsMatch(name) && !CoreList.Concat(PackagedList).Any(preset => string.Equals(preset.Name, name, StringComparison.OrdinalIgnoreCase));
}
