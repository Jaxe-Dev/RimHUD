using RimHUD.Data.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudSeparator : HudWidget
    {
        public const string Name = "Separator";

        public override float Height { get; } = 4f;

        public static HudSeparator Get() => new HudSeparator();

        public override bool Draw(Rect rect)
        {
            GUIPlus.SetColor(Theme.LineColor.Value);
            Widgets.DrawLineHorizontal(rect.x, rect.y + (rect.height / 2f), rect.width);
            GUIPlus.ResetColor();
            return true;
        }
    }
}
