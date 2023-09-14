using RimHUD.Configuration;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public sealed class SeparatorWidget : IWidget
  {
    public const string TypeName = "Separator";

    public float GetMaxHeight => GUIPlus.SmallPadding;

    public bool Draw(Rect rect)
    {
      GUIPlus.SetColor(Theme.LineColor.Value);
      Verse.Widgets.DrawLineHorizontal(rect.x, rect.y + (rect.height / 2f), rect.width);
      GUIPlus.ResetColor();
      return true;
    }
  }
}
