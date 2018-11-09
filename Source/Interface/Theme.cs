using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;
using ColorOption = RimHUD.Data.ColorOption;

namespace RimHUD.Interface
{
    internal static class Theme
    {
        private const int DefaultBaseFontSize = 12;

        public static GUIStyle BaseGUIStyle => new GUIStyle(Text.fontStyles[(int) GameFont.Medium]) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };

        [Persistent.Option("HudOptions", "RefreshRate")] public static RangeOption RefreshRate { get; } = new RangeOption(2, 0, 10, Lang.Get("Theme.RefreshRate"), value => (value * 100) + Lang.Get("Theme.RefreshRateUnits"), Lang.Get("Theme.RefreshRateDesc"));

        [Persistent.Option("HudPosition", "Docked")] public static BoolOption HudDocked { get; } = new BoolOption(true, Lang.Get("Theme.HudDocked"), Lang.Get("Theme.HudDockedDesc"), EnsureInspectPaneModify);
        [Persistent.Option("HudPosition", "Anchor")] public static RangeOption HudAnchor { get; } = new RangeOption(2, 0, 8, Lang.Get("Theme.HudAnchor"), value => Lang.GetIndexed("Theme.HudAnchors", value), onChange: _ => SetOffsetBounds());
        [Persistent.Option("HudPosition", "OffsetX")] public static RangeOption HudOffsetX { get; } = new RangeOption(0, -Screen.width, Screen.width, Lang.Get("Theme.HudOffsetX"));
        [Persistent.Option("HudPosition", "OffsetY")] public static RangeOption HudOffsetY { get; } = new RangeOption(0, -Screen.height, Screen.height, Lang.Get("Theme.HudOffsetY"));

        [Persistent.Option("HudDimensions", "Width")] public static RangeOption HudWidth { get; } = new RangeOption(280, 200, 600, Lang.Get("Theme.HudWidth"));
        [Persistent.Option("HudDimensions", "Height")] public static RangeOption HudHeight { get; } = new RangeOption(420, 300, 500, Lang.Get("Theme.HudHeight"));

        [Persistent.Option("InspectPane", "Modify")] public static BoolOption InspectPaneTabModify { get; } = new BoolOption(true, Lang.Get("Theme.InspectPaneModify"), Lang.Get("Theme.InspectPaneModifyDesc"), EnsureHudNotHidden);
        [Persistent.Option("InspectPane", "AddLog")] public static BoolOption InspectPaneTabAddLog { get; } = new BoolOption(true, Lang.Get("Theme.InspectPaneAddLog"), Lang.Get("Theme.InspectPaneAddLogDesc"));
        [Persistent.Option("InspectPane", "Height")] public static RangeOption InspectPaneHeight { get; } = new RangeOption(255, 200, 500, Lang.Get("Theme.InspectPaneHeight"));
        [Persistent.Option("InspectPane", "MinTabs")] public static RangeOption InspectPaneMinTabs { get; } = new RangeOption(7, 6, 12, Lang.Get("Theme.InspectPaneMinTabs"));
        [Persistent.Option("InspectPane", "TabWidth")] public static RangeOption InspectPaneTabWidth { get; } = new RangeOption(80, 72, 150, Lang.Get("Theme.InspectPaneTabWidth"));

