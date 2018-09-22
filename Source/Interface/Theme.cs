using RimHUD.Data;
using RimHUD.Integration;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    [StaticConstructorOnStartup]
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

        private const int DefaultInspectPaneHeight = 270;
        private const int DefaultInspectPaneMaxTabs = 7;
        private const int DefaultInspectPaneTabWidth = 80;

        public static GUIStyle BaseGUIStyle => new GUIStyle(Text.fontStyles[(int) GameFont.Medium]) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        private static readonly string[] HudAnchors = Lang.Get("Theme.HudAnchors").Split('|');

        [Persistent.Option("HudPosition", "Docked")]
        public static BoolOption HudDocked { get; } = new BoolOption(DefaultHudDocked, Lang.Get("Theme.HudDocked"), Lang.Get("Theme.HudDockedDesc"));

        [Persistent.Option("HudPosition", "Anchor")]
        public static RangeOption HudAnchor { get; } = new RangeOption(DefaultHudAnchor, 0, HudAnchors.LastIndex(), Lang.Get("Theme.HudAnchor"), value => HudAnchors[value], onChange: option => SetOffsetBounds());

        [Persistent.Option("HudPosition", "OffsetX")]
        public static RangeOption HudOffsetX { get; } = new RangeOption(DefaultHudOffsetX, -Screen.width, Screen.width, Lang.Get("Theme.HudOffsetX"));

        [Persistent.Option("HudPosition", "OffsetY")]
        public static RangeOption HudOffsetY { get; } = new RangeOption(DefaultHudOffsetY, -Screen.height, Screen.height, Lang.Get("Theme.HudOffsetY"));

        [Persistent.Option("HudDimensions", "Width")]
        public static RangeOption HudWidth { get; } = new RangeOption(DefaultHudWidth, 200, 600, Lang.Get("Theme.HudWidth"));

        [Persistent.Option("HudDimensions", "Height")]
        public static RangeOption HudHeight { get; } = new RangeOption(DefaultHudHeight, 300, 600, Lang.Get("Theme.HudHeight"));

        [Persistent.Option("InspectPane", "Modify")]
        public static BoolOption InspectPaneTabModify { get; } = new BoolOption(DefaultInspectPaneModify, Lang.Get("Theme.InspectPaneModify"), Lang.Get("Theme.InspectPaneModifyDesc"), EnsureHudNotHidden);

        [Persistent.Option("InspectPane", "AddLog")]
        public static BoolOption InspectPaneTabAddLog { get; } = new BoolOption(DefaultInspectPaneAddLog, Lang.Get("Theme.InspectPaneAddLog"), Lang.Get("Theme.InspectPaneAddLogDesc"));

        [Persistent.Option("InspectPane", "AddPawnRules")]
        public static BoolOption InspectPaneTabAddPawnRules { get; } = new BoolOption(DefaultInspectPaneAddPawnRules, Lang.Get("Theme.PawnRules.InspectPaneAddRules"));

        [Persistent.Option("InspectPane", "Height")]
        public static RangeOption InspectPaneHeight { get; } = new RangeOption(DefaultInspectPaneHeight, 200, 500, Lang.Get("Theme.InspectPaneHeight"));

        [Persistent.Option("InspectPane", "MinTabs")]
        public static RangeOption InspectPaneMinTabs { get; } = new RangeOption(DefaultInspectPaneMaxTabs, 6, 12, Lang.Get("Theme.InspectPaneMinTabs"));

        [Persistent.Option("InspectPane", "TabWidth")]
        public static RangeOption InspectPaneTabWidth { get; } = new RangeOption(DefaultInspectPaneTabWidth, 72, 150, Lang.Get("Theme.InspectPaneTabWidth"));

        [Persistent.Option("Text", "Regular")]
        public static TextStyle RegularTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleRegular"), null, BaseGUIStyle.fontSize, 7, 20, 100, 100, 250);

        [Persistent.Option("Text", "Large")]
        public static TextStyle LargeTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 3, 0, 5, 150, 100, 250);

        [Persistent.Option("Text", "Small")]
        public static TextStyle SmallTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250);

        [Persistent.Option("Text", "LabelWidth")]
        public static RangeOption LabelWidth { get; } = new RangeOption(DefaultLabelWidth, 30, 150, Lang.Get("Theme.LabelWidth"));

        [Persistent.Option("Text", "ValueWidth")]
        public static RangeOption ValueWidth { get; } = new RangeOption(DefaultValueWidth, 30, 150, Lang.Get("Theme.ValueWidth"));

        [Persistent.Option("Letters", "Padding")]
        public static RangeOption LetterPadding { get; } = new RangeOption(DefaultLetterPadding, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));

        [Persistent.Option("Letters", "Compress")]
        public static BoolOption LetterCompress { get; } = new BoolOption(DefaultLetterCompress, Lang.Get("Theme.LetterCompress"), Lang.Get("Theme.LetterCompressDesc"));

        [Persistent.Option("Integration.PawnRules", "HideGizmo")]
        public static BoolOption IntegrationPawnRulesHideGizmo { get; } = new BoolOption(false, Lang.Get("Theme.PawnRules.HideGizmo"), Lang.Get("Theme.PawnRules.HideGizmoDesc"), option => PawnRules.SetHideGizmo((bool) option.Object));

        [Persistent.Option("Integration.Bubbles", "FadeStart")]
        public static RangeOption IntegrationBubblesFadeStart { get; } = new RangeOption(500, 1, 2500, Lang.Get("Theme.Bubbles.FadeStart"), onChange: option => Bubbles.SetFadeStart((int) option.Object));

        [Persistent.Option("Integration.Bubbles", "FadeTime")]
        public static RangeOption IntegrationBubblesFadeTime { get; } = new RangeOption(500, 1, 2500, Lang.Get("Theme.Bubbles.FadeTime"), onChange: option => Bubbles.SetFadeTime((int) option.Object));

        [Persistent.Option("Integration.Bubbles", "Width")]
        public static RangeOption IntegrationBubblesWidth { get; } = new RangeOption(300, 150, 400, Lang.Get("Theme.Bubbles.Width"), onChange: option => Bubbles.SetWidth((int) option.Object));

        [Persistent.Option("Integration.Bubbles", "Padding")]
        public static RangeOption IntegrationBubblesPadding { get; } = new RangeOption(2, 2, 12, Lang.Get("Theme.Bubbles.Padding"), onChange: option => Bubbles.SetPadding((int) option.Object));

        [Persistent.Option("Integration.Bubbles", "MaxPerPawn")]
        public static RangeOption IntegrationBubblesMaxPerPawn { get; } = new RangeOption(3, 1, 10, Lang.Get("Theme.Bubbles.MaxPerPawn"), onChange: option => Bubbles.SetMaxPerPawn((int) option.Object));

        public static Color LineColor { get; set; } = new Color(0.8f, 0.8f, 0.8f, 0.4f);

        public static Color FactionOwnColor { get; set; } = new Color(1f, 1f, 1f);
        public static Color FactionAlliedColor { get; set; } = new Color(0f, 0.5f, 1f);
        public static Color FactionIndependentColor { get; set; } = new Color(0.4f, 0.9f, 1f);
        public static Color FactionHostileColor { get; set; } = new Color(1f, 0.1f, 0f);
        public static Color FactionWildColor { get; set; } = new Color(0.8f, 0.5f, 0.2f);

        public static Color SkillDisabledColor { get; } = new Color(0.5f, 0.5f, 0.5f);
        public static Color SkillNoPassionColor { get; } = new Color(0.9f, 0.9f, 0.9f);
        public static Color SkillMinorPassionColor { get; } = new Color(1f, 0.9f, 0.7f);
        public static Color SkillMajorPassionColor { get; } = new Color(1f, 0.8f, 0.4f);

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
        public static readonly Texture2D ConfigIcon = LoadTexture("ConfigIcon");
        public static readonly Texture2D InspectTabButtonFillTex = (Texture2D) Access.Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex.GetValue(null);
        public static readonly Texture2D SelectOverlappingNextTex = ContentFinder<Texture2D>.Get("UI/Buttons/SelectNextOverlapping");

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

        private static Texture2D LoadTexture(string key) => ContentFinder<Texture2D>.Get(Mod.Id + "/" + key);

        private static Color HexColor(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : default(Color);

        public static void ToDefault()
        {
            foreach (var property in typeof(Theme).GetProperties())
            {
                var attribute = property.TryGetAttribute<Persistent.Option>();
                if (attribute == null) { continue; }

                var propertyValue = property.GetValue(null, null);
                if (propertyValue == null) { continue; }

                if (!(propertyValue is ThemeOption option)) { continue; }
                option.ToDefault();
            }
        }
    }
}
