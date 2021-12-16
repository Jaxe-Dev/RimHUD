using System;
using UnityEngine;

namespace RimHUD.Data.Configuration
{
  internal class RangeOption : ThemeOption
  {
    private readonly Func<int, string> _format;

    public int Min { get; private set; }
    public int Max { get; private set; }

    [Attributes.Option(typeof(int))]
    public int Value { get => (int) Object; set => Object = Mathf.Clamp(value, Min, Max); }

    public RangeOption(int @default, int min, int max, string label, Func<int, string> format = null, string tooltip = null, Action<ThemeOption> onChange = null) : base(@default, label, tooltip, onChange)
    {
      _format = format;
      Min = min;
      Max = max;
    }

    public void SetMinMax(int min, int max)
    {
      Min = min;
      Max = max;
      Value = Mathf.Clamp(Value, Min, Max);
    }

    public override string ToString() => _format == null ? Value.ToString() : _format(Value);
  }
}
