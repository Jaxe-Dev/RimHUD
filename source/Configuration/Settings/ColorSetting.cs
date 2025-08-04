using System;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

public sealed class ColorSetting : ValueSetting
{
  [Setting(typeof(Color))] public Color Value { get => (Color)Object; set => Object = value; }

  public ColorSetting(Color @default, string label, string? tooltip = null, Action<ColorSetting>? onChange = null, Func<ColorSetting, bool>? saveCheck = null, bool canIncludeInPreset = false) : base(@default, label, tooltip, ConvertOnChange(onChange), ConvertSaveCheck(saveCheck), canIncludeInPreset)
  { }

  public override bool IsDefault() => Value.ToHex().Equals(((Color)Default).ToHex());

  public override string ToString() => Value.ToHex();
}
