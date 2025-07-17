using System;

namespace RimHUD.Configuration.Settings;

public sealed class BoolSetting : ValueSetting
{
  [Setting(typeof(bool))] public bool Value { get => (bool)Object; set => Object = value; }

  public BoolSetting(bool @default, string label, string? tooltip = null, Action<ValueSetting>? onChange = null) : base(@default, label, tooltip, onChange)
  { }
}
