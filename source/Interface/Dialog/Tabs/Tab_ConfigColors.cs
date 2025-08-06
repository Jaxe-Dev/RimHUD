using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog.Tabs;

public sealed class Tab_ConfigColors : Tab
{
  public override string Label { get; } = Lang.Get("Interface.Dialog_Config.Tab_Colors");

  private Vector2 _scrollPosition = Vector2.zero;
  private Rect _scrollRect;

  private ColorSetting? _selected;

  private readonly RangeSetting _hue = new(0, 0, 100, Lang.Get("Interface.Dialog_Config.Tab_Colors.Hue"));
  private readonly RangeSetting _saturation = new(0, 0, 100, Lang.Get("Interface.Dialog_Config.Tab_Colors.Saturation"));
  private readonly RangeSetting _lightness = new(0, 0, 100, Lang.Get("Interface.Dialog_Config.Tab_Colors.Lightness"));
  private readonly RangeSetting _alpha = new(0, 0, 100, Lang.Get("Interface.Dialog_Config.Tab_Colors.Alpha"));

  private string? _hueText;
  private string? _saturationText;
  private string? _lightnessText;
  private string? _alphaText;
  private string? _hexText;

  public override void Reset() => SelectColor();

  public override void Draw(Rect rect)
  {
    var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
    var l = new ListingPlus();

    var selected = _selected;

    WidgetsPlus.DrawContainer(hGrid[1]);

    l.BeginScrollView(hGrid[1], ref _scrollPosition, ref _scrollRect);

    l.Label(Lang.Get("Theme.HudColors").Bold());
    l.GapLine();
    l.ColorSettingSelect(Theme.MainTextColor, ref _selected);
    l.ColorSettingSelect(Theme.DisabledColor, ref _selected);
    l.ColorSettingSelect(Theme.CriticalColor, ref _selected);
    l.ColorSettingSelect(Theme.WarningColor, ref _selected);
    l.ColorSettingSelect(Theme.NeutralColor, ref _selected);
    l.ColorSettingSelect(Theme.GoodColor, ref _selected);
    l.ColorSettingSelect(Theme.ExcellentColor, ref _selected);
    l.ColorSettingSelect(Theme.BarBackgroundColor, ref _selected);
    l.ColorSettingSelect(Theme.BarMainColor, ref _selected);
    l.ColorSettingSelect(Theme.BarLowColor, ref _selected);
    l.ColorSettingSelect(Theme.BarThresholdColor, ref _selected);
    l.ColorSettingSelect(Theme.SelectorTextColor, ref _selected);
    l.ColorSettingSelect(Theme.SelectorBackgroundColor, ref _selected);
    l.ColorSettingSelect(Theme.LineColor, ref _selected);
    l.Gap();

    l.Label(Lang.Get("Theme.FactionColors").Bold());
    l.GapLine();
    l.ColorSettingSelect(Theme.FactionOwnColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionAlliedColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionIndependentColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionHostileColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionWildColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionPrisonerColor, ref _selected);
    l.ColorSettingSelect(Theme.FactionSlaveColor, ref _selected);
    l.Gap();

    l.Label(Lang.Get("Theme.SkillColors").Bold());
    l.GapLine();
    l.ColorSettingSelect(Theme.SkillMinorPassionColor, ref _selected);
    l.ColorSettingSelect(Theme.SkillMajorPassionColor, ref _selected);
    l.ColorSettingSelect(Theme.SkillSaturatedColor, ref _selected);
    l.ColorSettingSelect(Theme.SkillActiveColor, ref _selected);
    l.Gap();

    l.EndScrollView(ref _scrollRect);

    if (_selected is null) { return; }

    l.Begin(hGrid[2]);

    if (_selected != selected) { SelectColor(); }

    l.Label(Lang.Get("Interface.Dialog_Config.Tab_Colors.Editor", _selected.Label).Bold());
    l.GapLine();

    if (l.HexEntry(Lang.Get("Interface.Dialog_Config.Tab_Colors.RGBA"), _selected, ref _hexText)) { ParseColor(); }
    l.GapLine();

    var hslChanged = l.RangeSliderEntry(_hue, ref _hueText, 1) || l.RangeSliderEntry(_saturation, ref _saturationText, 2) || l.RangeSliderEntry(_lightness, ref _lightnessText, 3) || l.RangeSliderEntry(_alpha, ref _alphaText, 4);

    if (hslChanged)
    {
      var newColor = Color.HSVToRGB(_hue.Value.ToPercentageFloat(), _saturation.Value.ToPercentageFloat(), _lightness.Value.ToPercentageFloat());
      newColor.a = _alpha.Value.ToPercentageFloat();
      _selected.Value = newColor;
      _hexText = _selected.Value.ToHex();
    }

    l.GapLine();

    var sampleRect = l.GetRect(WidgetsPlus.ButtonHeight);
    Widgets.DrawBoxSolid(sampleRect, _selected.Value);

    l.End();
  }

  private void SelectColor()
  {
    if (_selected is null) { return; }

    GUI.FocusControl(null);
    ParseColor();
    _hexText = _selected.Value.ToHex();
  }

  private void ParseColor()
  {
    Color.RGBToHSV(_selected!.Value, out var hue, out var saturation, out var lightness);
    _hue.Value = hue.ToPercentageInt();
    _saturation.Value = saturation.ToPercentageInt();
    _lightness.Value = lightness.ToPercentageInt();
    _alpha.Value = _selected.Value.a.ToPercentageInt();
  }
}
