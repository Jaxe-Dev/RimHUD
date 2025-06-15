using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public sealed class HStackLayer(XElement xml) : StackLayer(xml)
{
  public const string Name = "HStack";

  public override string Id => Name;

  public override float Prepare()
  {
    if (Children.Length is 0 || !IsTargetted()) { return 0f; }

    var maxHeight = Children.Select(static container => container.Prepare()).Max();
    return Args.FillHeight ? -1f : maxHeight;
  }

  public override bool Draw(Rect rect)
  {
    if (Children.Length is 0) { return false; }

    var grid = rect.GetHGrid(LayoutLayer.Padding, Enumerable.Repeat(-1f, Children.Length).ToArray());
    var index = 1;

    foreach (var container in Children)
    {
      container.Draw(grid[index]);
      index++;
    }

    return true;
  }
}
