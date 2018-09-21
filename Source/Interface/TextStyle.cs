using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface
{
    internal class TextStyle
    {
        private readonly TextStyle _baseStyle;
        public string Label { get; }
        public GUIStyle GUIStyle => _baseStyle?.GUIStyle.ResizedBy(Size.Value) ?? Theme.BaseGUIStyle.SetTo(Size.Value);

        [Persistent.Option("Size")]
        public RangeOption Size { get; }

        [Persistent.Option("Height")]
        public RangeOption Height { get; }

        public float LineHeight => GUIStyle.lineHeight * Height.Value.ToPercentageFloat();

        public TextStyle(string label, TextStyle baseStyle, int size, int sizeMin, int sizeMax, int height, int heightMin, int heightMax)
        {
            _baseStyle = baseStyle;
            Label = label;

            Size = new RangeOption(size, sizeMin, sizeMax, Lang.Get("TextStyle.Size"), value => ((baseStyle != null) && (Size.Value > 0) ? "+" : null) + value);
            Height = new RangeOption(height, heightMin, heightMax, Lang.Get("TextStyle.Height"), value => value + "%");
        }

        public void ToDefault()
        {
            Size.ToDefault();
            Height.ToDefault();
        }
    }
}
