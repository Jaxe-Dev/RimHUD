using System.Xml.Linq;
using RimHUD.Data.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudElement : HudComponent
    {
        private const string DefNameAttribute = "DefName";

        private readonly string _type;
        private readonly string _defName;
        protected override HudTarget Targets { get; }

        public HudWidget Widget;

        private HudElement(string type, string defName, HudTarget target)
        {
            _type = type;
            _defName = defName;
            Targets = target;
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
            var widget = HudModel.GetWidget(model, _type, _defName);
            Widget = IsTargetted(model) ? widget : HudBlank.Get(widget.Height);
        }

        public override bool Draw(Rect rect)
        {
            var result = Widget?.Draw(rect) ?? false;
            Widget = null;
            return result;
        }

        public override XElement ToXml()
        {
            var xml = new XElement(_type);
            if (!_defName.NullOrEmpty()) { xml.Add(new XAttribute(DefNameAttribute, _defName)); }

            var targets = Targets.ToId();
            if (!targets.NullOrEmpty()) { xml.Add(new XAttribute(TargetAttribute, targets)); }
            return xml;
        }
    }
}
