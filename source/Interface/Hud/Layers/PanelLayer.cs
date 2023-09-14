using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers
{
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
      if (Children.Length is 0 || !IsTargetted()) { return 0f; }
      _heights = Children.Select(static row => row.Prepare()).ToArray();

      return Args.FillHeight ? -1f : _heights.Sum() + (LayoutLayer.Padding * (Children.Length - 1));
    }

    public override bool Draw(Rect rect)
    {
      if (Children.Length is 0) { return false; }

      var grid = rect.GetVGrid(LayoutLayer.Padding, _heights!);
      var index = 0;
      foreach (var item in Children)
      {
        index++;
        item.Draw(grid[index]);
      }

      return true;
    }
  }
}
