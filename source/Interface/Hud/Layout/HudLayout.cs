using RimHUD.Configuration;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public static class HudLayout
  {
    private static readonly float ConfigButtonSize = Mathf.Max(Textures.ConfigIcon.width, Textures.ConfigIcon.height);

    public static bool IsMouseOverConfigButton = true;

    public static void DrawDocked(Rect bounds, PawnModel owner)
    {
      var configRect = GetConfigButtonRect(bounds, false);
      IsMouseOverConfigButton = Mouse.IsOver(configRect);
      LayoutLayer.Docked.Draw(bounds, owner ?? PawnModel.Selected);
      if (Mouse.IsOver(bounds)) { DrawConfigButton(configRect); }
    }

    public static void DrawFloating()
    {
      var bounds = Theme.GetHudBounds();

      Verse.Widgets.DrawWindowBackground(bounds);
      var inner = bounds.ContractedBy(WidgetsPlus.MediumPadding);
      var configRect = GetConfigButtonRect(inner, true);
      IsMouseOverConfigButton = Mouse.IsOver(configRect);
      LayoutLayer.Floating.Draw(inner, PawnModel.Selected);
      if (Mouse.IsOver(bounds)) { DrawConfigButton(configRect); }
    }

    public static Rect GetConfigButtonRect(Rect bounds, bool top)
    {
      var y = top ? bounds.y : bounds.yMax - ConfigButtonSize;
      return new Rect(bounds.xMax - ConfigButtonSize, y, ConfigButtonSize, ConfigButtonSize);
    }

    public static void DrawConfigButton(Rect rect, bool tutorial = false)
    {
      if ((tutorial || Persistent.TutorialComplete) && Verse.Widgets.ButtonImage(rect, tutorial ? Textures.TutorialConfigIcon : Textures.ConfigIcon)) { Dialog_Config.Toggle(); }
    }
  }
}
