using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layout;

public sealed class LayoutEditor
{
  public LayoutElement Root { get; }
  public LayoutElement? Selected { get; set; }

  private float _lastHeight = -1f;
  private Vector2 _scrollPosition;

  public LayoutEditor() => Root = State.CurrentLayout.GetLayoutItem(this, null);

  public void Draw(Rect rect)
  {
    WidgetsPlus.DrawContainer(rect);

    var scrollRect = new Rect(rect.x, rect.y, rect.width - WidgetsPlus.ScrollbarWidth, _lastHeight > -1f ? _lastHeight : 99999f);
    Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, scrollRect);
    _lastHeight = Root.Draw(scrollRect.x, scrollRect.y, scrollRect.width);
    Verse.Widgets.EndScrollView();
  }

  public void Add(LayoutElement container, bool selectNew = false)
  {
    if (Selected is null) { return; }

    var newItem = new LayoutElement(container, this, Selected);
    Selected.Children.Insert(0, newItem);
    if (selectNew) { Selected = newItem; }
    Update();
  }

  public void AddSibling(LayoutElement container)
  {
    if (Selected?.Parent is null) { return; }

    var newItem = new LayoutElement(container, this, Selected.Parent);
    Selected.Parent.Children.Insert(Selected.Index + 1, newItem);
    Selected = newItem;
    Update();
  }

  public void Update()
  {
    LayoutPreset.Active = null;

    var updated = LayoutLayer.FromLayoutView(this);

    if (Theme.DockedMode.Value) { LayoutLayer.Docked = updated; }
    else { LayoutLayer.Floating = updated; }
  }
}
