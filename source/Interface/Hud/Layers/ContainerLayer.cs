using System.Xml.Linq;
using HarmonyLib;
using RimHUD.Interface.Hud.Layout;

namespace RimHUD.Interface.Hud.Layers;

public abstract class ContainerLayer<T>(XElement xml) : BaseLayer(xml)
  where T : BaseLayer
{
  protected abstract T[] Children { get; }

  public override bool IsVisible() => Children.Length > 0 && IsTarget();

  public override void Flush() => Children.Do(static item => item.Flush());

  public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement? parent) => new(this, editor, parent, Children);

  protected override XElement StartXml()
  {
    var xml = new XElement(Id);
    foreach (var child in Children) { xml.Add(child.ToXml()); }
    return xml;
  }
}
