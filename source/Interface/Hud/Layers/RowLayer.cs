using System.Linq;
using System.Xml.Linq;
using HarmonyLib;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public sealed class RowLayer(XElement xml) : ContainerLayer<WidgetLayer>(xml)
{
  public const string Name = "Row";

  public static readonly LayoutElement LayoutElement = new(LayoutElementType.Row, Name);

  public override LayoutElementType Type => LayoutElementType.Row;

  public override string Id => "Row";

  protected override WidgetLayer[] Children { get; } = xml.Elements().Select(WidgetLayer.FromXml).WhereNotNull().ToArray();

  private bool _visible;

  public override float Prepare()
  {
    _visible = false;
    if (Children.Length is 0 || !IsTargetted()) { return 0f; }

    var maxHeight = 0f;
    foreach (var child in Children)
    {
      child.Build();
      if (child.Widget is null) { continue; }
      maxHeight = Mathf.Max(maxHeight, child.Widget.GetMaxHeight);
    }

    _visible = maxHeight > 0f;

    return maxHeight;
  }

  public override bool Draw(Rect rect)
  {
    if (!_visible || Children.Length is 0) { return false; }

    var grid = rect.GetHGrid(GUIPlus.MediumPadding, Enumerable.Repeat(-1f, Children.Length).ToArray());

    var index = 1;
    Children.Where(static child => !(child.Widget!.GetMaxHeight <= 0f)).Where(child => child.Draw(grid[index])).Do(_ => index++);

    return index > 1;
  }
}
