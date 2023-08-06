using RimHUD.Configuration;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public class SeparatorWidget : IWidget
  {
    public const string Id = "Separator";

    public float Height { get; } = 4f;

    public static SeparatorWidget Get() => new SeparatorWidget();

    public bool Draw(Rect rect)
    {
      GUIPlus.SetColor(Theme.LineColor.Value);
      Verse.Widgets.DrawLineHorizontal(rect.x, rect.y + (rect.height / 2f), rect.width);
      GUIPlus.ResetColor();
      return true;
    }
  }
}
