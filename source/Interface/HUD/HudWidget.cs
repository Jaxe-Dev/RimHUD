using UnityEngine;

namespace RimHUD.Interface.HUD
{
  internal abstract class HudWidget : HudWidgetBase
  {
    protected abstract bool DoDraw(Rect rect);

    public override bool Draw(HudComponent component, Rect rect)
    {
      HudTimings.Update(component)?.Start();

      var result = DoDraw(rect);

      HudTimings.Update(component)?.Finish(rect);

      return result;
    }
  }
}
