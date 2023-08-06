using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Configuration
{
  public class TextStyle : IDefaultable
  {
    private readonly TextStyle _baseStyle;
    public string Label { get; }
    public GUIStyle GUIStyle { get; private set; }
    public int ActualSize => GUIStyle.fontSize;

    [Attributes.Option("Size")]
    public RangeOption Size { get; }
    [Attributes.Option("Height")]
    public RangeOption Height { get; }

    private readonly Action<TextStyle> _onChange;
    public float LineHeight;

    public TextStyle(string label, TextStyle baseStyle, int size, int sizeMin, int sizeMax, int height, int heightMin, int heightMax, Action<TextStyle> onChange = null)
    {
      _baseStyle = baseStyle;
      _onChange = onChange;
      Label = label;

      Size = new RangeOption(size, sizeMin, sizeMax, Lang.Get("Theme.TextStyle.Size"), value => (baseStyle != null && Size.Value > 0 ? "+" : null) + value, onChange: _ => UpdateStyle());
      Height = new RangeOption(height, heightMin, heightMax, Lang.Get("Theme.TextStyle.Height"), value => value + "%", onChange: _ => UpdateStyle());

      UpdateStyle();
    }

    public void UpdateStyle()
    {
      GUIStyle = _baseStyle?.GUIStyle.ResizedBy(Size.Value) ?? Theme.BaseGUIStyle.SetTo(Size.Value);
      LineHeight = GUIStyle.lineHeight * Height.Value.ToPercentageFloat();

      _onChange?.Invoke(this);
    }

    public void ToDefault()
    {
      Size.ToDefault();
      Height.ToDefault();
    }
  }
}
