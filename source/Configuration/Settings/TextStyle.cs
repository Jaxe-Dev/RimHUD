using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Configuration.Settings;

public sealed class TextStyle : BaseSetting
{
  private readonly TextStyle? _baseTextStyle;

  public string? Label { get; }

  public GUIStyle GUIStyle { get; private set; } = null!;

  public int ActualSize => GUIStyle.fontSize;

  [Setting("Size")] public RangeSetting Size { get; }
  [Setting("Line")] public RangeSetting Height { get; }

  public float LineHeight { get; private set; }

  public TextStyle(string? label, TextStyle? baseTextStyle, int size, int sizeMin, int sizeMax, int height, int heightMin, int heightMax, Action<TextStyle>? onChange = null, Func<TextStyle, bool>? saveCheck = null, bool canIncludeInPreset = false) : base(ConvertOnChange(onChange), ConvertSaveCheck(saveCheck), canIncludeInPreset)
  {
    _baseTextStyle = baseTextStyle;

    Label = label;

    IsUpdating = true;

    Size = new RangeSetting(size, sizeMin, sizeMax, Lang.Get("Theme.TextStyle.Size"), value => _baseTextStyle is null ? value.ToString() : value.ToStringWithSign(), onChange: _ => UpdateStyle());
    Height = new RangeSetting(height, heightMin, heightMax, Lang.Get("Theme.TextStyle.Height"), static value => $"{value}%", onChange: _ => UpdateStyle());

    UpdateStyle();

    IsUpdating = false;
  }

  public static void SetFromString(string? value)
  {
    Presets.IsLoading = true;
    Theme.RegularTextStyle.Size.ToDefault();
    Theme.RegularTextStyle.Height.ToDefault();
    Theme.LargeTextStyle.Size.ToDefault();
    Theme.LargeTextStyle.Height.ToDefault();
    Theme.SmallTextStyle.Size.ToDefault();
    Theme.SmallTextStyle.Height.ToDefault();
    Theme.RegularTextStyle.UpdateStyle();
    Theme.UpdateTextStyles();
    Presets.IsLoading = false;

    if (value.NullOrWhitespace()) { return; }

    var split = value.Split('|');
    if (split?.Length is not 6) { return; }

    Presets.IsLoading = true;
    Theme.RegularTextStyle.Size.Value = split[0].ToInt() ?? Theme.RegularTextStyle.Size.Value;
    Theme.RegularTextStyle.Height.Value = split[1].ToInt() ?? Theme.RegularTextStyle.Height.Value;
    Theme.LargeTextStyle.Size.Value = split[2].ToInt() ?? Theme.LargeTextStyle.Size.Value;
    Theme.LargeTextStyle.Height.Value = split[3].ToInt() ?? Theme.LargeTextStyle.Height.Value;
    Theme.SmallTextStyle.Size.Value = split[4].ToInt() ?? Theme.SmallTextStyle.Size.Value;
    Theme.SmallTextStyle.Height.Value = split[5].ToInt() ?? Theme.SmallTextStyle.Height.Value;
    Theme.RegularTextStyle.UpdateStyle();
    Theme.UpdateTextStyles();
    Presets.IsLoading = false;
  }

  public static string GetSizesString() => $"{Theme.RegularTextStyle.Size.Value}|{Theme.RegularTextStyle.Height.Value}|{Theme.LargeTextStyle.Size.Value}|{Theme.LargeTextStyle.Height.Value}|{Theme.SmallTextStyle.Size.Value}|{Theme.SmallTextStyle.Height.Value}";

  public override void ToDefault()
  {
    Size.ToDefault();
    Height.ToDefault();
  }

  public override bool IsDefault() => Size.IsDefault() && Height.IsDefault();

  public void UpdateStyle()
  {
    GUIStyle = _baseTextStyle?.GUIStyle.ResizedBy(Size.Value) ?? Theme.BaseGUIStyle.SetTo(Size.Value);
    LineHeight = GUIStyle.lineHeight * Height.Value.ToPercentageFloat();

    OnChange();
  }
}
