using System;
using System.Linq;
using System.Reflection;
using RimHUD.Extensions;
using Verse;

namespace RimHUD.Compatibility
{
  public class ExternalMod
  {
    public string Name { get; }
    public Version VersionExpected { get; }
    public Version VersionFound { get; }
    public int VersionCompare => VersionExpected?.ComparePartial(VersionFound) ?? 0;

    public Assembly MainAssembly { get; }

    public virtual bool IsActive => MainAssembly != null;

    public ExternalMod(string name, string assemblyName, Version versionExpected = null)
    {
      Name = name;
      VersionExpected = versionExpected;

      var modContentPack = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.Name == name);
      if (modContentPack == null) { return; }

      MainAssembly = modContentPack.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
      if (MainAssembly == null)
      {
        Mod.Warning($"External mod '{Name}' does not contain expected main assembly");
        return;
      }

      VersionFound = MainAssembly.GetName().Version;

      if (VersionCompare != 0) { Mod.Log($"The loaded version of '{Name}' ({VersionFound}) is different from expected ({VersionExpected})"); }
    }
  }
}
