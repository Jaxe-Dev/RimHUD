using RimHUD.Data;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;
using ColorOption = RimHUD.Data.ColorOption;

namespace RimHUD.Interface
{
    internal static class Theme
    {
        private const int DefaultBaseFontSize = 12;

        private const bool DefaultHudDocked = true;

        private const int DefaultHudAnchor = 2;
        private const int DefaultHudOffsetX = 0;
        private const int DefaultHudOffsetY = 0;

        private const int DefaultHudWidth = 280;
        private const int DefaultHudHeight = 360;

        private const int DefaultLetterPadding = 4;
        private const bool DefaultLetterCompress = true;

        private const int DefaultLabelWidth = 95;
        private const int DefaultValueWidth = 40;

        private const bool DefaultInspectPaneModify = true;
        private const bool DefaultInspectPaneAddLog = true;
        private const bool DefaultInspectPaneAddPawnRules = true;

        private const int DefaultInspectPaneHeight = 280;
        private const int DefaultInspectPaneMaxTabs = 7;
        private const int DefaultInspectPaneTabWidth = 82;

        public static GUIStyle BaseGUIStyle => new GUIStyle(Text.fontStyles[(int) GameFont.Medium]) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        public static readonly Def[] SkippedNeeds = { Access.NeedDefOfMood, NeedDefOf.Rest, NeedDefOf.Food, NeedDefOf.Joy, Access.NeedDefOfBeauty, Access.NeedDefOfComfort, Access.NeedDefOfOutdoors, Access.NeedDefOfRoomSize };

        [Persistent.Option("HudPosition", "Docked")] public static BoolOption HudDocked { get; } = new BoolOption(DefaultHudDocked, Lang.Get("Theme.HudDocked"), Lang.Get("Theme.HudDockedDesc"), EnsureInspectPaneModify);
        [Persistent.Option("HudPosition", "Anchor")] public static RangeOption HudAnchor { get; } = new RangeOption(DefaultHudAnchor, 0, 8, Lang.Get("Theme.HudAnchor"), value => Lang.GetIndexed("Theme.HudAnchors", value), onChange: _ => SetOffsetBounds());
        [Persistent.Option("HudPosition", "OffsetX")] public static RangeOption HudOffsetX { get; } = new RangeOption(DefaultHudOffsetX, -Screen.width, Screen.width, Lang.Get("Theme.HudOffsetX"));
        [Persistent.Option("HudPosition", "OffsetY")] public static RangeOption HudOffsetY { get; } = new RangeOption(DefaultHudOffsetY, -Screen.height, Screen.height, Lang.Get("Theme.HudOffsetY"));

        [Persistent.Option("HudDimensions", "Width")] public static RangeOption HudWidth { get; } = new RangeOption(DefaultHudWidth, 200, 600, Lang.Get("Theme.HudWidth"));
        [Persistent.Option("HudDimensions", "Height")] public static RangeOption HudHeight { get; } = new RangeOption(DefaultHudHeight, 300, 600, Lang.Get("Theme.HudHeight"));

        [Persistent.Option("InspectPane", "Modify")] public static BoolOption InspectPaneTabModify { get; } = new BoolOption(DefaultInspectPaneModify, Lang.Get("Theme.InspectPaneModify"), Lang.Get("Theme.InspectPaneModifyDesc"), EnsureHudNotHidden);
        [Persistent.Option("InspectPane", "AddLog")] public static BoolOption InspectPaneTabAddLog { get; } = new BoolOption(DefaultInspectPaneAddLog, Lang.Get("Theme.InspectPaneAddLog"), Lang.Get("Theme.InspectPaneAddLogDesc"));
        [Persistent.Option("InspectPane", "AddPawnRules")] public static BoolOption InspectPaneTabAddPawnRules { get; } = new BoolOption(DefaultInspectPaneAddPawnRules, Lang.Get("Theme.PawnRules.InspectPaneAddRules"));
        [Persistent.Option("InspectPane", "Height")] public static RangeOption InspectPaneHeight { get; } = new RangeOption(DefaultInspectPaneHeight, 200, 500, Lang.Get("Theme.InspectPaneHeight"));
        [Persistent.Option("InspectPane", "MinTabs")] public static RangeOption InspectPaneMinTabs { get; } = new RangeOption(DefaultInspectPaneMaxTabs, 6, 12, Lang.Get("Theme.InspectPaneMinTabs"));
        [Persistent.Option("InspectPane", "TabWidth")] public static RangeOption InspectPaneTabWidth { get; } = new RangeOption(DefaultInspectPaneTabWidth, 72, 150, Lang.Get("Theme.InspectPaneTabWidth"));

