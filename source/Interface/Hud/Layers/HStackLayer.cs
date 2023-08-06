using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers
{
  public class HStackLayer : StackLayer
  {
    public const string Name = "HStack";
    public override string Id { get; } = Name;

    public HStackLayer(XElement xe, bool? fillHeight) : base(xe, fillHeight)
    { }

    public override float Prepare(PawnModel owner)
    {
      if (Containers.Length == 0 || !IsTargetted(owner)) { return 0f; }
      var maxHeight = Containers.Select(container => container.Prepare(owner)).Max();
      return FillHeight ? -1f : maxHeight;
    }

    public override bool Draw(Rect rect)
    {
      if (Containers.Length == 0) { return false; }

      var grid = rect.GetHGrid(LayoutLayer.Padding, Enumerable.Repeat(-1f, Containers.Length).ToArray());
      var index = 1;
      foreach (var container in Containers)
      {
        container.Draw(grid[index]);
        index++;
      }

      return true;
    }
  }
}
