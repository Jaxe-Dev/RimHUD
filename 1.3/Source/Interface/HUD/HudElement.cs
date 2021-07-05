using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Interface.Dialog;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudElement : HudComponent
    {
        public const string DefNameAttribute = "DefName";

        private readonly string _elementType;
        public override string ElementName => _elementType;
        public string DefName { get; }
        public override HudTarget Targets { get; }

        public HudWidget Widget;

        private HudElement(string elementType, string defName, HudTarget targets)
        {
            _elementType = elementType;
            DefName = defName;
            Targets = targets;
        }

        public override float Prepare(PawnModel model) => Widget?.Height ?? 0f;

        public static HudElement FromXml(XElement xe)
        {
            var type = xe.Name.ToString();
            if (!HudModel.IsValidType(type))
            {
                Mod.Error($"Invalid HUD widget type '{type}'");
                return null;
            }

            var defName = xe.Attribute(DefNameAttribute)?.Value;
            var targets = TargetsFromXml(xe);

            return new HudElement(type, defName, targets);
        }

        public void Build(PawnModel model)
        {
            var widget = HudModel.GetWidget(model, _elementType, DefName);
            Widget = IsTargetted(model) ? widget : HudBlank.Get(widget.Height);
        }

        public override bool Draw(Rect rect)
        {
            var result = Widget?.Draw(rect) ?? false;
            return result;
        }

        public void Flush() => Widget = null;

        public override XElement ToXml()
        {
            var xml = new XElement(_elementType);
            if (!DefName.NullOrEmpty()) { xml.Add(new XAttribute(DefNameAttribute, DefName)); }

            var targets = Targets.ToId();
            if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }
            return xml;
        }

        public override LayoutItem GetLayoutItem(LayoutEditor editor, LayoutItem parent) => new LayoutItem(editor, parent, this);
    }
}
