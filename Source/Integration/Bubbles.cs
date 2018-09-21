using System;
using System.Reflection;
using Harmony;

namespace RimHUD.Integration
{
    internal static class Bubbles
    {
        public const string Url = "https://steamcommunity.com/sharedfiles/filedetails/?id=1516158345";
        public const string Description = "Shows bubbles when characters perform a social interaction with the text that would normally only be found in the log.";

        private static readonly Assembly Assembly = Union.GetModAssembly("Bubbles");
        private static readonly Type Integrator = Assembly?.GetType("Bubbles.Interface.Bubbler");

        public static readonly bool IsLoaded = Integrator != null;

        private static void SetValue(string name, object value)
        {
            if (!IsLoaded) { return; }
            AccessTools.Property(Integrator, name).SetValue(null, value, null);
        }

        public static void SetFadeStart(int value) => SetValue("FadeStart", value);
        public static void SetFadeTime(int value) => SetValue("FadeTime", value);
        public static void SetWidth(int value) => SetValue("BubbleWidth", value);
        public static void SetPadding(int value) => SetValue("BubblePadding", value);
        public static void SetMaxPerPawn(int value) => SetValue("MaxBubblesPerPawn", value);
    }
}
