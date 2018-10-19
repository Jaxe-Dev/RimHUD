using System.IO;
using System.Reflection;
using Harmony;
using RimHUD.Compatibility;
using RimHUD.Data;
using RimHUD.Integration;
using RimWorld;
using Verse;

namespace RimHUD
{
    [StaticConstructorOnStartup]
    internal static class Mod
    {
        public const string Id = "RimHUD";
        public const string Name = Id;
        public const string Version = "1.2.9.1";

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
        public static bool FirstTimeUser { get; }

        public static readonly Assembly Assembly;
        public static readonly HarmonyInstance Harmony;

        static Mod()
        {
            Assembly = Assembly.GetExecutingAssembly();
            Harmony = HarmonyInstance.Create(Id);
            Harmony.PatchAll();

            FirstTimeUser = !ConfigDirectory.Exists;
            ConfigDirectory.Create();

            Persistent.Load();

            if (!FirstTimeUser) { HugsLib.RegisterUpdateFeature(); }

            Log("Initialized");
        }

        public static void OnStartup() => CompatibilityManager.OnStartup();
        public static void OnEnteredGame() => Persistent.CheckAlerts();

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);
        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

        public class Exception : System.Exception
        {
            public Exception(string message) : base(PrefixMessage(message))
            { }
        }
    }
}
