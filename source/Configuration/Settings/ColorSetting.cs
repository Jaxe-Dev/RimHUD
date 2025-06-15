using System;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

public sealed class ColorSetting(Color @default, string label, string? tooltip = null, Action<ValueSetting>? onChange = null) : ValueSetting(@default, label, tooltip, onChange)
{
  [Setting(typeof(Color))]
  public Color Value { get => (Color)Object; set => Object = value; }
}
