using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;

namespace RimHUD.Interface.Hud.Layers
{
  public class PanelLayer : ContainerLayer
  {
    public const string Name = "Panel";

    public static readonly LayoutElement LayoutElement = new LayoutElement(LayoutElementType.Panel, Name);

    public override string Id { get; } = Name;

    public override bool FillHeight { get; }
    public override HudTarget Targets { get; }

    private readonly RowLayer[] _rows;
    private float[] _heights;

    public PanelLayer(XElement xe, bool? fillHeight)
    {
      Targets = TargetsFromXml(xe);

      var rows = new List<RowLayer>();
      foreach (var element in xe.Elements())
      {
        if (element.Name != RowLayer.Name)
        {
          Mod.Error($"Invalid HUD container element '{element.Name}' instead of '{RowLayer.Name}'");
          continue;
        }

        rows.Add(new RowLayer(element));
      }

      FillHeight = fillHeight ?? false;

      _rows = rows.ToArray();
    }

    public override float Prepare(PawnModel owner)
    {
      if (_rows.Length == 0 || !IsTargetted(owner)) { return 0f; }
      _heights = _rows.Select(row => row.Prepare(owner)).ToArray();

      return FillHeight ? -1f : _heights.Sum() + (LayoutLayer.Padding * (_rows.Length - 1));
    }

    public override bool Draw(Rect rect)
    {
      if (_rows.Length == 0) { return false; }

      var grid = rect.GetVGrid(LayoutLayer.Padding, _heights);
      var index = 0;
      foreach (var row in _rows)
      {
        index++;
        row.Draw(grid[index]);
      }

      return true;
    }

    public override void Flush()
    {
      foreach (var row in _rows) { row.Flush(); }
    }

    public override XElement ToXml()
    {
      var xml = new XElement(Id);
      foreach (var row in _rows) { xml.Add(row.ToXml()); }
      return xml;
    }

    public override LayoutElement GetLayoutItem(LayoutEditor editor, LayoutElement parent)
    {
      var item = new LayoutElement(editor, parent, this);
      foreach (var row in _rows) { item.Contents.Add(row.GetLayoutItem(editor, item)); }

      return item;
    }
  }
}
