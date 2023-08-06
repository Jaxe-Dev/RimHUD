using System;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;
using Verse;

namespace RimHUD.Engine
{
  public static class Tutorial
  {
    private const int PulseInterval = 1000;
    private const float PulseStart = 0.55f;
    private const float PulseAmount = 0.2f;

    private const float ModSettingsMarginX = 180f;
    private const float ModSettingsMarginY = 200f;
    private const float InspectPaneNoticeWidth = 500f;
    private const float InspectPaneNoticeHeight = 30f;
    private const float DialogConfigNoticeWidth = 320f;
    private const float DialogConfigNoticeHeight = 120f;

    private const float LineThickness = 3f;

    private static int _stage;
    private static Vector2 _lineFocus;

    private static DateTime _lastPulse;
    private static bool _lastPulseDescending;

    private static Rect _dialogConfigNoticeRect;

    private static Rect _dialogConfigCloseButton;
    private static Rect _dialogConfigPresetButton;

    private static readonly Color OutlineColor = new Color(1f, 0.9f, 0f);
    private static readonly Color BackgroundColor = new Color(0.1f, 0.1f, 0.1f);

    public static void SetDialogConfigCloseButton(Rect rect) => _dialogConfigCloseButton = rect.ToScreenRect();
    public static void SetDialogConfigPresetButton(Rect rect) => _dialogConfigPresetButton = rect.ToScreenRect();

    private static void SetFocus(Rect focus, Vector2 anchor)
    {
      var xOffset = (focus.width.Half() + WidgetsPlus.MediumPadding) * anchor.x;
      var yOffset = (focus.height.Half() + WidgetsPlus.MediumPadding) * anchor.y;
      var center = focus.center;

      _lineFocus = new Vector2(center.x + xOffset, center.y - yOffset);
    }

    private static void DrawPulse(Rect rect)
    {
      if (_lastPulse == default) { _lastPulse = DateTime.Now; }

      var timeSpan = DateTime.Now - _lastPulse;
      if (timeSpan.TotalMilliseconds >= PulseInterval)
      {
        _lastPulse = DateTime.Now;
        _lastPulseDescending = !_lastPulseDescending;
        timeSpan = TimeSpan.Zero;
      }

      var alphaModifier = PulseAmount * (float)(timeSpan.TotalMilliseconds / PulseInterval);

      Widgets.DrawBoxSolid(rect.ContractedBy(1f), new Color(0f, 0f, 0f, PulseStart + (_lastPulseDescending ? PulseAmount - alphaModifier : alphaModifier)));
    }

    private static void DrawBox(Rect rect)
    {
      Widgets.DrawShadowAround(rect);
      Widgets.DrawBoxSolidWithOutline(rect, BackgroundColor, OutlineColor, (int)LineThickness);
    }

    private static void DrawNotice(Rect rect, string text)
    {
      DrawLine(rect);
      DrawBox(rect);

      WidgetsPlus.DrawMultilineText(rect.ContractedBy(WidgetsPlus.LargePadding, 0f), text, alignment: TextAnchor.MiddleLeft);
    }

    private static void DrawLine(Rect notice)
    {
      if (_lineFocus == default) { return; }

      Widgets.DrawLine(_lineFocus, notice.center, OutlineColor, LineThickness);
      _lineFocus = default;
    }

    public static void CheckComplete(bool fromClose = false)
    {
      if (_stage < (fromClose ? 2 : 3)) { return; }

      _stage = 0;
      Persistent.CompleteTutorial();
    }

