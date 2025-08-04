using System;

namespace RimHUD.Configuration.Settings;

public sealed class BoolSetting : ValueSetting
{
  [Setting(typeof(bool))] public bool Value { get => (bool)Object; set => Object = value; }

  public BoolSetting(bool @default, string label, string? tooltip = null, Action<BoolSetting>? onChange = null, Func<BoolSetting, bool>? saveCheck = null, bool canIncludeInPreset = false) : base(@default, label, tooltip, ConvertOnChange(onChange), ConvertSaveCheck(saveCheck), canIncludeInPreset)
  { }
}
