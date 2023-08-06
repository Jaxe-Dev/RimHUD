using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layers
{
  public class RowLayer : BaseLayer
  {
    public const string Name = "Row";

    public static readonly LayoutElement LayoutElement = new LayoutElement(LayoutElementType.Row, Name);

    public override string Id => "Row";

    private readonly WidgetLayer[] _controls;
    private bool _visible;

    public override HudTarget Targets { get; }

    public RowLayer(XElement xe)
    {
      Targets = TargetsFromXml(xe);
      _controls = xe.Elements().Select(WidgetLayer.FromXml).Where(control => control != null).ToArray();
    }

    public override float Prepare(PawnModel owner)
    {
      _visible = false;
      if (_controls.Length == 0 || !IsTargetted(owner)) { return 0f; }

      var maxHeight = 0f;
      foreach (var control in _controls)
      {
        control.Build(owner);
        maxHeight = Mathf.Max(maxHeight, control.Widget.Height);
      }

      _visible = maxHeight > 0f;
      return maxHeight;
    }

    public override bool Draw(Rect rect)
    {
      if (!_visible || _controls.Length == 0) { return false; }

      var grid = rect.GetHGrid(WidgetsPlus.MediumPadding, Enumerable.Repeat(-1f, _controls.Length).ToArray());

      var index = 1;
      foreach (var control in _controls)
      {
        if (control.Widget.Height <= 0f) { continue; }
        if (control.Draw(grid[index])) { index++; }
      }

      return index > 1;
    }

    public void Flush()
    {
      foreach (var control in _controls) { control.Flush(); }
    }

    public override XElement ToXml()
    {
      var xml = new XElement(Name);

      var targets = Targets.ToId();
      if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }

      foreach (var control in _controls) { xml.Add(control.ToXml()); }
      return xml;
    }

    public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent)
    {
      var item = new LayoutElement(editor, parent, this);
      foreach (var control in _controls) { item.Contents.Add(control.GetLayoutItem(editor, item)); }

      return item;
    }
  }
}
