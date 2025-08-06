using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public sealed class PanelLayer : ContainerLayer<RowLayer>
{
  public const string Name = "Panel";

  public static readonly LayoutElement LayoutElement = new(LayoutElementType.Panel, Name);
  public override LayoutElementType Type => LayoutElementType.Panel;

  public override string Id => Name;

  protected override RowLayer[] Children { get; }

  private float[]? _heights;

  public PanelLayer(XElement xml) : base(xml)
  {
    var rows = new List<RowLayer>();
    foreach (var element in xml.Elements())
    {
      if (element.Name != RowLayer.Name)
      {
        Report.Error($"Invalid container element '{element.Name}' instead of '{RowLayer.Name}'.");
        continue;
      }
      rows.Add(new RowLayer(element));
    }
    Children = rows.ToArray();
  }

  public override float Prepare()
  {
    if (!IsVisible()) { return 0f; }

    var list = new List<float>(Children.Length);
    var totalHeight = 0f;

    foreach (var height in Children.Select(static child => child.Prepare()))
    {
      list.Add(height);
      totalHeight += height;
    }

    _heights = list.ToArray();
    return Args.FillHeight ? -1f : totalHeight + (LayoutLayer.Padding * (Children.Length - 1));
  }

  public override bool Draw(Rect rect)
  {
    if (!IsVisible() || _heights is null) { return false; }

    var grid = rect.GetVGrid(LayoutLayer.Padding, _heights.Where(static height => height > 0f).ToArray());
    var gridIndex = 1;

    for (var i = 0; i < Children.Length; i++)
    {
      if (_heights[i] <= 0f) { continue; }
      Children[i].Draw(grid[gridIndex]);

      gridIndex++;
    }

    return true;
  }
}
