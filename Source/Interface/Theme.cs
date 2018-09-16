using System;
using System.Xml.Linq;
using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    [StaticConstructorOnStartup]
    internal static class Theme
    {
        private const int DefaultBaseFontSize = 12;

        private const int DefaultHudWidth = 280;
        private const int DefaultHudHeight = 360;

        private const int DefaultHudAnchor = 2;
        private const int DefaultHudOffsetX = 0;
        private const int DefaultHudOffsetY = 0;

        private const int DefaultLetterPadding = 4;
        private const bool DefaultLetterCompress = true;

        public static RangeOption HudWidth { get; } = new RangeOption(DefaultHudWidth, 200, 600, Lang.Get("Theme.HudWidth"));
        public static RangeOption HudHeight { get; } = new RangeOption(DefaultHudHeight, 300, 600, Lang.Get("Theme.HudHeight"));

        private static readonly string[] HudAnchors = Lang.Get("Theme.HudAnchors").Split('|');
        public static RangeOption HudAnchor { get; } = new RangeOption(DefaultHudAnchor, 0, HudAnchors.LastIndex(), Lang.Get("Theme.HudAnchor"), value => HudAnchors[value], onChange: SetOffsetBounds);
        public static RangeOption HudOffsetX { get; } = new RangeOption(DefaultHudOffsetX, -Screen.width, Screen.width, Lang.Get("Theme.HudOffsetX"));
        public static RangeOption HudOffsetY { get; } = new RangeOption(DefaultHudOffsetY, -Screen.height, Screen.height, Lang.Get("Theme.HudOffsetY"));

        public static RangeOption LetterPadding { get; } = new RangeOption(DefaultLetterPadding, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));
        public static bool LetterCompress { get; set; } = DefaultLetterCompress;

        public static GUIStyle BaseGUIStyle => new GUIStyle(Text.fontStyles[(int) GameFont.Medium]) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };

        public static TextStyle RegularTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleRegular"), null, BaseGUIStyle.fontSize, 7, 20, 100, 100, 250);
        public static TextStyle LargeTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 3, 0, 5, 150, 100, 250);
        public static TextStyle SmallTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250);

        public static Color BaseTextColor { get; set; } = new Color(0.9f, 0.9f, 0.9f);

        public static Color BackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        public static Color LineColor { get; set; } = new Color(0.8f, 0.8f, 0.8f, 0.4f);
        public static Color DisabledColor { get; set; } = new Color(0.5f, 0.5f, 0.5f);

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
        public static readonly Texture2D ConfigIcon = LoadTexture("ConfigIcon");

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

        private static int GetHudAnchorFromString(string value)
        {
            var index = Array.IndexOf(HudAnchors, value);
            return index >= 0 ? index : DefaultHudAnchor;
        }

        public static void SetOffsetBounds()
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
            HudWidth.ToDefault();
            HudHeight.ToDefault();
            HudAnchor.ToDefault();
            HudOffsetX.ToDefault();
            HudOffsetY.ToDefault();
            RegularTextStyle.ToDefault();
            LargeTextStyle.ToDefault();
            SmallTextStyle.ToDefault();
            LetterPadding.ToDefault();
            LetterCompress = DefaultLetterCompress;
        }

        public static XElement ToXml()
        {
            var xml = new XElement("RimHUD");

            var dimensions = new XElement("HudDimensions");
            dimensions.Add(new XElement("Width", HudWidth.Value));
            dimensions.Add(new XElement("Height", HudHeight.Value));
            xml.Add(dimensions);

            var position = new XElement("HudPosition");
            position.Add(new XElement("Anchor", HudAnchor.ToString()));
            position.Add(new XElement("OffsetX", HudOffsetX.Value));
            position.Add(new XElement("OffsetY", HudOffsetY.Value));
            xml.Add(position);

            var text = new XElement("TextStyle");
            text.Add(RegularTextStyle.ToXml("RegularTextStyle"));
            text.Add(LargeTextStyle.ToXml("LargeTextStyle"));
            text.Add(SmallTextStyle.ToXml("SmallTextStyle"));
            xml.Add(text);

            var other = new XElement("Other");
            other.Add(new XElement("LetterCompress", LetterCompress));
            other.Add(new XElement("LetterPadding", LetterPadding.Value));
            xml.Add(other);

            return xml;
        }

        public static void FromXml(XElement xml)
        {
            var dimensions = xml.Element("HudDimensions");
            HudWidth.FromString(dimensions?.Element("Width")?.Value);
            HudHeight.FromString(dimensions?.Element("Height")?.Value);

            var position = xml.Element("HudPosition");
            HudAnchor.Value = GetHudAnchorFromString(position?.Element("Anchor")?.Value);
            HudOffsetX.FromString(position?.Element("OffsetX")?.Value);
            HudOffsetY.FromString(position?.Element("OffsetY")?.Value);

            var text = xml.Element("TextStyle");
            RegularTextStyle.FromXml(text?.Element("RegularTextStyle"));
            LargeTextStyle.FromXml(text?.Element("LargeTextStyle"));
            SmallTextStyle.FromXml(text?.Element("SmallTextStyle"));

            var other = xml.Element("Other");
            LetterCompress = other?.Element("LetterCompress")?.Value.ToBool() ?? LetterCompress;
            LetterPadding.FromString(other?.Element("LetterPadding")?.Value);
        }
    }
}
