using System;
using UnityEngine;

namespace RimHUD.Data.Configuration
{
  internal class ColorOption : ThemeOption
  {
    [Attributes.Option(typeof(Color))]
    public Color Value { get => (Color) Object; set => Object = value; }

    public ColorOption(Color @default, string label, string tooltip = null, Action<ThemeOption> onChange = null) : base(@default, label, tooltip, onChange)
    { }
  }
}
