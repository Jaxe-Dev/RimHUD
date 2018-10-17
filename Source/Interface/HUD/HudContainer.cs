using System.Xml.Linq;
using RimHUD.Data.Models;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudContainer
    {
        protected abstract string ElementName { get; }

        public abstract bool FillHeight { get; }

        public abstract float Prepare(PawnModel model);
        public abstract bool Draw(Rect rect);

        public abstract XElement ToXml();
    }
}
