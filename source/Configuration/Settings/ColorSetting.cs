using System;
using UnityEngine;

namespace RimHUD.Configuration.Settings
{
  public sealed class ColorSetting : ValueSetting
  {
    [Setting(typeof(Color))]
    public Color Value { get => (Color)Object; set => Object = value; }

    public ColorSetting(Color @default, string label, string? tooltip = null, Action<ValueSetting>? onChange = null) : base(@default, label, tooltip, onChange)
    { }
  }
}
