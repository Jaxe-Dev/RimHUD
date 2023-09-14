using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Layout;
using Verse;

namespace RimHUD.Configuration
{
  public static class Presets
  {
    private const string Extension = ".xml";

    private const string DirectoryName = "Presets";
    private const string ModDirectoryName = Mod.Id + "\\Presets";

    public static readonly DirectoryInfo UserPresetsDirectory = new(Path.Combine(Persistent.ConfigDirectory.FullName, DirectoryName));

    public static readonly Regex ValidFilenameRegex = new("^(?:[\\p{L}\\p{N}_\\-]|[\\p{L}\\p{N}_\\-]+[\\p{L}\\p{N}_\\- ]*[\\p{L}\\p{N}_\\-]+)$");

    public static void Load(bool reset)
    {
      var docked = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.DockedFileName));
      var floating = new FileInfo(Path.Combine(Persistent.ConfigDirectory.FullName, LayoutLayer.FloatingFileName));

      if (!reset && docked.Exists && Persistent.LoadXml(docked) is { } dockedXe) { LayoutLayer.Docked = LayoutLayer.FromXml(dockedXe); }
      else { LayoutLayer.Docked.ToXml().Save(docked.FullName); }

      if (!reset && floating.Exists && Persistent.LoadXml(floating) is { } floatingXe) { LayoutLayer.Floating = LayoutLayer.FromXml(floatingXe); }
      else { LayoutLayer.Floating.ToXml().Save(floating.FullName); }
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
      return directory.Exists ? directory.GetFiles($"*{Extension}").Select(static file => LayoutPreset.FromFile(null, file)).WhereNotNull().ToArray() : new LayoutPreset[] { };
    }
  }
}