        [Persistent.Option("Text", "Regular")] public static TextStyle RegularTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleRegular"), null, BaseGUIStyle.fontSize, 7, 20, 100, 100, 250);
        [Persistent.Option("Text", "Large")] public static TextStyle LargeTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 3, 0, 5, 150, 100, 250);
        [Persistent.Option("Text", "Small")] public static TextStyle SmallTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250);
        [Persistent.Option("Text", "LabelWidth")] public static RangeOption LabelWidth { get; } = new RangeOption(DefaultLabelWidth, 30, 150, Lang.Get("Theme.LabelWidth"));
        [Persistent.Option("Text", "ValueWidth")] public static RangeOption ValueWidth { get; } = new RangeOption(DefaultValueWidth, 30, 150, Lang.Get("Theme.ValueWidth"));

        [Persistent.Option("Letters", "Padding")] public static RangeOption LetterPadding { get; } = new RangeOption(DefaultLetterPadding, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));
        [Persistent.Option("Letters", "Compress")] public static BoolOption LetterCompress { get; } = new BoolOption(DefaultLetterCompress, Lang.Get("Theme.LetterCompress"), Lang.Get("Theme.LetterCompressDesc"));

        [Persistent.Option("Integration.General", "ShowCustomNeeds")] public static BoolOption IntegrationGeneralShowCustomNeeds { get; } = new BoolOption(false, Lang.Get("Theme.GeneralIntegration.ShowCustomNeeds"));

        [Persistent.Option("HudColors", "MainText")] public static ColorOption MainTextColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.MainTextColor"));
        [Persistent.Option("HudColors", "Critical")] public static ColorOption CriticalColor { get; } = new ColorOption(new Color(1f, 0.2f, 0.2f), Lang.Get("Theme.CriticalColor"));
        [Persistent.Option("HudColors", "Warning")] public static ColorOption WarningColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.1f), Lang.Get("Theme.WarningColor"));
        [Persistent.Option("HudColors", "Info")] public static ColorOption InfoColor { get; } = new ColorOption(new Color(0.6f, 0.6f, 0.6f), Lang.Get("Theme.InfoColor"));
        [Persistent.Option("HudColors", "Good")] public static ColorOption GoodColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.8f), Lang.Get("Theme.GoodColor"));
        [Persistent.Option("HudColors", "Excellent")] public static ColorOption ExcellentColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.2f), Lang.Get("Theme.ExcellentColor"));
        [Persistent.Option("HudColors", "BarBackground")] public static ColorOption BarBackgroundColor { get; } = new ColorOption(new Color(0.2f, 0.2f, 0.2f), Lang.Get("Theme.BarBackgroundColor"));
        [Persistent.Option("HudColors", "BarMain")] public static ColorOption BarMainColor { get; } = new ColorOption(new Color(0.1f, 0.6f, 0f), Lang.Get("Theme.BarMainColor"));
        [Persistent.Option("HudColors", "BarLow")] public static ColorOption BarLowColor { get; } = new ColorOption(new Color(0.6f, 0f, 0.1f), Lang.Get("Theme.BarLowColor"));
        [Persistent.Option("HudColors", "BarThresholdMinor")] public static ColorOption BarThresholdMinorColor { get; } = new ColorOption(new Color(0.9f, 0.9f, 0.4f, 0.4f), Lang.Get("Theme.BarThresholdMinorColor"));
        [Persistent.Option("HudColors", "BarThresholdMajor")] public static ColorOption BarThresholdMajorColor { get; } = new ColorOption(new Color(0.9f, 0.7f, 0.4f, 0.4f), Lang.Get("Theme.BarThresholdMajorColor"));
        [Persistent.Option("HudColors", "BarThresholdExtreme")] public static ColorOption BarThresholdExtremeColor { get; } = new ColorOption(new Color(0.9f, 0.4f, 0.4f, 0.4f), Lang.Get("Theme.BarThresholdExtremeColor"));
        [Persistent.Option("HudColors", "Line")] public static ColorOption LineColor { get; } = new ColorOption(new Color(0.8f, 0.8f, 0.8f, 0.4f), Lang.Get("Theme.LineColor"));

        [Persistent.Option("FactionColors", "Own")] public static ColorOption FactionOwnColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.FactionOwnColor"));
        [Persistent.Option("FactionColors", "Allied")] public static ColorOption FactionAlliedColor { get; } = new ColorOption(new Color(0f, 0.5f, 1f), Lang.Get("Theme.FactionAlliedColor"));
        [Persistent.Option("FactionColors", "Independent")] public static ColorOption FactionIndependentColor { get; } = new ColorOption(new Color(0.4f, 0.9f, 1f), Lang.Get("Theme.FactionIndependentColor"));
        [Persistent.Option("FactionColors", "Hostile")] public static ColorOption FactionHostileColor { get; } = new ColorOption(new Color(1f, 0.1f, 0f), Lang.Get("Theme.FactionHostileColor"));
        [Persistent.Option("FactionColors", "Wild")] public static ColorOption FactionWildColor { get; } = new ColorOption(new Color(0.8f, 0.5f, 0.2f), Lang.Get("Theme.FactionWildColor"));

        [Persistent.Option("SkillColors", "Disabled")] public static ColorOption SkillDisabledColor { get; } = new ColorOption(new Color(0.5f, 0.5f, 0.5f), Lang.Get("Theme.SkillDisabledColor"));
        [Persistent.Option("SkillColors", "MinorPassion")] public static ColorOption SkillMinorPassionColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.7f), Lang.Get("Theme.SkillMinorPassionColor"));
        [Persistent.Option("SkillColors", "MajorPassion")] public static ColorOption SkillMajorPassionColor { get; } = new ColorOption(new Color(1f, 0.8f, 0.4f), Lang.Get("Theme.SkillMajorPassionColor"));

        public static Rect GetHudBounds()
        {
            var anchor = GetHudAnchor();
            var x = ((UI.screenWidth * anchor.x) + HudOffsetX.Value) - (HudWidth.Value * anchor.x);
            var y = ((UI.screenHeight * anchor.y) + HudOffsetY.Value) - (HudHeight.Value * anchor.y);

            return new Rect(x, y, HudWidth.Value, HudHeight.Value).Fix();
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
