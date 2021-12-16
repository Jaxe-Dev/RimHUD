using UnityEngine;

namespace RimHUD.Interface.HUD
{
  internal class HudBlank : HudWidgetBase
  {
    public const string Name = "Blank";
    public static HudBlank GetEmpty => new HudBlank(0f);

    public override float Height { get; }

    private HudBlank(float height) => Height = height;

    public static HudBlank Get(float height) => new HudBlank(height);

    public override bool Draw(HudComponent component, Rect rect) => true;
  }
}
