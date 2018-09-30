using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface
{
    internal class TextStyle : IDefaultable
    {
        private readonly TextStyle _baseStyle;
        public string Label { get; }
        public GUIStyle GUIStyle { get; private set; }
        public int ActualSize => GUIStyle.fontSize;

        [Persistent.Option("Size")] public RangeOption Size { get; }
        [Persistent.Option("Height")] public RangeOption Height { get; }

        public float LineHeight;

        public TextStyle(string label, TextStyle baseStyle, int size, int sizeMin, int sizeMax, int height, int heightMin, int heightMax)
        {
            _baseStyle = baseStyle;
            Label = label;

            Size = new RangeOption(size, sizeMin, sizeMax, Lang.Get("TextStyle.Size"), value => ((baseStyle != null) && (Size.Value > 0) ? "+" : null) + value, onChange: _ => UpdateStyle());
            Height = new RangeOption(height, heightMin, heightMax, Lang.Get("TextStyle.Height"), value => value + "%", onChange: _ => UpdateStyle());

            UpdateStyle();
        }

        private void UpdateStyle()
        {
            GUIStyle = _baseStyle?.GUIStyle.ResizedBy(Size.Value) ?? Theme.BaseGUIStyle.SetTo(Size.Value);
            LineHeight = GUIStyle.lineHeight * Height.Value.ToPercentageFloat();
        }

        public void ToDefault()
        {
            Size.ToDefault();
            Height.ToDefault();
        }
    }
}
