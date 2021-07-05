using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal abstract class HudWidget
    {
        public abstract float Height { get; }
        public abstract bool Draw(Rect rect);
    }
}
