using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface;
using UnityEngine;
using Verse;

namespace RimHUD.Configuration
{
  public static class Theme
  {
    private const int DefaultBaseFontSize = 12;

    public static GUIStyle BaseGUIStyle => new GUIStyle(Text.CurFontStyle) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
    private static Font _baseFont = Text.CurFontStyle.font;

    [Attributes.Option("HudOptions", "RefreshRate")]
    public static RangeOption RefreshRate { get; } = new RangeOption(5, 1, 50, Lang.Get("Theme.RefreshRate"), value => (value * 100) + Lang.Get("Theme.RefreshRateUnits"), Lang.Get("Theme.RefreshRateDesc"));

    [Attributes.Option("HudPosition", "Docked")]
    public static BoolOption HudDocked { get; } = new BoolOption(true, Lang.Get("Theme.HudDocked"), Lang.Get("Theme.HudDockedDesc"), EnsureInspectPaneModify);
    [Attributes.Option("HudPosition", "Anchor")]
    public static RangeOption HudAnchor { get; } = new RangeOption(2, 0, 8, Lang.Get("Theme.HudAnchor"), value => Lang.GetIndexed("Theme.HudAnchors", value), onChange: _ => SetOffsetBounds());
    [Attributes.Option("HudPosition", "OffsetX")]
    public static RangeOption HudOffsetX { get; } = new RangeOption(0, -Screen.width, Screen.width, Lang.Get("Theme.HudOffsetX"));
    [Attributes.Option("HudPosition", "OffsetY")]
    public static RangeOption HudOffsetY { get; } = new RangeOption(0, -Screen.height, Screen.height, Lang.Get("Theme.HudOffsetY"));

    [Attributes.Option("HudDimensions", "Width")]
    public static RangeOption HudWidth { get; } = new RangeOption(320, 200, 600, Lang.Get("Theme.HudWidth"));
    [Attributes.Option("HudDimensions", "Height")]
    public static RangeOption HudHeight { get; } = new RangeOption(396, 300, 1500, Lang.Get("Theme.HudHeight"));

    [Attributes.Option("InspectPane", "Modify")]
    public static BoolOption InspectPaneTabModify { get; } = new BoolOption(true, Lang.Get("Theme.InspectPaneModify"), Lang.Get("Theme.InspectPaneModifyDesc"), EnsureHudNotHidden);
    [Attributes.Option("InspectPane", "AddLog")]
    public static BoolOption InspectPaneTabAddLog { get; } = new BoolOption(true, Lang.Get("Theme.InspectPaneAddLog"), Lang.Get("Theme.InspectPaneAddLogDesc"));
    [Attributes.Option("InspectPane", "Height")]
    public static RangeOption InspectPaneHeight { get; } = new RangeOption(255, 200, 1500, Lang.Get("Theme.InspectPaneHeight"));
    [Attributes.Option("InspectPane", "MinTabs")]
    public static RangeOption InspectPaneMinTabs { get; } = new RangeOption(7, 6, 12, Lang.Get("Theme.InspectPaneMinTabs"));
    [Attributes.Option("InspectPane", "TabWidth")]
    public static RangeOption InspectPaneTabWidth { get; } = new RangeOption(85, 72, 150, Lang.Get("Theme.InspectPaneTabWidth"), onChange: _ => { InspectPanePlus.ClearButtonWidths(); });

    [Attributes.Option("Text", "Regular")]
    public static TextStyle RegularTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleRegular"), null, BaseGUIStyle.fontSize, 7, 20, 100, 100, 250, _ => UpdateTextStyles());
    [Attributes.Option("Text", "Large")]
    public static TextStyle LargeTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 3, 0, 5, 150, 100, 250);
    [Attributes.Option("Text", "Small")]
    public static TextStyle SmallTextStyle { get; } = new TextStyle(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250);
    [Attributes.Option("Text", "LabelWidth")]
    public static RangeOption LabelWidth { get; } = new RangeOption(95, 30, 150, Lang.Get("Theme.LabelWidth"));
    [Attributes.Option("Text", "ValueWidth")]
    public static RangeOption ValueWidth { get; } = new RangeOption(40, 30, 150, Lang.Get("Theme.ValueWidth"));
    [Attributes.Option("Text", "ShowDecimals")]
    public static BoolOption ShowDecimals { get; } = new BoolOption(true, Lang.Get("Theme.ShowDecimals"));

