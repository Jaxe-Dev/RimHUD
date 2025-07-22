using System;
using System.Linq;
using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Extensions;
using Verse;

namespace RimHUD.Integration;

public abstract class IntegratedMod
{
  private readonly string _name;

  private readonly Type? _integrator;

  protected bool IsActive => Enabled && _integrator is not null;

  protected bool Enabled { get; private set; }

  protected Traverse Traverse => Traverse.Create(_integrator) ?? throw new Report.Exception($"Error creating Traverse integrator for '{_name}'.");

  protected IntegratedMod(string name, string assemblyName, string integratorName, Version? versionExpected = null)
  {
    _name = name;

    var modContentPack = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.Name == name);
    if (modContentPack is null) { return; }

    var mainAssembly = modContentPack.assemblies!.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
    if (mainAssembly is null)
    {
      Report.Warning($"Integrated mod '{name}' does not contain expected assembly.");
      return;
    }

    var versionFound = mainAssembly.GetName().Version;

    if (versionExpected is not null && versionExpected.ComparePartial(versionFound) is not 0) { Report.Log($"The loaded version of '{name}' ({versionFound}) is different from expected ({versionExpected})"); }

    _integrator = mainAssembly.GetType(integratorName);
    if (_integrator is null)
    {
      Report.Warning($"Integrator type '{integratorName}' not found for '{name}'.");
      return;
    }

    if (!IsActive) { return; }

    Enabled = true;
    Report.Log($"Integrated with {name}");
  }

  protected void DisableFrom(Exception exception)
  {
    Enabled = false;
    Report.Warning($"Integrated mod '{_name}' disabled due to the following exception:\n{exception.Message}.");
  }
}
