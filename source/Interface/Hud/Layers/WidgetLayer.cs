using System.Xml.Linq;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layers
{
  public class WidgetLayer : BaseLayer
  {
    public const string DefNameAttribute = "DefName";
    public const string VariantAttribute = "Variant";

    private readonly string _type;
    public override string Id => _type;

    public string DefName { get; }
    public string Variant { get; }

    public override HudTarget Targets { get; }

    public IWidget Widget;

    private WidgetLayer(string type, string defName, string variant, HudTarget targets)
    {
      _type = type;
      DefName = defName;
      Variant = variant;
      Targets = targets;

      HudTimings.Add(this);
    }

    public override float Prepare(PawnModel owner) => Widget?.Height ?? 0f;

    public static WidgetLayer FromXml(XElement xe)
    {
      var id = xe.Name.ToString();
      var defName = xe.Attribute(DefNameAttribute)?.Value;

      if (!HudBuilder.IsValidType(id))
      {
        Mod.Error($"Invalid HUD widget id '{id}'. It is recommended to reset the settings to default.");
        return new WidgetLayer(id, defName, null, HudTarget.All);
      }

      var variant = xe.Attribute(VariantAttribute)?.Value;
      var targets = TargetsFromXml(xe);

      return new WidgetLayer(id, defName, variant, targets);
    }

    public void Build(PawnModel owner)
    {
      var widget = HudBuilder.GetWidget(owner, _type, DefName, Variant);
      Widget = IsTargetted(owner) ? widget : BlankWidget.Get(widget.Height);
    }

    public override bool Draw(Rect rect)
    {
      HudTimings.Update(this)?.Start();

      var result = Widget?.Draw(rect) ?? false;

      HudTimings.Update(this)?.Finish(rect);

      return result;
    }

    public void Flush() => Widget = null;

    public override XElement ToXml()
    {
      var xml = new XElement(_type);
      if (!DefName.NullOrEmpty()) { xml.Add(new XAttribute(DefNameAttribute, DefName)); }
      if (!Variant.NullOrEmpty()) { xml.Add(new XAttribute(VariantAttribute, Variant)); }

      var targets = Targets.ToId();
      if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }
      return xml;
    }

    public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent) => new LayoutElement(editor, parent, this);

    ~WidgetLayer() => HudTimings.Remove(this);
  }
}
