using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudWidgetBase
    {
        public abstract float Height { get; }

        public abstract bool Draw(HudComponent component, Rect rect);
    }
}
