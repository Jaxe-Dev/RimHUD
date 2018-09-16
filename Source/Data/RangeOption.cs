using System;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Data
{
    internal class RangeOption : Option<int>
    {
        private readonly Func<int, string> _format;
        private readonly Action _onChange;

        public int Min { get; private set; }
        public int Max { get; private set; }
        public new int Value
        {
            get => base.Value;
            set
            {
                base.Value = Mathf.Clamp(value, Min, Max);
                _onChange?.Invoke();
            }
        }

        public RangeOption(int @default, int min, int max, string label, Func<int, string> format = null, string tooltip = null, Action onChange = null) : base(@default, label, tooltip)
        {
            _format = format;
            _onChange = onChange;
            Min = min;
            Max = max;
        }

        public void ToDefault() => Value = Default;

        public void SetMinMax(int min, int max)
        {
            Min = min;
            Max = max;
            Value = Mathf.Clamp(Value, Min, Max);
        }

        public void FromString(string value) => Value = value.ToInt() ?? Value;

        public override string ToString() => _format == null ? Value.ToString() : _format(Value);
    }
}
