using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using RimHUD.Compatibility;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD
{
  public class Mod : Verse.Mod
  {
    public const string Id = "RimHUD";
    public const string Name = Id;
    public const string Version = "1.13.0";

    public const string PackageId = "Jaxe.RimHUD";
    public const string WorkshopLink = "https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027";

    public static IEnumerable<string> SameConfigVersions { get; } = new[]
    {
      "1.12"
    };

    public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));

    public static Mod Instance { get; private set; }

    public static ModContentPack ContentPack => Instance.Content;

    public static bool DevMode { get; set; }

    public static bool FirstTimeUser { get; private set; }

    public static Assembly Assembly { get; private set; }

    public Mod(ModContentPack content) : base(content)
    {
      Instance = this;

      Assembly = Assembly.GetExecutingAssembly();

      var harmony = new Harmony(Id);
      harmony.PatchAll();

      FirstTimeUser = !ConfigDirectory.Exists;
      ConfigDirectory.Create();

      IntegrationManager.EarlyInitialization();

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

        IntegrationManager.FinalInitialization();
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
      LayoutLayer.Docked.Flush();
      LayoutLayer.Floating.Flush();
    }

    public override string SettingsCategory() => Name;

    public override void DoSettingsWindowContents(Rect rect) => Tutorial.OverlayModSettings(rect);

    public class Exception : System.Exception
    {
      public Exception(string message, System.Exception innerException = null) : base(message, innerException)
      { }
    }
  }
}
