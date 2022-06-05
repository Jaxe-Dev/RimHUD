using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimHUD.Data;
using RimHUD.Data.Compatibility;
using RimHUD.Data.Integration;
using RimHUD.Data.Storage;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using RimWorld;
using Verse;

namespace RimHUD
{
  [StaticConstructorOnStartup]
  internal static class Mod
  {
    public const string Id = "RimHUD";
    public const string Name = Id;
    public const string Version = "1.10.2";

    public const string PackageId = "Jaxe.RimHUD";
    public const string WorkshopLink = "https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027";

    public static IEnumerable<string> SameConfigVersions { get; } = new[]
    {
      "1.10"
    };

    public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
    public static readonly ModContentPack ContentPack;

    public static bool DevMode { get; set; }

    public static bool FirstTimeUser { get; }

    public static readonly Assembly Assembly;

    static Mod()
    {
      Assembly = Assembly.GetExecutingAssembly();
      var harmony = new Harmony(Id);
      harmony.PatchAll();

      FirstTimeUser = !ConfigDirectory.Exists;
      ContentPack = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.Contains(Assembly));
      ConfigDirectory.Create();

      Log("Initialized");

      LongEventHandler.QueueLongEvent(() => OnStartup(harmony), "InitializingInterface", false, null);
    }

    private static void OnStartup(Harmony harmony)
    {
      try
      {
        Persistent.Load();
        Access.Initialize();
        Textures.Initialize();

        IntegrationManager.Initialize();
        CompatibilityManager.Initialize();
      }
      catch (System.Exception exception)
      {
        var info = new Troubleshooter.ExceptionInfo(exception);
        Error("RimHUD was unable to initialize properly due to the following exception:\n" + info.Text);
        State.Activated = false;
        harmony.UnpatchAll();
      }
    }

    public static void OnEnteredGame() => Persistent.CheckAlerts();

    public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
    public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
    public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
    public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);
    private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

    public static void ClearCache()
    {
      InspectPanePlus.ClearCache();
      HudLayout.Docked.Flush();
      HudLayout.Floating.Flush();
    }

    public class Exception : System.Exception
    {
      public Exception(string message, System.Exception innerException = null) : base(message, innerException)
      { }
    }
  }
}
