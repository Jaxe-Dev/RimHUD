using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal class HudBlank : HudWidget
    {
        public const string Name = "Blank";

        public override float Height { get; }

        public HudBlank(float height) => Height = height;

        public static HudBlank Get(float height) => new HudBlank(height);

        public override bool Draw(Rect rect) => true;
    }
}
