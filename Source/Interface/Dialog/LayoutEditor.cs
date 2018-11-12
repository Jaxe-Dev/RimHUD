using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class LayoutEditor
    {
        public LayoutItem Root { get; }
        public LayoutItem Selected { get; set; }
        public bool HasSelected => Selected != null;
        public bool CanAddContainer => (Selected != null) && (Selected.Type == LayoutItemType.Stack);
        public bool CanAddRow => (Selected != null) && (Selected.Type == LayoutItemType.Panel);
        public bool CanAddElement => (Selected != null) && (Selected.Type == LayoutItemType.Row);

        public bool Docked { get; }

        private float _lastHeight = -1f;
        private Vector2 _scrollPosition;

        public LayoutEditor(bool docked)
        {
            Docked = docked;
            Root = (docked ? HudLayout.Docked : HudLayout.Floating).GetLayoutItem(this, null);
        }

        public void Draw(Rect rect)
        {
            Widgets.DrawOptionUnselected(rect);

            var viewRect = new Rect(rect.x, rect.y, rect.width - GUIPlus.ScrollbarWidth, _lastHeight > -1f ? _lastHeight : 99999f);
            Widgets.BeginScrollView(rect, ref _scrollPosition, viewRect);
            _lastHeight = Root.Draw(viewRect.x, viewRect.y, viewRect.width);
            Widgets.EndScrollView();
        }

        public void Add(LayoutItem container)
        {
            Selected.Contents.Insert(0, new LayoutItem(container.Type, container.Id, container.Def, this, Selected));
            Update();
        }

        public void Update()
        {
            var updated = HudLayout.FromLayoutView(this);
            if (Docked) { HudLayout.Docked = updated; }
            else { HudLayout.Floating = updated; }
        }
    }
}
