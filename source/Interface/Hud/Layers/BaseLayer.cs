using System.Xml.Linq;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public abstract class BaseLayer(HudArgs args)
{
  public abstract LayoutElementType Type { get; }

  public abstract string Id { get; }

  public abstract float Prepare();
  public abstract bool Draw(Rect rect);

  public abstract LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent);

  protected abstract XElement StartXml();

  public abstract void Flush();

  public HudArgs Args { get; } = args;

  protected BaseLayer(XElement xml) : this(new HudArgs(xml))
  { }

  public XElement ToXml() => Args.ToXml(StartXml());

  protected bool IsTargetted() => Args.Targets.HasTarget(Active.Target);
}
