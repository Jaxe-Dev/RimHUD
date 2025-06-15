using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud;

public static class HudLayout
{
  private static readonly float ConfigButtonSize = Mathf.Max(TexturesPlus.ConfigIcon.width, TexturesPlus.ConfigIcon.height);

  public static bool IsMouseOverConfigButton = true;

  public static Rect GetConfigButtonRect(Rect rect, bool floating) => new(rect.xMax - ConfigButtonSize, floating ? rect.y : rect.yMax - ConfigButtonSize, ConfigButtonSize, ConfigButtonSize);

  public static void DrawConfigButton(Rect rect, bool tutorial = false)
  {
    if ((tutorial || Tutorial.IsComplete) && Verse.Widgets.ButtonImage(rect, tutorial ? TexturesPlus.TutorialConfigIcon : TexturesPlus.ConfigIcon)) { Dialog_Config.Toggle(); }
  }

  public static void DrawDocked(Rect rect) => Draw(rect, false);

  public static void DrawFloating()
  {
    var rect = Theme.GetHudBounds();
    Verse.Widgets.DrawWindowBackground(rect);

    Draw(rect, true);
  }

  private static void Draw(Rect rect, bool floating)
  {
    var layoutRect = floating ? rect.ContractedBy(GUIPlus.MediumPadding) : rect;
    var configRect = GetConfigButtonRect(layoutRect, floating);

    IsMouseOverConfigButton = Mouse.IsOver(configRect);

    (floating ? LayoutLayer.Floating : LayoutLayer.Docked).Draw(layoutRect);

    if (Mouse.IsOver(rect)) { DrawConfigButton(configRect); }
  }
}
