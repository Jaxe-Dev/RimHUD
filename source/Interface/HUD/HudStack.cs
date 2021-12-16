using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Interface.Dialog;

namespace RimHUD.Interface.HUD
{
  internal abstract class HudStack : HudContainer
  {
    public const string FillAttributeName = "FillHeight";

    public override HudTarget Targets { get; }
    public override bool FillHeight { get; }

    protected readonly HudContainer[] Containers;

    protected HudStack(XElement xe, bool? fillHeight)
    {
      Targets = TargetsFromXml(xe);

      var containers = new List<HudContainer>();
      foreach (var element in xe.Elements())
      {
        var elementFillHeight = element.Attribute(FillAttributeName)?.Value.ToBool() ?? false;

        var stack = FromXml(element, elementFillHeight);
        if (stack != null) { containers.Add(stack); }
        else if (element.Name == HudPanel.Name) { containers.Add(new HudPanel(element, elementFillHeight)); }
        else { Mod.Error($"Invalid HUD container '{element.Name}'"); }
      }

      FillHeight = fillHeight ?? false;

      Containers = containers.ToArray();
    }

    public override void Flush()
    {
      foreach (var container in Containers) { container.Flush(); }
    }

    private static HudStack FromXml(XElement xml, bool? fillHeight)
    {
      if (xml.Name == HudHStack.Name) { return new HudHStack(xml, fillHeight); }
      return xml.Name == HudVStack.Name ? new HudVStack(xml, fillHeight) : null;
    }

    public override XElement ToXml()
    {
      var xml = new XElement(ElementName);

      if (FillHeight) { xml.Add(new XAttribute(FillAttributeName, FillHeight)); }

      foreach (var container in Containers) { xml.Add(container.ToXml()); }
      return xml;
    }

    public override LayoutItem GetLayoutItem(LayoutEditor editor, LayoutItem parent)
    {
      var item = new LayoutItem(editor, parent, this);
      foreach (var container in Containers) { item.Contents.Add(container.GetLayoutItem(editor, item)); }

      return item;
    }
  }
}
