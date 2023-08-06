using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;

namespace RimHUD.Interface.Hud.Layers
{
  public abstract class StackLayer : ContainerLayer
  {
    public const string FillAttributeName = "FillHeight";

    public static readonly LayoutElement[] LayoutElements =
    {
      new LayoutElement(LayoutElementType.Stack, VStackLayer.Name),
      new LayoutElement(LayoutElementType.Stack, HStackLayer.Name)
    };

    public override HudTarget Targets { get; }
    public override bool FillHeight { get; }

    protected readonly ContainerLayer[] Containers;

    protected StackLayer(XElement xe, bool? fillHeight)
    {
      Targets = TargetsFromXml(xe);

      var containers = new List<ContainerLayer>();
      foreach (var element in xe.Elements())
      {
        var elementFillHeight = element.Attribute(FillAttributeName)?.Value.ToBool() ?? false;

        var stack = FromXml(element, elementFillHeight);
        if (stack != null) { containers.Add(stack); }
        else if (element.Name == PanelLayer.Name) { containers.Add(new PanelLayer(element, elementFillHeight)); }
        else { Mod.Error($"Invalid HUD container '{element.Name}'"); }
      }

      FillHeight = fillHeight ?? false;

      Containers = containers.ToArray();
    }

    public override void Flush()
    {
      foreach (var container in Containers) { container.Flush(); }
    }

    private static StackLayer FromXml(XElement xml, bool? fillHeight)
    {
      if (xml.Name == HStackLayer.Name) { return new HStackLayer(xml, fillHeight); }
      return xml.Name == VStackLayer.Name ? new VStackLayer(xml, fillHeight) : null;
    }

    public override XElement ToXml()
    {
      var xml = new XElement(Id);

      if (FillHeight) { xml.Add(new XAttribute(FillAttributeName, FillHeight)); }

      foreach (var container in Containers) { xml.Add(container.ToXml()); }
      return xml;
    }

    public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent)
    {
      var item = new LayoutElement(editor, parent, this);
      foreach (var container in Containers) { item.Contents.Add(container.GetLayoutItem(editor, item)); }

      return item;
    }
  }
}