        [Persistent.Option("Text", "Regular")] public static TextStyle RegularTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleRegular"), null, BaseGUIStyle.fontSize, 7, 20, 100, 100, 250, _ => UpdateTextStyles());
        [Persistent.Option("Text", "Large")] public static TextStyle LargeTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 3, 0, 5, 150, 100, 250);
        [Persistent.Option("Text", "Small")] public static TextStyle SmallTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250);
        [Persistent.Option("Text", "LabelWidth")] public static RangeOption LabelWidth { get; } = new RangeOption(95, 30, 150, Lang.Get("Theme.LabelWidth"));
        [Persistent.Option("Text", "ValueWidth")] public static RangeOption ValueWidth { get; } = new RangeOption(40, 30, 150, Lang.Get("Theme.ValueWidth"));
        [Persistent.Option("Text", "ShowDecimals")] public static BoolOption ShowDecimals { get; } = new BoolOption(true, Lang.Get("Theme.ShowDecimals"));

        [Persistent.Option("Letters", "Padding")] public static RangeOption LetterPadding { get; } = new RangeOption(4, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));
        [Persistent.Option("Letters", "Compress")] public static BoolOption LetterCompress { get; } = new BoolOption(true, Lang.Get("Theme.LetterCompress"), Lang.Get("Theme.LetterCompressDesc"));

        [Persistent.Option("HudColors", "MainText")] public static ColorOption MainTextColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.MainTextColor"));
        [Persistent.Option("HudColors", "Disabled")] public static ColorOption DisabledColor { get; } = new ColorOption(new Color(0.5f, 0.5f, 0.5f), Lang.Get("Theme.DisabledColor"));
        [Persistent.Option("HudColors", "Critical")] public static ColorOption CriticalColor { get; } = new ColorOption(new Color(1f, 0.2f, 0.2f), Lang.Get("Theme.CriticalColor"));
        [Persistent.Option("HudColors", "Warning")] public static ColorOption WarningColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.1f), Lang.Get("Theme.WarningColor"));
        [Persistent.Option("HudColors", "Info")] public static ColorOption InfoColor { get; } = new ColorOption(new Color(0.6f, 0.6f, 0.6f), Lang.Get("Theme.InfoColor"));
        [Persistent.Option("HudColors", "Good")] public static ColorOption GoodColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.8f), Lang.Get("Theme.GoodColor"));
        [Persistent.Option("HudColors", "Excellent")] public static ColorOption ExcellentColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.2f), Lang.Get("Theme.ExcellentColor"));
        [Persistent.Option("HudColors", "BarBackground")] public static ColorOption BarBackgroundColor { get; } = new ColorOption(new Color(0.2f, 0.2f, 0.2f), Lang.Get("Theme.BarBackgroundColor"));
        [Persistent.Option("HudColors", "BarMain")] public static ColorOption BarMainColor { get; } = new ColorOption(new Color(0.25f, 0.6f, 0f), Lang.Get("Theme.BarMainColor"));
        [Persistent.Option("HudColors", "BarLow")] public static ColorOption BarLowColor { get; } = new ColorOption(new Color(0.6f, 0f, 0f), Lang.Get("Theme.BarLowColor"));
        [Persistent.Option("HudColors", "BarThreshold")] public static ColorOption BarThresholdColor { get; } = new ColorOption(new Color(0f, 0f, 0f, 0.75f), Lang.Get("Theme.BarThresholdColor"));
        [Persistent.Option("HudColors", "SelectorText")] public static ColorOption SelectorTextColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.SelectorTextColor"));
        [Persistent.Option("HudColors", "SelectorBackground")] public static ColorOption SelectorBackgroundColor { get; } = new ColorOption(new Color(0.31f, 0.32f, 0.33f), Lang.Get("Theme.SelectorBackgroundColor"));
        [Persistent.Option("HudColors", "Line")] public static ColorOption LineColor { get; } = new ColorOption(new Color(0.8f, 0.8f, 0.8f, 0.4f), Lang.Get("Theme.LineColor"));

        [Persistent.Option("FactionColors", "Own")] public static ColorOption FactionOwnColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.FactionOwnColor"));
        [Persistent.Option("FactionColors", "Allied")] public static ColorOption FactionAlliedColor { get; } = new ColorOption(new Color(0f, 0.5f, 1f), Lang.Get("Theme.FactionAlliedColor"));
        [Persistent.Option("FactionColors", "Independent")] public static ColorOption FactionIndependentColor { get; } = new ColorOption(new Color(0.4f, 0.9f, 1f), Lang.Get("Theme.FactionIndependentColor"));
        [Persistent.Option("FactionColors", "Hostile")] public static ColorOption FactionHostileColor { get; } = new ColorOption(new Color(1f, 0.1f, 0f), Lang.Get("Theme.FactionHostileColor"));
        [Persistent.Option("FactionColors", "Wild")] public static ColorOption FactionWildColor { get; } = new ColorOption(new Color(0.8f, 0.5f, 0.2f), Lang.Get("Theme.FactionWildColor"));

        [Persistent.Option("SkillColors", "MinorPassion")] public static ColorOption SkillMinorPassionColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.7f), Lang.Get("Theme.SkillMinorPassionColor"));
        [Persistent.Option("SkillColors", "MajorPassion")] public static ColorOption SkillMajorPassionColor { get; } = new ColorOption(new Color(1f, 0.8f, 0.4f), Lang.Get("Theme.SkillMajorPassionColor"));

        public static Rect GetHudBounds()
        {
            var anchor = GetHudAnchor();
            var x = ((UI.screenWidth * anchor.x) + HudOffsetX.Value) - (HudWidth.Value * anchor.x);
            var y = ((UI.screenHeight * anchor.y) + HudOffsetY.Value) - (HudHeight.Value * anchor.y);

            return new Rect(x, y, HudWidth.Value, HudHeight.Value).Round();
        }

        private static Vector2 GetHudAnchor()
        {
            var value = HudAnchor.Value;
            return new Vector2(Mathf.Floor(value % 3f) / 2f, Mathf.Floor(value / 3f) / 2f);
        }

        private static void EnsureHudNotHidden(ThemeOption option)
        {
            if (!(option is BoolOption inspectPaneModify)) { throw new Mod.Exception("InspectPaneModify is not a BoolOption"); }
            if (HudDocked.Value && !inspectPaneModify.Value) { HudDocked.Value = false; }
        }
        private static void EnsureInspectPaneModify(ThemeOption option)
        {
            if ((bool) option.Object) { InspectPaneTabModify.Value = true; }
        }

        private static void UpdateTextStyles()
        {
            SmallTextStyle?.UpdateStyle();
            LargeTextStyle?.UpdateStyle();
        }

        private static void SetOffsetBounds()
        {
            var anchor = GetHudAnchor();
            var halfWidth = UI.screenWidth * 0.5f;
            var halfHeight = UI.screenHeight * 0.5f;
            var xMin = 0 - (halfWidth * anchor.x);
            var yMin = 0 - (halfHeight * anchor.y);

            HudOffsetX.SetMinMax(Mathf.RoundToInt(xMin), Mathf.RoundToInt(xMin + halfWidth));
            HudOffsetY.SetMinMax(Mathf.RoundToInt(yMin), Mathf.RoundToInt(yMin + halfHeight));
        }
    }
}
