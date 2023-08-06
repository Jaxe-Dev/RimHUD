using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public class BlankWidget : IWidget
  {
    public const string Id = "Blank";
    public static BlankWidget GetEmpty => new BlankWidget(0f);

    public float Height { get; }

    private BlankWidget(float height) => Height = height;

    public static BlankWidget Get(float height) => new BlankWidget(height);

    public bool Draw(Rect rect) => true;
  }
}
