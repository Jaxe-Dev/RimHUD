using System;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

public sealed class RangeSetting(int @default, int min, int max, string label, Func<int, string>? format = null, string? tooltip = null, Action<ValueSetting>? onChange = null) : ValueSetting(@default, label, tooltip, onChange)
{
  public int Min { get; private set; } = min;
  public int Max { get; private set; } = max;

  [Setting(typeof(int))]
  public int Value { get => (int)Object; set => Object = Mathf.Clamp(value, Min, Max); }

  public override string ToString() => format is null ? Value.ToString() : format(Value);

  public void SetMinMax(int min, int max)
  {
    Min = min;
    Max = max;
    Value = Mathf.Clamp(Value, Min, Max);
  }
}
