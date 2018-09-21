using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class Layout
    {
        public const float DefaultPadding = 2f;
        public const float DefaultLinePadding = 5f;
        public const float NullValue = -1f;

        protected readonly TextStyle DefaultStyle;

        public Rect Bounds { get; private set; }

        public float CurX { get; private set; }
        public float CurY { get; private set; }

        public float RemainingWidth => Bounds.xMax - CurX;

        private float _largestHeight = -1f;
        public Rect CurrentRect { get; private set; }

        public Layout(TextStyle defaultStyle) => DefaultStyle = defaultStyle;

        public void Bind(Rect bounds)
        {
            Bounds = bounds;
            CurX = bounds.x;
            CurY = bounds.y;
        }

        public void Next(TextStyle style, float width = NullValue) => Next(width, style.LineHeight);

        public void Next(float width = NullValue, float height = NullValue)
        {
            var newWidth = width < 0f ? RemainingWidth + width : width;
            var newHeight = height < 0f ? (_largestHeight < 0f ? DefaultStyle.LineHeight : _largestHeight) : height;

            CurrentRect = new Rect(CurX, CurY, newWidth, newHeight);
            CurX = CurrentRect.xMax + 1f;

            _largestHeight = Mathf.Max(_largestHeight, CurrentRect.height);
        }

        public void PadRight(float padding = DefaultPadding) => CurX += padding < 0f ? RemainingWidth - padding : padding;

        public void PadDown(float padding = DefaultPadding)
        {
            CurX = Bounds.x;
            CurY += (_largestHeight < 0f ? 0f : _largestHeight) + 1f + padding;
            _largestHeight = -1f;
        }

        public void PadLine(float padding = DefaultLinePadding)
        {
            CurX = Bounds.x;

            GUIPlus.SetColor(Theme.LineColor);
            Widgets.DrawLineHorizontal(CurX, CurY, Bounds.width);
            GUIPlus.ResetColor();

            PadDown(padding);
        }

        public void DrawLabel(string text, Color? color = null, TextStyle style = null) => GUIPlus.DrawLabel(CurrentRect, text, color, style ?? DefaultStyle);

        public void DrawBar(float percentage) => GUIPlus.DrawBar(CurrentRect, percentage, GetBarColorFromPercentage(percentage));

        public void DrawIcon(Texture texture, Color? color = null)
        {
            GUIPlus.SetColor(color);
            GUI.DrawTexture(CurrentRect, texture);
            GUIPlus.ResetColor();
        }

        private static Color GetBarColorFromPercentage(float percentage) => Color.Lerp(Theme.BarLowColor, Theme.BarMainColor, percentage);
    }
}
