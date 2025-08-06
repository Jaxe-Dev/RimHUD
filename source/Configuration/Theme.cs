using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface;
using RimHUD.Interface.Dialog.Tabs;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;
using Verse;

namespace RimHUD.Configuration;

public static class Theme
{
  private const int DefaultBaseFontSize = 13;

  private static BaseSetting[]? _settings;

  private static Font _baseFont = GetCurrentFontStyle();

  public static readonly Color StackColor = new(1f, 1f, 0.5f);
  public static readonly Color PanelColor = new(1f, 0.75f, 0.5f);
  public static readonly Color RowColor = new(0.75f, 0.75f, 0.75f);
  public static readonly Color WidgetColor = Color.white;
  public static readonly Color MissingWidgetColor = Color.red;

  public static readonly Color CorePresetColor = new(1f, 1f, 0.75f);
  public static readonly Color UserPresetColor = new(0.75f, 1f, 0.75f);
  public static readonly Color ExternalModColor = new(0.5f, 1f, 1f);

  public static readonly Color ContainerColor = new(1f, 1f, 1f, 0.025f);

  public static readonly Color ButtonSelectedColor = new(0.5f, 1f, 0.5f);
  public static readonly Color ItemSelectedColor = new(0.25f, 0.4f, 0.1f);

  public static IEnumerable<BaseSetting> Settings => _settings ??= typeof(Theme).GetProperties().Where(static property => property.TryGetAttribute<SettingAttribute>() is not null).Select(static property => property.GetValue(null, null) as BaseSetting).WhereNotNull().ToArray();
  public static GUIStyle BaseGUIStyle => new(Text.CurFontStyle) { fontSize = DefaultBaseFontSize, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
  public static GUIStyle TinyUITextStyle => new(Text.CurFontStyle) { fontSize = DefaultBaseFontSize - 2, alignment = TextAnchor.MiddleLeft, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };

  [Setting("General", "RefreshRate")] public static RangeSetting RefreshRate { get; } = new(5, 1, 30, Lang.Get("Theme.RefreshRate"), static value => (value * 100) + Lang.Get("Theme.RefreshRateUnits"), Lang.Get("Theme.RefreshRateDesc"));
  [Setting("General", "DockedMode")] public static BoolSetting DockedMode { get; } = new(true, Lang.Get("Theme.InspectPaneDocked"), Lang.Get("Theme.InspectPaneDockedDesc"), EnsureInspectPaneModify);

  [Setting("InspectPane", "Modify")] public static BoolSetting InspectPaneTabModify { get; } = new(true, Lang.Get("Theme.InspectPaneModify"), Lang.Get("Theme.InspectPaneModifyDesc"), EnsureHudNotHidden);
  [Setting("InspectPane", "AddLog")] public static BoolSetting InspectPaneTabAddLog { get; } = new(true, Lang.Get("Theme.InspectPaneAddLog"), Lang.Get("Theme.InspectPaneAddLogDesc"));
  [Setting("InspectPane", "Height")] public static RangeSetting InspectPaneHeight { get; } = new(232, 165, 800, Lang.Get("Theme.InspectPaneHeight"), saveCheck: static _ => LayoutLayer.Docked.HasDefinedHeight, canIncludeInPreset: true);
  [Setting("InspectPane", "MinTabs")] public static RangeSetting InspectPaneMinTabs { get; } = new(7, 6, 12, Lang.Get("Theme.InspectPaneMinTabs"), saveCheck: static _ => LayoutLayer.Floating.HasDefinedTabs, canIncludeInPreset: true);
  [Setting("InspectPane", "TabWidth")] public static RangeSetting InspectPaneTabWidth { get; } = new(85, 72, 150, Lang.Get("Theme.InspectPaneTabWidth"), saveCheck: static _ => LayoutLayer.Floating.HasDefinedWidth, canIncludeInPreset: true);
  [Setting("InspectPane", "ShowFactionIcon")] public static BoolSetting ShowFactionIcon { get; } = new(true, Lang.Get("Theme.ShowFactionIcon"));
  [Setting("InspectPane", "ShowIdeoligionIcon")] public static BoolSetting ShowIdeoligionIcon { get; } = new(true, Lang.Get("Theme.ShowIdeoligionIcon"));

  [Setting("Floating", "Anchor")] public static RangeSetting FloatingAnchor { get; } = new(2, 0, 8, Lang.Get("Theme.FloatingAnchor"), static value => Lang.GetIndexed("Theme.FloatingAnchors", value), onChange: static _ => SetOffsetBounds());
  [Setting("Floating", "OffsetX")] public static RangeSetting FloatingOffsetX { get; } = new(0, -Screen.width, Screen.width, Lang.Get("Theme.FloatingOffsetX"));
  [Setting("Floating", "OffsetY")] public static RangeSetting FloatingOffsetY { get; } = new(0, -Screen.height, Screen.height, Lang.Get("Theme.FloatingOffsetY"));
  [Setting("Floating", "Width")] public static RangeSetting FloatingWidth { get; } = new(320, 100, 1000, Lang.Get("Theme.FloatingWidth"), saveCheck: static _ => LayoutLayer.Floating.HasDefinedWidth, canIncludeInPreset: true);
  [Setting("Floating", "Height")] public static RangeSetting FloatingHeight { get; } = new(416, 100, 1000, Lang.Get("Theme.FloatingHeight"), saveCheck: static _ => LayoutLayer.Floating.HasDefinedHeight, canIncludeInPreset: true);
  [Setting("Floating", "LetterPadding")] public static RangeSetting LetterPadding { get; } = new(4, 0, 12, Lang.Get("Theme.LetterPadding"), tooltip: Lang.Get("Theme.LetterPaddingDesc"));
  [Setting("Floating", "LetterCompress")] public static BoolSetting LetterCompress { get; } = new(true, Lang.Get("Theme.LetterCompress"), Lang.Get("Theme.LetterCompressDesc"));

  [Setting("Text", "Regular")] public static TextStyle RegularTextStyle { get; } = new(Lang.Get("Theme.TextStyleRegular"), null, DefaultBaseFontSize, 7, 20, 100, 100, 250, static _ => UpdateTextStyles(), static _ => Presets.Current?.HasDefinedTextStyles ?? false, true);
  [Setting("Text", "Large")] public static TextStyle LargeTextStyle { get; } = new(Lang.Get("Theme.TextStyleLarge"), RegularTextStyle, 9, 0, 24, 100, 100, 250, saveCheck: static _ => Presets.Current?.HasDefinedTextStyles ?? false, canIncludeInPreset: true);
  [Setting("Text", "Small")] public static TextStyle SmallTextStyle { get; } = new(Lang.Get("Theme.TextStyleSmall"), RegularTextStyle, -1, -5, 0, 100, 100, 250, saveCheck: static _ => Presets.Current?.HasDefinedTextStyles ?? false, canIncludeInPreset: true);
  [Setting("Text", "LabelWidth")] public static RangeSetting LabelWidth { get; } = new(95, 30, 150, Lang.Get("Theme.LabelWidth"));
  [Setting("Text", "ValueWidth")] public static RangeSetting ValueWidth { get; } = new(40, 30, 150, Lang.Get("Theme.ValueWidth"));
  [Setting("Text", "ShowDecimals")] public static BoolSetting ShowDecimals { get; } = new(true, Lang.Get("Theme.ShowDecimals"));

  [Setting("Colors", "Line")] public static ColorSetting LineColor { get; } = new(new Color(0.8f, 0.8f, 0.8f, 0.4f), Lang.Get("Theme.LineColor"));

  [Setting("Colors", "Main")] public static ColorSetting MainTextColor { get; } = new(Color.white, Lang.Get("Theme.MainColor"));
  [Setting("Colors", "Disabled")] public static ColorSetting DisabledColor { get; } = new(new Color(0.5f, 0.5f, 0.5f), Lang.Get("Theme.DisabledColor"));
  [Setting("Colors", "Critical")] public static ColorSetting CriticalColor { get; } = new(ColorLibrary.RedReadable, Lang.Get("Theme.CriticalColor"), saveCheck: static setting => setting.Value.ToHex().Equals("D46F68FF", StringComparison.OrdinalIgnoreCase)); // TEMPORARY
  [Setting("Colors", "Warning")] public static ColorSetting WarningColor { get; } = new(new Color(1f, 0.9f, 0.1f), Lang.Get("Theme.WarningColor"), saveCheck: static setting => setting.Value.ToHex().Equals("FF0000FF", StringComparison.OrdinalIgnoreCase));   // TEMPORARY
  [Setting("Colors", "Neutral")] public static ColorSetting NeutralColor { get; } = new(new Color(0.65f, 0.65f, 0.65f), Lang.Get("Theme.NeutralColor"));
  [Setting("Colors", "Good")] public static ColorSetting GoodColor { get; } = new(new Color(0.4f, 0.8f, 0.8f), Lang.Get("Theme.GoodColor"));
  [Setting("Colors", "Excellent")] public static ColorSetting ExcellentColor { get; } = new(new Color(0.4f, 0.8f, 0.2f), Lang.Get("Theme.ExcellentColor"));

  [Setting("Colors", "BarBackground")] public static ColorSetting BarBackgroundColor { get; } = new(new Color(0.2f, 0.2f, 0.2f), Lang.Get("Theme.BarBackgroundColor"));
  [Setting("Colors", "BarMain")] public static ColorSetting BarMainColor { get; } = new(new Color(0.25f, 0.6f, 0f), Lang.Get("Theme.BarMainColor"));
  [Setting("Colors", "BarLow")] public static ColorSetting BarLowColor { get; } = new(new Color(0.6f, 0f, 0f), Lang.Get("Theme.BarLowColor"));
  [Setting("Colors", "BarThreshold")] public static ColorSetting BarThresholdColor { get; } = new(new Color(0f, 0f, 0f, 0.75f), Lang.Get("Theme.BarThresholdColor"));

  [Setting("Colors", "SelectorText")] public static ColorSetting SelectorTextColor { get; } = new(Color.white, Lang.Get("Theme.SelectorTextColor"));
  [Setting("Colors", "SelectorBackground")] public static ColorSetting SelectorBackgroundColor { get; } = new(new Color(0.31f, 0.32f, 0.33f), Lang.Get("Theme.SelectorBackgroundColor"));

  [Setting("Colors", "SkillMinorPassion")] public static ColorSetting SkillMinorPassionColor { get; } = new(new Color(1f, 0.9f, 0.7f), Lang.Get("Theme.SkillMinorPassionColor"));
  [Setting("Colors", "SkillMajorPassion")] public static ColorSetting SkillMajorPassionColor { get; } = new(new Color(1f, 0.8f, 0.4f), Lang.Get("Theme.SkillMajorPassionColor"));
  [Setting("Colors", "SkillSaturated")] public static ColorSetting SkillSaturatedColor { get; } = new(new Color(1f, 0.8f, 0.8f), Lang.Get("Theme.SkillSaturatedColor"));
  [Setting("Colors", "SkillActive")] public static ColorSetting SkillActiveColor { get; } = new(new Color(0.9f, 1f, 0.7f), Lang.Get("Theme.SkillActiveColor"));

  [Setting("Colors", "FactionOwn")] public static ColorSetting FactionOwnColor { get; } = new(Color.white, Lang.Get("Theme.FactionOwnColor"));
  [Setting("Colors", "FactionAllied")] public static ColorSetting FactionAlliedColor { get; } = new(ColoredText.FactionColor_Ally, Lang.Get("Theme.FactionAlliedColor"));
  [Setting("Colors", "FactionIndependent")] public static ColorSetting FactionIndependentColor { get; } = new(ColoredText.FactionColor_Neutral, Lang.Get("Theme.FactionIndependentColor"));
  [Setting("Colors", "FactionHostile")] public static ColorSetting FactionHostileColor { get; } = new(ColoredText.FactionColor_Hostile, Lang.Get("Theme.FactionHostileColor"));
  [Setting("Colors", "FactionWild")] public static ColorSetting FactionWildColor { get; } = new(new Color(0.8f, 0.5f, 0.2f), Lang.Get("Theme.FactionWildColor"));
  [Setting("Colors", "FactionPrisoner")] public static ColorSetting FactionPrisonerColor { get; } = new(new Color(1f, 0.7f, 0.4f), Lang.Get("Theme.FactionPrisonerColor"));
  [Setting("Colors", "FactionSlave")] public static ColorSetting FactionSlaveColor { get; } = new(new Color(1f, 0.85f, 0.2f, byte.MaxValue), Lang.Get("Theme.FactionSlaveColor"));

  private static Font GetCurrentFontStyle() => Text.CurFontStyle!.font;

  public static void UpdateTextStyles()
  {
    SmallTextStyle.UpdateStyle();
    LargeTextStyle.UpdateStyle();

    State.CurrentLayout.RefreshNow();
  }

  public static void CheckFontChange()
  {
    GUIPlus.SetFont(GameFont.Small);
    var isChanged = _baseFont != GetCurrentFontStyle();
    GUIPlus.ResetFont();

    if (!isChanged) { return; }

    _baseFont = GetCurrentFontStyle();
    RegularTextStyle.UpdateStyle();
  }

  private static Vector2 GetHudAnchor() => new(Mathf.Floor(FloatingAnchor.Value % 3f).Half(), Mathf.Floor(FloatingAnchor.Value / 3f).Half());

  public static Rect GetHudBounds()
  {
    var anchor = GetHudAnchor();
    var x = ((UI.screenWidth * anchor.x) + FloatingOffsetX.Value) - (FloatingWidth.Value * anchor.x);
    var y = ((UI.screenHeight * anchor.y) + FloatingOffsetY.Value) - (FloatingHeight.Value * anchor.y);

    return new Rect(x, y, FloatingWidth.Value, FloatingHeight.Value).Round();
  }

  private static void SetOffsetBounds()
  {
    var anchor = GetHudAnchor();
    var halfWidth = UI.screenWidth.Half();
    var halfHeight = UI.screenHeight.Half();
    var xMin = 0 - (halfWidth * anchor.x);
    var yMin = 0 - (halfHeight * anchor.y);

    FloatingOffsetX.SetMinMax(Mathf.RoundToInt(xMin), Mathf.RoundToInt(xMin + halfWidth));
    FloatingOffsetY.SetMinMax(Mathf.RoundToInt(yMin), Mathf.RoundToInt(yMin + halfHeight));
  }

  private static void EnsureHudNotHidden(BoolSetting setting)
  {
    if (DockedMode.Value && !setting.Value) { DockedMode.Value = false; }
  }

  private static void EnsureInspectPaneModify(BoolSetting setting)
  {
    if (setting.Value) { InspectPaneTabModify.Value = true; }

    Tab_ConfigContent.RefreshEditor();
  }

  public static void SetDefaultForPreset()
  {
    InspectPaneTabWidth.ToDefault();
    InspectPaneMinTabs.ToDefault();

    InspectPaneHeight.ToDefault();
    FloatingHeight.ToDefault();
    FloatingWidth.ToDefault();

    RegularTextStyle.ToDefault();
    LargeTextStyle.ToDefault();
    SmallTextStyle.ToDefault();
  }

  public static bool IsDefaultForPreset() => LayoutLayer.IsAllDefault() && SettingAttribute.GetAll(typeof(Theme)).Where(static settings => settings.CanIncludeInPreset).All(static settings => settings.IsDefault());
}
