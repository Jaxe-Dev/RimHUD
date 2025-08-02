using System;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

public sealed class ColorSetting : ValueSetting
{
  [Setting(typeof(Color))] public Color Value { get => (Color)Object; set => Object = value; }

  public ColorSetting(Color @default, string label, string? tooltip = null, Action<ColorSetting>? onChange = null, Func<ColorSetting, bool>? extraDefaultCheck = null) : base(@default, label, tooltip, setting => onChange?.Invoke((ColorSetting)setting), setting => extraDefaultCheck?.Invoke((ColorSetting)setting) ?? false)
  { }

  public override bool IsDefault() => ExtraDefaultCheck() || Value.ToHex().Equals(((Color)Default).ToHex());

  public override string ToString() => Value.ToHex();
}
