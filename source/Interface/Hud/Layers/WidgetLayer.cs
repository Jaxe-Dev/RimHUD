using System.Linq;
using System.Xml.Linq;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers;

public sealed class WidgetLayer : BaseLayer
{
  public override LayoutElementType Type => LayoutElementType.Widget;

  public override string Id { get; }

  public IWidget? Widget;

  private WidgetLayer(string id, HudArgs args) : base(args)
  {
    Id = id;
    HudTimings.Add(this);
  }

  ~WidgetLayer() => HudTimings.Remove(this);

  public static WidgetLayer FromXml(XElement xml)
  {
    var id = xml.Name.ToString();
    var args = new HudArgs(xml);

    if (HudContent.IsValidId(id, args.DefName)) { return new WidgetLayer(id, args); }

    var root = xml.Ancestors().FirstOrDefault(element => element.Parent == xml.Document?.Root)?.Name;
    Report.ErrorOnce((root is null ? $"Invalid id '{id}'" : $"Layer '{root}' has invalid id '{id}'") + (args.DefName is null ? null : $" with def '{args.DefName}'") + ". It is recommended to reset your config to default.");

    args.Targets = LayerTarget.All;

    return new WidgetLayer(id, args);
  }

  protected override XElement StartXml() => new(Id);

  public override float Prepare() => Widget?.GetMaxHeight ?? 0f;

  public override bool Draw(Rect rect)
  {
    HudTimings.Update(this)?.Start();

    var result = Widget?.Draw(rect) ?? false;

    HudTimings.Update(this)?.Finish(rect);

    return result;
  }

  public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent) => new(this, editor, parent);

  public void Build()
  {
    var widget = HudContent.GetWidget(Id, Args);
    Widget = IsTargetted() ? widget : new BlankWidget(widget.GetMaxHeight);
  }

  public override void Flush() => Widget = null;
}
