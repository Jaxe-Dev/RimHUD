using System.Linq;
using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudRow : HudComponent
    {
        public const string Name = "Row";

        private readonly HudElement[] _elements;
        public bool HasElements => _elements.Length > 0;
        private bool _visible;

        protected override HudTarget Targets { get; }

        public HudRow(XElement xe)
        {
            Targets = TargetsFromXml(xe);
            _elements = xe.Elements().Select(HudElement.FromXml).Where(element => element != null).ToArray();
        }

        public override float Prepare(PawnModel model)
        {
            _visible = false;
            if (!HasElements || !IsTargetted(model)) { return 0f; }

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

        public override XElement ToXml()
        {
            var xml = new XElement(Name);

            var targets = Targets.ToId();
            if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }

            foreach (var element in _elements) { xml.Add(element.ToXml()); }
            return xml;
        }
    }
}
