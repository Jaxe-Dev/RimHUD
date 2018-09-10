using Harmony;
using Verse;

namespace RimHUD
{
    [StaticConstructorOnStartup]
    internal static class Mod
    {
        public const string Id = "RimHUD";
        public const string Name = "RimHUD";
        public const string Author = "Jaxe";
        public const string Version = "1.0.0";

        static Mod()
        {
            HarmonyInstance.Create(Id).PatchAll();
            Log("Loaded");
        }

        public static void Log(string message) => Verse.Log.Message($"[{Name} v{Version}] {message}");

        public class Exception : System.Exception
        {
            public Exception(string message) : base($"[{Name} : EXCEPTION] {message}")
            { }
        }
    }
}
