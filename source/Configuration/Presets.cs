using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

  public static readonly DirectoryInfo UserPresetsDirectory = new(Path.Combine(Persistent.ConfigDirectory.FullName, DirectoryName));

  public static readonly Regex ValidFilenameRegex = new(@"^(?:[\p{L}\p{N}_\-]|[\p{L}\p{N}_\-]+[\p{L}\p{N}_\- ]*[\p{L}\p{N}_\-]+)$");

  public static void Load(bool reset)
  {
    var docked = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
    var floating = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

    if (!reset && docked.Exists && Persistent.LoadXml(docked) is { } dockedXe) { LayoutLayer.Docked = LayoutLayer.FromXml(dockedXe); }
    else { LayoutLayer.Docked.ToXml().Save(docked.FullName); }

    if (!reset && floating.Exists && Persistent.LoadXml(floating) is { } floatingXe) { LayoutLayer.Floating = LayoutLayer.FromXml(floatingXe); }
    else { LayoutLayer.Floating.ToXml().Save(floating.FullName); }

    if (reset) { LayoutPreset.Active = LayoutPreset.DefaultName; }
  }

  public static void Load(string preset)
  {
    if (string.IsNullOrWhiteSpace(preset) || preset is LayoutPreset.DefaultName)
    {
      Load(true);
      return;
    }

    if (LoadedModManager.RunningMods is null) { return; }

    var integratedPreset = new FileInfo(Path.Combine(Mod.ContentPack.RootDir, DirectoryName, preset + Extension));
    if (integratedPreset.Exists && (LayoutPreset.FromFile(Mod.ContentPack, integratedPreset)?.Load() ?? false)) { return; }

    if ((from mod in LoadedModManager.RunningMods.Where(static mod => !mod.IsOfficialMod && mod != Mod.ContentPack) let modFile = new FileInfo(Path.Combine(mod.RootDir, ModDirectoryName, preset + Extension)) where modFile.Exists && (LayoutPreset.FromFile(mod, modFile)?.Load() ?? false) select mod).Any()) { return; }

    Load(true);
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
    if (!preset.IsUserMade || !preset.File.ExistsNow()) { return; }
    preset.File.Delete();
    LayoutPreset.RefreshList();
  }

  public static IEnumerable<LayoutPreset> GetIntegratedList()
  {
    var list = new List<LayoutPreset>();
    if (LoadedModManager.RunningMods is null) { return list.ToArray(); }

    foreach (var mod in LoadedModManager.RunningMods)
    {
      var directory = new DirectoryInfo(Path.Combine(mod.RootDir, mod == Mod.ContentPack ? DirectoryName : ModDirectoryName));
      if (!directory.Exists) { continue; }

      list.AddRange(directory.GetFiles($"*{Extension}").Select(file => LayoutPreset.FromFile(mod, file)).WhereNotNull());
    }

    return list.ToArray();
  }

  public static IEnumerable<LayoutPreset> GetUserList()
  {
    var directory = new DirectoryInfo(Path.Combine(Persistent.ConfigDirectory.FullName, DirectoryName));
    return directory.Exists ? directory.GetFiles($"*{Extension}").Select(static file => LayoutPreset.FromFile(null, file)).WhereNotNull().ToArray() : [];
  }
}
