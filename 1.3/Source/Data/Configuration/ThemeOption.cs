using System;
using RimHUD.Data.Storage;

namespace RimHUD.Data.Configuration
{
    internal abstract class ThemeOption : IDefaultable
    {
        private readonly Action<ThemeOption> _onChange;

        public object Default { get; }
        private object _object;
        public object Object
        {
            get => _object;
            protected set
            {
                if (Persistent.IsLoaded && Equals(_object, value)) { return; }

                _object = value;
                _onChange?.Invoke(this);
            }
        }

        public string Label { get; }
        public string Tooltip { get; }

        protected ThemeOption(object @default, string label, string tooltip, Action<ThemeOption> onChange)
        {
            Default = @default;
            Object = @default;

            Label = label;
            Tooltip = tooltip;

            _onChange = onChange;
        }

        public void ToDefault() => Object = Default;
        public void Refresh() => _onChange(this);
    }
}
