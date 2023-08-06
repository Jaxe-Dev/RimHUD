using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers
{
  public class VStackLayer : StackLayer
  {
    public const string Name = "VStack";
    public override string Id { get; } = Name;

    private float[] _heights;

    public VStackLayer(XElement xe, bool? fillHeight) : base(xe, fillHeight)
    { }

    public override float Prepare(PawnModel owner)
    {
      if (Containers.Length == 0 || !IsTargetted(owner)) { return 0f; }

      var list = new List<float>();
      var totalFixedHeight = 0f;
      var totalVisible = 0;

      foreach (var container in Containers)
      {
        var height = container.Prepare(owner);
        list.Add(height);
        if ((int)height != -1) { totalFixedHeight += height; }
        if ((int)height != 0) { totalVisible++; }
      }

      _heights = list.ToArray();

      return FillHeight ? -1f : totalFixedHeight + (LayoutLayer.Padding * (totalVisible - 1));
    }

    public override bool Draw(Rect rect)
    {
      if (Containers.Length == 0) { return false; }

      var grid = rect.GetVGrid(LayoutLayer.Padding, _heights);
      var index = 1;
      foreach (var container in Containers)
      {
        if (_heights[index - 1] != 0f) { container.Draw(grid[index]); }
        index++;
      }

      return true;
    }
  }
}
