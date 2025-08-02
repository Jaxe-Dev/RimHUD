using System;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

public sealed class RangeSetting : ValueSetting
{
  private readonly Func<int, string>? _format;

  public int Min { get; private set; }
  public int Max { get; private set; }

  [Setting(typeof(int))] public int Value { get => (int)Object; set => Object = Mathf.Clamp(value, Min, Max); }

  public RangeSetting(int @default, int min, int max, string label, Func<int, string>? format = null, string? tooltip = null, Action<RangeSetting>? onChange = null, Func<RangeSetting, bool>? extraDefaultCheck = null) : base(@default, label, tooltip, setting => onChange?.Invoke((RangeSetting)setting), setting => extraDefaultCheck?.Invoke((RangeSetting)setting) ?? false)
  {
    _format = format;
    Min = min;
    Max = max;
  }

  public override string ToString() => _format is null ? Value.ToString() : _format(Value);

  public void SetMinMax(int min, int max)
  {
    Min = min;
    Max = max;
    Value = Mathf.Clamp(Value, Min, Max);
  }
}
