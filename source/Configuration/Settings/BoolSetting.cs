using System;

namespace RimHUD.Configuration.Settings;

public sealed class BoolSetting(bool @default, string label, string? tooltip = null, Action<ValueSetting>? onChange = null) : ValueSetting(@default, label, tooltip, onChange)
{
  [Setting(typeof(bool))]
  public bool Value { get => (bool)Object; set => Object = value; }
}
