using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class HudListing
    {
        public const float DefaultPadding = 2f;
        public const float DefaultLinePadding = 5f;
        public const float DefaultHeight = 13f;
        public const float DefaultNullValue = -1f;
        public const TextAnchor DefaultTextAnchor = TextAnchor.MiddleLeft;

        public Rect Bounds { get; private set; }

        public float CurX { get; private set; }
        public float CurY { get; private set; }

        public float RemainingWidth => Bounds.xMax - CurX;

        private float _largestHeight = -1f;
        public Rect CurrentRect { get; private set; }

        public void Bind(Rect bounds)
        {
            Bounds = bounds;
            CurX = bounds.x;
            CurY = bounds.y;
        }

        public void Next(float width = DefaultNullValue, float height = DefaultNullValue, Vector2? offset = null)
        {
            var newX = CurX + (offset?.x ?? 0f);
            var newY = CurY + (offset?.y ?? 0f);
            var newWidth = width < 0f ? RemainingWidth + width : width;
            var newHeight = height < 0f ? (_largestHeight < 0f ? DefaultHeight : _largestHeight) : height;

            CurrentRect = new Rect(newX, newY, newWidth, newHeight);
            CurX = CurrentRect.xMax + 1f;

            _largestHeight = Mathf.Max(_largestHeight, CurrentRect.height);
        }
        public void Pad(float padding = DefaultPadding) => CurX += padding < 0f ? RemainingWidth - padding : padding;

        public void Gap(float padding = DefaultPadding)
        {
            CurX = Bounds.x;
            CurY += (_largestHeight < 0f ? 0f : _largestHeight) + 1f + padding;
            _largestHeight = -1f;
        }

        public void GapLine(float padding = DefaultLinePadding)
        {
            CurX = Bounds.x;

            var previousColor = GUI.color;
            GUI.color = Theme.LineColor;
            Widgets.DrawLineHorizontal(CurX, CurY, Bounds.width);
            GUI.color = previousColor;

            Gap(padding);
        }

        public void DrawLabel(string text, int fontSize = Theme.BaseFontSize, TextAnchor alignment = DefaultTextAnchor, Color? color = null, bool wrap = false)
        {
            var previousColor = GUI.color;
            GUI.color = color ?? Theme.BaseFontColor;
            GUI.Label(CurrentRect, text, GUIPlus.GetGUIStyle(alignment, fontSize, wrap));
            GUI.color = previousColor;
        }

        public void DrawBar(float percentage) => GUIPlus.DrawBar(CurrentRect, percentage, GetBarColorFromPercentage(percentage));

        public void DrawIcon(Texture texture, Color? color = null)
        {
            var previousColor = GUI.color;
            if (color != null) { GUI.color = color.Value; }
            GUI.DrawTexture(CurrentRect, texture);
            GUI.color = previousColor;
        }

        private static Color GetBarColorFromPercentage(float percentage) => Color.Lerp(Theme.BarLowColor, Theme.BarMainColor, percentage);
    }
}
