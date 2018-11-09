using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Interface.Dialog;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudComponent
    {
        protected const string TargetAttribute = "Targets";

        public abstract string ElementName { get; }
        public abstract HudTarget Targets { get; }

        public abstract float Prepare(PawnModel model);
        public abstract bool Draw(Rect rect);

        public abstract XElement ToXml();

        protected static HudTarget TargetsFromXml(XElement xe) => HudTargetUtility.FromId(xe.Attribute(TargetAttribute)?.Value);
        protected bool IsTargetted(PawnModel model) => Targets.HasTarget(model.Target);

        public abstract LayoutItem GetWidget(LayoutDesign design, LayoutItem parent);
    }
}