    [Attributes.Option("InspectPane", "ShowFactionIcon")]
    public static BoolOption ShowFactionIcon { get; } = new BoolOption(true, Lang.Get("Theme.ShowFactionIcon"));
    [Attributes.Option("InspectPane", "ShowIdeoligionIcon")]
    public static BoolOption ShowIdeoligionIcon { get; } = new BoolOption(true, Lang.Get("Theme.ShowIdeoligionIcon"));

    [Attributes.Option("Letters", "Padding")]
    public static RangeOption LetterPadding { get; } = new RangeOption(4, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));
    [Attributes.Option("Letters", "Compress")]
    public static BoolOption LetterCompress { get; } = new BoolOption(false, Lang.Get("Theme.LetterCompress"), Lang.Get("Theme.LetterCompressDesc"));

    [Attributes.Option("HudColors", "MainText")]
    public static ColorOption MainTextColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.MainTextColor"));
    [Attributes.Option("HudColors", "Disabled")]
    public static ColorOption DisabledColor { get; } = new ColorOption(new Color(0.5f, 0.5f, 0.5f), Lang.Get("Theme.DisabledColor"));
    [Attributes.Option("HudColors", "Critical")]
    public static ColorOption CriticalColor { get; } = new ColorOption(new Color(1f, 0.2f, 0.2f), Lang.Get("Theme.CriticalColor"));
    [Attributes.Option("HudColors", "Warning")]
    public static ColorOption WarningColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.1f), Lang.Get("Theme.WarningColor"));
    [Attributes.Option("HudColors", "Info")]
    public static ColorOption InfoColor { get; } = new ColorOption(new Color(0.6f, 0.6f, 0.6f), Lang.Get("Theme.InfoColor"));
    [Attributes.Option("HudColors", "Good")]
    public static ColorOption GoodColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.8f), Lang.Get("Theme.GoodColor"));
    [Attributes.Option("HudColors", "Excellent")]
    public static ColorOption ExcellentColor { get; } = new ColorOption(new Color(0.4f, 0.8f, 0.2f), Lang.Get("Theme.ExcellentColor"));
    [Attributes.Option("HudColors", "BarBackground")]
    public static ColorOption BarBackgroundColor { get; } = new ColorOption(new Color(0.2f, 0.2f, 0.2f), Lang.Get("Theme.BarBackgroundColor"));
    [Attributes.Option("HudColors", "BarMain")]
    public static ColorOption BarMainColor { get; } = new ColorOption(new Color(0.25f, 0.6f, 0f), Lang.Get("Theme.BarMainColor"));
    [Attributes.Option("HudColors", "BarLow")]
    public static ColorOption BarLowColor { get; } = new ColorOption(new Color(0.6f, 0f, 0f), Lang.Get("Theme.BarLowColor"));
    [Attributes.Option("HudColors", "BarThreshold")]
    public static ColorOption BarThresholdColor { get; } = new ColorOption(new Color(0f, 0f, 0f, 0.75f), Lang.Get("Theme.BarThresholdColor"));
    [Attributes.Option("HudColors", "SelectorText")]
    public static ColorOption SelectorTextColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.SelectorTextColor"));
    [Attributes.Option("HudColors", "SelectorBackground")]
    public static ColorOption SelectorBackgroundColor { get; } = new ColorOption(new Color(0.31f, 0.32f, 0.33f), Lang.Get("Theme.SelectorBackgroundColor"));
    [Attributes.Option("HudColors", "Line")]
    public static ColorOption LineColor { get; } = new ColorOption(new Color(0.8f, 0.8f, 0.8f, 0.4f), Lang.Get("Theme.LineColor"));

    [Attributes.Option("FactionColors", "Own")]
    public static ColorOption FactionOwnColor { get; } = new ColorOption(new Color(1f, 1f, 1f), Lang.Get("Theme.FactionOwnColor"));
    [Attributes.Option("FactionColors", "Allied")]
    public static ColorOption FactionAlliedColor { get; } = new ColorOption(new Color(0f, 0.5f, 1f), Lang.Get("Theme.FactionAlliedColor"));
    [Attributes.Option("FactionColors", "Independent")]
    public static ColorOption FactionIndependentColor { get; } = new ColorOption(new Color(0.4f, 0.9f, 1f), Lang.Get("Theme.FactionIndependentColor"));
    [Attributes.Option("FactionColors", "Hostile")]
    public static ColorOption FactionHostileColor { get; } = new ColorOption(new Color(1f, 0.1f, 0f), Lang.Get("Theme.FactionHostileColor"));
    [Attributes.Option("FactionColors", "Wild")]
    public static ColorOption FactionWildColor { get; } = new ColorOption(new Color(0.8f, 0.5f, 0.2f), Lang.Get("Theme.FactionWildColor"));
    [Attributes.Option("FactionColors", "Prisoner")]
    public static ColorOption FactionPrisonerColor { get; } = new ColorOption(new Color(0.85f, 1f, 0.5f), Lang.Get("Theme.FactionPrisonerColor"));
    [Attributes.Option("FactionColors", "Slave")]
    public static ColorOption FactionSlaveColor { get; } = new ColorOption(new Color(0.95f, 0.85f, 0.1f), Lang.Get("Theme.FactionSlaveColor"));

    [Attributes.Option("SkillColors", "MinorPassion")]
    public static ColorOption SkillMinorPassionColor { get; } = new ColorOption(new Color(1f, 0.9f, 0.7f), Lang.Get("Theme.SkillMinorPassionColor"));
    [Attributes.Option("SkillColors", "MajorPassion")]
    public static ColorOption SkillMajorPassionColor { get; } = new ColorOption(new Color(1f, 0.8f, 0.4f), Lang.Get("Theme.SkillMajorPassionColor"));
    [Attributes.Option("SkillColors", "Saturated")]
    public static ColorOption SkillSaturatedColor { get; } = new ColorOption(new Color(1f, 0.8f, 0.8f), Lang.Get("Theme.SkillSaturatedColor"));
    [Attributes.Option("SkillColors", "Active")]
    public static ColorOption SkillActiveColor { get; } = new ColorOption(new Color(0.9f, 1f, 0.7f), Lang.Get("Theme.SkillActiveColor"));

    public static readonly Color ThirdPartyModColor = new Color(0.5f, 1f, 1f);

    public static readonly Color StackColor = new Color(1f, 1f, 0.5f);
    public static readonly Color PanelColor = new Color(1f, 0.75f, 0.5f);
    public static readonly Color RowColor = new Color(0.75f, 0.75f, 0.75f);
    public static readonly Color WidgetColor = new Color(1f, 1f, 1f);

    public static readonly Color ContainerColor = new Color(1f, 1f, 1f, 0.025f);

    public static readonly Color ButtonSelectedColor = new Color(0.5f, 1f, 0.5f);
    public static readonly Color ItemSelectedColor = new Color(0.25f, 0.4f, 0.1f);

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
      if ((bool)option.Object) { InspectPaneTabModify.Value = true; }
    }

    private static void UpdateTextStyles()
    {
      SmallTextStyle?.UpdateStyle();
      LargeTextStyle?.UpdateStyle();
    }

    public static void CheckFontChange()
    {
      GUIPlus.SetFont(GameFont.Small);
      var isChanged = _baseFont != Text.CurFontStyle.font;
      GUIPlus.ResetFont();

      if (!isChanged) { return; }

      _baseFont = Text.CurFontStyle.font;
      RegularTextStyle.UpdateStyle();
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
