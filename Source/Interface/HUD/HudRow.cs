using System.Linq;
using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Interface.Dialog;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudRow : HudComponent
    {
        public const string Name = "Row";

        public override string ElementName => "Row";

        private readonly HudElement[] _elements;
        private bool _visible;

        public override HudTarget Targets { get; }

        public HudRow(XElement xe)
        {
            Targets = TargetsFromXml(xe);
            _elements = xe.Elements().Select(HudElement.FromXml).Where(element => element != null).ToArray();
        }

        public override float Prepare(PawnModel model)
        {
            _visible = false;
            if ((_elements.Length == 0) || !IsTargetted(model)) { return 0f; }

            var maxHeight = 0f;
            foreach (var element in _elements)
            {
                element.Build(model);
                maxHeight = Mathf.Max(maxHeight, element.Widget.Height);
            }

            _visible = maxHeight > 0f;
            return maxHeight;
        }

        public override bool Draw(Rect rect)
        {
            if (!_visible || (_elements.Length == 0)) { return false; }

            var grid = rect.GetHGrid(GUIPlus.MediumPadding, Enumerable.Repeat(-1f, _elements.Length).ToArray());

            var index = 1;
            foreach (var element in _elements)
            {
                if (element.Widget.Height <= 0f) { continue; }
                if (element.Draw(grid[index])) { index++; }
            }

            return index > 1;
        }

        public void Flush()
        {
            foreach (var element in _elements) { element.Flush(); }
        }

        public override XElement ToXml()
        {
            var xml = new XElement(Name);

            var targets = Targets.ToId();
            if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }

            foreach (var element in _elements) { xml.Add(element.ToXml()); }
            return xml;
        }

        public override LayoutItem GetLayoutItem(LayoutView view, LayoutItem parent)
        {
            var item = new LayoutItem(view, parent, this);
            foreach (var element in _elements) { item.Contents.Add(element.GetLayoutItem(view, item)); }

            return item;
        }
    }
}
