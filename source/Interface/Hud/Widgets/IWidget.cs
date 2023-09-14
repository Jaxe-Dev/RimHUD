using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public interface IWidget
  {
    float GetMaxHeight { get; }

    bool Draw(Rect rect);
  }
}
