using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using RimHUD.Interface.Dialog;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal class HudPanel : HudContainer
    {
        public const string Name = "Panel";
        public override string ElementName { get; } = Name;

        public override bool FillHeight { get; }
        public override HudTarget Targets { get; }

        private readonly HudRow[] _rows;
        private float[] _heights;

        public HudPanel(XElement xe, bool? fillHeight)
        {
            Targets = TargetsFromXml(xe);

            var rows = new List<HudRow>();
            foreach (var element in xe.Elements())
            {
                if (element.Name != HudRow.Name)
                {
                    Mod.Error($"Invalid HUD container element '{element.Name}' instead of '{HudRow.Name}'");
                    continue;
                }

                rows.Add(new HudRow(element));
            }

            FillHeight = fillHeight ?? false;

            _rows = rows.ToArray();
        }

        public override float Prepare(PawnModel model)
        {
            if (_rows.Length == 0 || !IsTargetted(model)) { return 0f; }
            _heights = _rows.Select(row => row.Prepare(model)).ToArray();

            return FillHeight ? -1f : _heights.Sum() + (HudLayout.Padding * (_rows.Length - 1));
        }

        public override bool Draw(Rect rect)
        {
            if (_rows.Length == 0) { return false; }

            var grid = rect.GetVGrid(HudLayout.Padding, _heights);
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
            var xml = new XElement(ElementName);
            foreach (var row in _rows) { xml.Add(row.ToXml()); }
            return xml;
        }

        public override LayoutItem GetLayoutItem(LayoutEditor editor, LayoutItem parent)
        {
            var item = new LayoutItem(editor, parent, this);
            foreach (var row in _rows) { item.Contents.Add(row.GetLayoutItem(editor, item)); }

            return item;
        }
    }
}
