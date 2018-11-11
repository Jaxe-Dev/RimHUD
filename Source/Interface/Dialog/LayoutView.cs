using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class LayoutView
    {
        public LayoutItem Root { get; }
        public LayoutItem Selected { get; set; }

        private float _lastHeight = -1f;
        private Vector2 _scrollPosition;

        public LayoutView(HudContainer root) => Root = root.GetLayoutItem(this, null);

        public void Draw(Rect rect)
        {
            Widgets.DrawOptionUnselected(rect);

            var viewRect = new Rect(rect.x, rect.y, rect.width - GUIPlus.ScrollbarWidth, _lastHeight > -1f ? _lastHeight : 99999f);
            Widgets.BeginScrollView(rect, ref _scrollPosition, viewRect);
            _lastHeight = Root.Draw(viewRect.x, viewRect.y, viewRect.width);
            Widgets.EndScrollView();
        }
    }
}
