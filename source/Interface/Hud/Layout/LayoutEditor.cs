using RimHUD.Interface.Hud.Layers;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layout
{
  public class LayoutEditor
  {
    public LayoutElement Root { get; }
    public LayoutElement Selected { get; set; }
    public bool HasSelected => Selected != null;
    public bool CanAddContainer => Selected != null && Selected.Type == LayoutElementType.Stack;
    public bool CanAddRow => Selected != null && (Selected.Type == LayoutElementType.Panel || Selected.Type == LayoutElementType.Row);
    public bool CanAddWidget => Selected != null && Selected.Type == LayoutElementType.Row;

    public bool Docked { get; }

    private float _lastHeight = -1f;
    private Vector2 _scrollPosition;

    public LayoutEditor(bool docked)
    {
      Docked = docked;
      Root = (docked ? LayoutLayer.Docked : LayoutLayer.Floating).GetLayoutItem(this, null);
    }

    public void Draw(Rect rect)
    {
      WidgetsPlus.DrawContainer(rect);

      var viewRect = new Rect(rect.x, rect.y, rect.width - WidgetsPlus.ScrollbarWidth, _lastHeight > -1f ? _lastHeight : 99999f);
      Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, viewRect);
      _lastHeight = Root.Draw(viewRect.x, viewRect.y, viewRect.width);
      Verse.Widgets.EndScrollView();
    }

    public void Add(LayoutElement container, string variant = null, bool selectNew = false)
    {
      var newItem = new LayoutElement(container.Type, container.Id, container.Def, variant, this, Selected, container.CustomLabel);
      Selected.Contents.Insert(0, newItem);
      if (selectNew) { Selected = newItem; }
      Update();
    }

    public void AddSibling(LayoutElement container, string variant = null)
    {
      var newItem = new LayoutElement(container.Type, container.Id, container.Def, variant, this, Selected.Parent);
      Selected.Parent.Contents.Insert(Selected.Index + 1, newItem);
      Selected = newItem;
      Update();
    }

    public void Update()
    {
      var updated = LayoutLayer.FromLayoutView(this);
      if (Docked) { LayoutLayer.Docked = updated; }
      else { LayoutLayer.Floating = updated; }
    }
  }
}
