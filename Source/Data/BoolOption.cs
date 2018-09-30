using System;

namespace RimHUD.Data
{
    internal class BoolOption : ThemeOption
    {
        [Persistent.Option(typeof(bool))] public bool Value { get => (bool) Object; set => Object = value; }

        public BoolOption(bool @default, string label, string tooltip = null, Action<ThemeOption> onChange = null) : base(@default, label, tooltip, onChange)
        { }
    }
}
