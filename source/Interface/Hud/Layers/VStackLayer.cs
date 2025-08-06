using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public class VStackLayer : StackLayer
{
  public const string Name = "VStack";
  public override string Id => Name;

  private float[]? _heights;

  public VStackLayer(XElement xml) : base(xml)
  { }

  public override float Prepare()
  {
    if (!IsVisible()) { return 0f; }

    var list = new List<float>();
    var totalFixedHeight = 0f;
    var totalVisible = 0;

    foreach (var height in Children.Select(static container => container.Prepare()))
    {
      list.Add(height);
      if (height is not -1f) { totalFixedHeight += height; }
      if (height is not 0f) { totalVisible++; }
    }

    _heights = list.ToArray();

    return Args.FillHeight ? -1f : totalFixedHeight + (LayoutLayer.Padding * (totalVisible - 1));
  }

  public override bool Draw(Rect rect)
  {
    if (!IsVisible() || _heights is null) { return false; }

    var grid = rect.GetVGrid(LayoutLayer.Padding, _heights);
    var index = 1;

    foreach (var child in Children)
    {
      if (_heights[index - 1] is not 0f) { child.Draw(grid[index]); }
      index++;
    }

    return true;
  }
}
