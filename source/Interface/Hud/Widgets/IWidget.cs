using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public interface IWidget
  {
    float Height { get; }

    bool Draw(Rect rect);
  }
}
