using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal abstract class WindowPlus : Window
    {
        private const float CloseButtonOffset = 55f;

        public override Vector2 InitialSize { get; }
        protected string Title { get; set; }

        protected WindowPlus(Vector2 size) : this(null, size)
        { }

        protected WindowPlus(string title = null, Vector2 size = default(Vector2))
        {
            draggable = true;
            doCloseX = true;
            doCloseButton = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            closeOnAccept = false;

            InitialSize = size == default(Vector2) ? new Vector2(800f, 600f) : size;
            Title = title;
        }

        protected abstract void DrawContent(Rect rect);

        public override void DoWindowContents(Rect rect)
        {
            var wordWrap = Text.WordWrap;
            Text.WordWrap = false;

            DrawContent(DrawTitle(rect));

            Text.WordWrap = wordWrap;
        }

        private Rect DrawTitle(Rect rect)
        {
            if (Title.NullOrEmpty()) { return rect; }

            var header = new ListingPlus();

            header.Begin(rect);
            header.Label(Title, font: GameFont.Medium);
            header.GapLine();
            header.End();

            var contentRect = new Rect(rect.x, rect.y + header.CurHeight, rect.width, rect.height - header.CurHeight);
            if (doCloseButton) { contentRect.height -= CloseButtonOffset; }

            return contentRect;
        }
    }
}
