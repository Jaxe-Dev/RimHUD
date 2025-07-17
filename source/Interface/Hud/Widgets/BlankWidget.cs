using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class BlankWidget : IWidget
{
  public const string TypeName = "Blank";

  public static BlankWidget Collapsed => new(0f);

  public float GetMaxHeight { get; }

  public BlankWidget(float height) => GetMaxHeight = height;

  public bool Draw(Rect rect) => true;
}
