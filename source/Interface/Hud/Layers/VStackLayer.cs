using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public class VStackLayer(XElement xml) : StackLayer(xml)
{
  public const string Name = "VStack";
  public override string Id => Name;

  private float[]? _heights;

  public override float Prepare()
  {
    if (Children.Length is 0 || !IsTargetted()) { return 0f; }

    var list = new List<float>();
    var totalFixedHeight = 0f;
    var totalVisible = 0;

    foreach (var height in Children.Select(static container => container.Prepare()))
    {
      list.Add(height);
      if ((int)height is not -1) { totalFixedHeight += height; }
      if ((int)height is not 0) { totalVisible++; }
    }

    _heights = list.ToArray();

    return Args.FillHeight ? -1f : totalFixedHeight + (LayoutLayer.Padding * (totalVisible - 1));
  }

  public override bool Draw(Rect rect)
  {
    if (Children.Length is 0 || _heights is null || !IsTargetted()) { return false; }

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
