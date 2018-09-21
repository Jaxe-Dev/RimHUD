using System.IO;
using Harmony;
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
        public const string Version = "1.1.0";

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
        public static bool FirstTimeUser { get; }

        static Mod()
        {
            HarmonyInstance.Create(Id).PatchAll();

            FirstTimeUser = !ConfigDirectory.Exists;
            ConfigDirectory.Create();

            Persistent.Load();

            if (!FirstTimeUser) { HugsLib.RegisterUpdateFeature(); }

            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);
        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

        public class Exception : System.Exception
        {
            public Exception(string message) : base($"[{Name} : EXCEPTION] {message}")
            { }
        }
    }
}
