using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layout;

namespace RimHUD.Interface.Hud.Layers;

public abstract class StackLayer : ContainerLayer<BaseLayer>
{
  public static readonly LayoutElement[] LayoutElements =
  [
    new LayoutElement(LayoutElementType.Stack, VStackLayer.Name),
    new LayoutElement(LayoutElementType.Stack, HStackLayer.Name)
  ];

  public override LayoutElementType Type => LayoutElementType.Stack;

  protected override BaseLayer[] Children { get; }

  protected StackLayer(XElement xml) : base(xml)
  {
    var children = new List<BaseLayer>();
    foreach (var element in xml.Elements())
    {
      switch (element.Name.ToString())
      {
        case HStackLayer.Name:
          children.Add(new HStackLayer(element));
          break;
        case VStackLayer.Name:
          children.Add(new VStackLayer(element));
          break;
        case PanelLayer.Name:
          children.Add(new PanelLayer(element));
          break;
        default:
          Report.Error($"Invalid container '{element.Name}'.");
          break;
      }
    }

    Children = children.ToArray();
  }

  public override bool IsVisible() => Children.Length > 0 && IsTarget();
}
