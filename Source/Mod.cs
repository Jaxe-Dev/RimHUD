using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using Multiplayer.API;
using RimHUD.Data.Compatibility;
using RimHUD.Data.Integration;
using RimHUD.Data.Storage;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using RimWorld;
using Verse;

namespace RimHUD
{
    [StaticConstructorOnStartup]
    public static class Mod
    {
        public const string Id = "RimHUD";
        public const string Name = Id;
        public const string Version = "1.5.0";
        public const bool VersionNeedsNewConfig = false;

        public static readonly string[] SameConfigVersions =
        {
            "1.4.0",
            "1.4.1",
            "1.4.2",
            "1.4.3",
            "1.4.4"
        };

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
        public static readonly ModContentPack ContentPack;

        public static bool FirstTimeUser { get; }

        public static readonly Assembly Assembly;
        public static readonly HarmonyInstance Harmony;

        static Mod()
        {
            Assembly = Assembly.GetExecutingAssembly();
            Harmony = HarmonyInstance.Create(Id);
            Harmony.PatchAll();

            FirstTimeUser = !ConfigDirectory.Exists;
            ContentPack = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.Contains(Assembly));
            ConfigDirectory.Create();

            if (!FirstTimeUser) { HugsLib.RegisterUpdateFeature(); }

            Log("Initialized");

            if (MP.enabled)
            {
                MP.RegisterAll();
                Log("Multiplayer ready with API " + MP.API);
            }
        }

        public static void OnStartup()
        {
            Persistent.Load();
            CompatibilityManager.OnStartup();
        }

        public static void OnEnteredGame() => Persistent.CheckAlerts();

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
        public static void ErrorOnce(string message, string key) => Verse.Log.ErrorOnce(PrefixMessage(message), key.GetHashCode());
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
            public Exception(string message, System.Exception innerException = null) : base(message, innerException) { }
        }
    }
}
