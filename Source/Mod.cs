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
using RimHUD.Interface.Dialog;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using RimWorld;
using Verse;

namespace RimHUD
{
    [StaticConstructorOnStartup]
    public static class Mod
    {
        public const string Id = "RimHUD";
        public const string Name = Id;
        public const string Version = "1.6.0";
        public const bool VersionNeedsNewConfig = false;

        public static IEnumerable<string> SameConfigVersions { get; } = new[]
        {
            "1.6"
        };

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));
        public static readonly ModContentPack ContentPack;

        public static bool FirstTimeUser { get; }

        public static readonly Assembly Assembly;
        public static readonly Harmony Harmony;

        static Mod()
        {
            Assembly = Assembly.GetExecutingAssembly();
            Harmony = new Harmony(Id);
            Harmony.PatchAll();

            FirstTimeUser = !ConfigDirectory.Exists;
            ContentPack = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.Contains(Assembly));
            ConfigDirectory.Create();

            if (!FirstTimeUser) { Mod_HugsLib.TryRegisterUpdateFeature(); }

            Log("Initialized");

            LongEventHandler.QueueLongEvent(OnStartup, "InitializingInterface", false, null);
        }

        public static void OnStartup()
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
                var info = new ExceptionInfo(exception);
                Error("RimHUD was unable to initialize properly due to the following exception:\n" + info.Text);
                State.Activated = false;
                Harmony.UnpatchAll();
            }
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

        public static void HandleError(System.Exception exception)
        {
            State.Activated = false;
            Dialog_Error.Open(new ExceptionInfo(exception));
        }

        public class Exception : System.Exception
        {
            public Exception(string message, System.Exception innerException = null) : base(message, innerException) { }
        }

        public class ExceptionInfo
        {
            public string Message { get; }
            public string StackTrace { get; }
            public bool IsExternalError { get; }
            public string PossibleMod { get; }
            public string Text => Message + "\n\nStacktrace:\n" + StackTrace;

            public ExceptionInfo(System.Exception exception)
            {
                Message = exception.Message;
                StackTrace = BuildStacktrace(exception);

                IsExternalError = !string.Equals(exception.Source, Id);
                PossibleMod = IsExternalError ? LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies.loadedAssemblies.Any(assembly => assembly.GetName().Name == exception.Source))?.Name : null;

                if (exception.InnerException == null) { return; }

                var innerException = exception.InnerException;
                var level = 1;
                do
                {
                    Message += $"\n{new string('+', level)} [{innerException.Source}] {innerException.Message}";
                    StackTrace += "\n\n" + BuildStacktrace(innerException);
                    level++;
                    innerException = innerException.InnerException;
                }
                while (innerException != null);
            }

            private static string BuildStacktrace(System.Exception exception) => $"[{exception.Source}: {exception.Message}]\n{exception.StackTrace}";
        }
    }
}