    public static void OverlayModSettings(Rect rect)
    {
      var boxRect = rect.ContractedBy(ModSettingsMarginX, ModSettingsMarginY);
      var vGrid = boxRect.ContractedBy(WidgetsPlus.XLPadding).GetVGrid(WidgetsPlus.SmallPadding, -1f, WidgetsPlus.ButtonHeight);
      DrawBox(boxRect);

      if (!Persistent.TutorialComplete) { WidgetsPlus.DrawMultilineText(vGrid[0], Lang.Get("Interface.Settings.Tutorial").Bold() + "\n\n" + Lang.Get("Interface.Settings.TutorialStart").Italic(), alignment: TextAnchor.MiddleCenter); }
      else
      {
        WidgetsPlus.DrawMultilineText(vGrid[1], Lang.Get("Interface.Settings.Tutorial").Bold() + "\n\n" + Lang.Get("Interface.Settings.TutorialComplete").Italic(), alignment: TextAnchor.MiddleCenter);
        if (WidgetsPlus.DrawButton(vGrid[2], Lang.Get("Interface.Settings.ResetTutorial"))) { Persistent.ResetTutorial(); }
      }
    }

    public static void DoInspectPane(Rect rect)
    {
      if (Find.WindowStack.IsOpen(typeof(Dialog_Config))) { return; }
      if (!Theme.HudDocked.Value) { Theme.HudDocked.Value = true; }

      DrawPulse(rect);

      var contentRect = InspectPanePlus.GetContentRect(rect);

      var isMouseOver = Mouse.IsOver(contentRect);
      var text = Mod.Name + " - " + (isMouseOver ? Lang.Get("Interface.Tutorial.InspectPane") : Lang.Get("Interface.Tutorial.InspectPaneHover")).Bold();

      var noticeRect = contentRect.center.GetCenteredRect(InspectPaneNoticeWidth, InspectPaneNoticeHeight);

      if (!isMouseOver)
      {
        DrawNotice(noticeRect, text);
        return;
      }

      var configButtonRect = HudLayout.GetConfigButtonRect(contentRect, false);
      SetFocus(configButtonRect, Vector2.left);
      DrawNotice(noticeRect, text);

      HudLayout.DrawConfigButton(configButtonRect, true);
    }

    public static void DoDialogConfigTab(TabManager tabs)
    {
      if (_stage == 1) { tabs.SelectType<Tab_ConfigContent>(); }
      else { tabs.SelectType<Tab_ConfigDesign>(); }
    }

    public static void DoDialogConfigEarly()
    {
      var closeButtonRect = _dialogConfigCloseButton.ToGUIRect();

      _dialogConfigNoticeRect = new Rect(closeButtonRect.xMax + WidgetsPlus.XLPadding, closeButtonRect.yMin - (DialogConfigNoticeHeight + WidgetsPlus.MediumPadding), DialogConfigNoticeWidth, DialogConfigNoticeHeight);

      if (!Widgets.ButtonInvisible(_dialogConfigNoticeRect)) { return; }

      _stage++;

      CheckComplete();
    }

    public static void DoDialogConfigLate(Rect rect)
    {
      DrawPulse(rect);

      string text;
      if (_stage == 0) { text = Lang.Get("Interface.Tutorial.DialogConfig1").Bold() + "\n\n" + Lang.Get("Interface.Tutorial.DialogConfigContinue").Italic(); }
      else if (_stage == 1)
      {
        text = Lang.Get("Interface.Tutorial.DialogConfig2").Bold() + "\n\n" + Lang.Get("Interface.Tutorial.DialogConfigContinue").Italic();

        var presetButton = _dialogConfigPresetButton.ToGUIRect();
        SetFocus(presetButton, Vector2.down);
        Tab_ConfigContent.DrawPresetSelector(presetButton);
      }
      else if (_stage == 2)
      {
        text = Lang.Get("Interface.Tutorial.DialogConfig3").Bold() + "\n\n" + Lang.Get("Interface.Tutorial.DialogConfigEnd").Italic();

        var closeButtonRect = _dialogConfigCloseButton.ToGUIRect();
        SetFocus(closeButtonRect, Vector2.right);
        WidgetsPlus.DrawButton(closeButtonRect, Lang.Get("Interface.Button.Close"));
      }
      else { throw new Mod.Exception("Invalid Tutorial Stage"); }

      DrawNotice(_dialogConfigNoticeRect, text);
    }
  }
}
