using System.Xml.Linq;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers
{
  public abstract class BaseLayer
  {
    public const string TargetAttribute = "Targets";

    public abstract string Id { get; }
    public abstract HudTarget Targets { get; }

    public abstract float Prepare(PawnModel owner);
    public abstract bool Draw(Rect rect);

    public abstract XElement ToXml();

    public abstract LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent);

    protected static HudTarget TargetsFromXml(XElement xe) => HudTargetUtility.FromId(xe.Attribute(TargetAttribute)?.Value);
    protected bool IsTargetted(PawnModel owner) => Targets.HasTarget(owner.Target);
  }
}
