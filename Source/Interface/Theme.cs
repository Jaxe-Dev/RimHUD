using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    [StaticConstructorOnStartup]
    internal static class Theme
    {
        public const int BaseFontSize = 10;

        public static GUIStyle BaseFontStyle { get; set; } = new GUIStyle(Text.fontStyles[(int) GameFont.Medium]) { fontSize = BaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        public static Color BaseFontColor { get; set; } = new Color(0.9f, 0.9f, 0.9f);
        public static Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        public static Color LineColor { get; set; } = new Color(0.8f, 0.8f, 0.8f, 0.4f);

        public static Color FactionOwnColor { get; set; } = new Color(0.2f, 0.8f, 0.1f);
        public static Color FactionAlliedColor { get; set; } = new Color(0f, 0.5f, 1f);
        public static Color FactionIndependentColor { get; set; } = new Color(0.4f, 0.9f, 1f);
        public static Color FactionHostileColor { get; set; } = new Color(1f, 0.1f, 0f);
        public static Color FactionWildColor { get; set; } = new Color(0.8f, 0.5f, 0.2f);

        public static Color BarBackgroundColor => new Color(0.2f, 0.2f, 0.2f);
        public static Color BarMainColor { get; set; } = new Color(0.1f, 0.6f, 0f);
        public static Color BarLowColor { get; set; } = new Color(0.6f, 0f, 0.1f);

        public static Color CriticalColor { get; set; } = new Color(1f, 0.2f, 0.2f);
        public static Color WarningColor { get; set; } = new Color(1f, 0.9f, 0.1f);
        public static Color InfoColor { get; set; } = new Color(0.6f, 0.6f, 0.6f);
        public static Color GoodColor { get; set; } = new Color(0.4f, 0.8f, 0.8f);
        public static Color ExcellentColor { get; set; } = new Color(0.4f, 0.8f, 0.2f);

        public static Color MentalThresholdMinor { get; set; } = new Color(0.9f, 0.9f, 0.4f, 0.4f);
        public static Color MentalThresholdMajor { get; set; } = new Color(0.9f, 0.7f, 0.4f, 0.4f);
        public static Color MentalThresholdExtreme { get; set; } = new Color(0.9f, 0.4f, 0.4f, 0.4f);

        public static readonly Texture2D ToggleIcon = LoadTexture("ToggleIcon");

        private static Texture2D LoadTexture(string key) => ContentFinder<Texture2D>.Get(Mod.Id + "/" + key);

        private static Color HexColor(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : default(Color);
    }
}
