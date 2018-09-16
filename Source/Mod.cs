using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using RimHUD.Data;
using RimWorld;
using Verse;

namespace RimHUD
{
    [StaticConstructorOnStartup]
    internal static class Mod
    {
        public const string Id = "RimHUD";
        public const string Name = Id;
        public const string Version = "1.0.1";

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
        public static bool FirstTimeUser { get; }

        static Mod()
        {
            HarmonyInstance.Create(Id).PatchAll();

            FirstTimeUser = !ConfigDirectory.Exists;
            ConfigDirectory.Create();

            //if (!FirstTimeUser) { TryRegisterHugsLibUpdateFeature(); }
            TryRegisterHugsLibUpdateFeature(); // FOR THIS VERSION ONLY

            Persistent.Load();

            Log("Initialized");
        }

        private static void TryRegisterHugsLibUpdateFeature()
        {
            var hugsLib = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Name == "HugsLibController" select type).FirstOrDefault();
            if (hugsLib == null) { return; }

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Traverse.Create(hugsLib).Field("instance")?.Property("UpdateFeatures")?.Method("InspectActiveMod", Id, version)?.GetValue();
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
