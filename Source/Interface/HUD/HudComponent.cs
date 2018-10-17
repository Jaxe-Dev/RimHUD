using System.Xml.Linq;
using RimHUD.Data.Models;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudComponent
    {
        public const string TargetAttribute = "Targets";

        protected abstract HudTarget Targets { get; }

        public abstract bool Draw(Rect rect);
        public abstract float Prepare(PawnModel model);

        public abstract XElement ToXml();

        protected static HudTarget TargetsFromXml(XElement xe) => HudTargetUtility.FromId(xe.Attribute(TargetAttribute)?.Value);
        protected bool IsTargetted(PawnModel model) => Targets.HasTarget(model.Target);
    }
}
