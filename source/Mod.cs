using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Integration;
using UnityEngine;
using Verse;

namespace RimHUD;

public sealed class Mod : Verse.Mod
{
  public const string Id = "RimHUD";
  public const string Name = Id;
  public const string Version = "1.17.2";

  public const string MinConfigVersion = "1.17.0";

  public const string WorkshopLink = "https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027";

  private static Mod? _instance;

  public static bool DevMode { get; set; }
  public static ModContentPack ContentPack => _instance!.Content;

  public Mod(ModContentPack content) : base(content)
  {
    _instance = this;

    var harmony = new Harmony(Id);
    harmony.PatchAll();

    Persistent.Initialize();

    Report.Log("Initialized");

    LongEventHandler.QueueLongEvent(() => OnStartup(harmony), "InitializingInterface", false, null);
  }

  private static void OnStartup(Harmony harmony)
  {
    try
    {
      Persistent.Load();
      Integrations.Load();
    }
    catch (Report.Exception exception)
    {
      Report.Error($"RimHUD was unable to initialize properly due to the following exception:\n{exception}");
      State.Activated = false;
      harmony.UnpatchAll();
    }
  }

  public override string SettingsCategory() => Id;

  public override void DoSettingsWindowContents(Rect rect) => Tutorial.Presentation.OverlayModSettings(rect);
}
