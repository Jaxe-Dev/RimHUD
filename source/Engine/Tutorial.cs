using System;
using System.Xml.Linq;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.Dialog.Tabs;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Screen;
using UnityEngine;
using Verse;

namespace RimHUD.Engine
{
  public static class Tutorial
  {
    public static class Presentation
    {
      private const float ModSettingsMarginX = 180f;
      private const float ModSettingsMarginY = 200f;

      private const int PulseInterval = 1000;
      private const float PulseStart = 0.55f;
      private const float PulseAmount = 0.2f;
      private const float InspectPaneNoticeWidth = 500f;
      private const float InspectPaneNoticeHeight = 30f;
      private const float DialogConfigNoticeWidth = 320f;
      private const float DialogConfigNoticeHeight = 120f;
      private const float LineThickness = 3f;

      private static Vector2 _lineFocus;

      private static DateTime _lastPulse;
      private static bool _lastPulseDescending;

      private static readonly Color OutlineColor = new(1f, 0.9f, 0f);
      private static readonly Color BackgroundColor = new(0.1f, 0.1f, 0.1f);

      public static class Stages
      {
        private static int _stage;

        private static Rect _dialogConfigNoticeRect;
        private static Rect _dialogConfigCloseButton;
        private static Rect _dialogConfigLayoutButton;

        public static void CheckComplete(bool fromClose = false)
        {
          if (_stage < (fromClose ? 2 : 3)) { return; }

          _stage = 0;
          Complete();
        }

        public static void SetDialogConfigCloseButton(Rect rect) => _dialogConfigCloseButton = rect.ToScreenRect();
        public static void SetDialogConfigLayoutButton(Rect rect) => _dialogConfigLayoutButton = rect.ToScreenRect();

        public static void DoInspectPane(Rect rect)
        {
          if (Find.WindowStack!.IsOpen(typeof(Dialog_Config))) { return; }
          Theme.DockedMode.Value = true;

          DrawPulse(rect);

          var contentRect = InspectPanePlus.GetBounds(rect);

          var isMouseOver = Mouse.IsOver(contentRect);
          var text = $"{Mod.Name} - {(isMouseOver ? Lang.Get("Interface.Tutorial.InspectPane") : Lang.Get("Interface.Tutorial.InspectPaneHover")).Bold()}";

          var noticeRect = contentRect.center.GetCenteredRect(InspectPaneNoticeWidth, InspectPaneNoticeHeight);

          if (!isMouseOver)
          {
            DrawNotice(noticeRect, text);
            return;
          }

          var configButtonRect = HudLayout.GetConfigButtonRect(contentRect);
          SetFocus(configButtonRect, Vector2.left);
          DrawNotice(noticeRect, text);

          HudLayout.DrawConfigButton(configButtonRect, true);
        }

        public static void DoDialogConfigTab(TabManager tabs)
        {
          if (_stage is 1) { tabs.SelectType<Tab_ConfigContent>(); }
          else { tabs.SelectType<Tab_ConfigDesign>(); }
        }

        public static void DoDialogConfigEarly()
        {
          var closeButtonRect = _dialogConfigCloseButton.ToGUIRect();

          _dialogConfigNoticeRect = new Rect(closeButtonRect.xMax + GUIPlus.XLPadding, closeButtonRect.yMin - (DialogConfigNoticeHeight + GUIPlus.MediumPadding), DialogConfigNoticeWidth, DialogConfigNoticeHeight);

          if (!Widgets.ButtonInvisible(_dialogConfigNoticeRect)) { return; }

          _stage++;

          CheckComplete();
        }

        public static void DoDialogConfigLate(Rect rect)
        {
          DrawPulse(rect);

          string text;
          switch (_stage)
          {
            case 0:
              text = $"{Lang.Get("Interface.Tutorial.DialogConfig1").Bold()}\n\n{Lang.Get("Interface.Tutorial.DialogConfigContinue").Italic()}";
              break;
            case 1:
            {
              text = $"{Lang.Get("Interface.Tutorial.DialogConfig2").Bold()}\n\n{Lang.Get("Interface.Tutorial.DialogConfigContinue").Italic()}";

              var layoutButtonRect = _dialogConfigLayoutButton.ToGUIRect();
              SetFocus(layoutButtonRect, Vector2.down);
              Tab_ConfigContent.DrawLayoutSelector(layoutButtonRect);
              break;
            }
            case 2:
            {
              text = $"{Lang.Get("Interface.Tutorial.DialogConfig3").Bold()}\n\n{Lang.Get("Interface.Tutorial.DialogConfigEnd").Italic()}";

              var closeButtonRect = _dialogConfigCloseButton.ToGUIRect();
              SetFocus(closeButtonRect, Vector2.right);
              WidgetsPlus.DrawButton(closeButtonRect, Lang.Get("Interface.Button.Close"));
              break;
            }
            default:
              throw new Exception("Invalid tutorial stage.");
          }

          DrawNotice(_dialogConfigNoticeRect, text);
        }
      }

      public static void OverlayModSettings(Rect rect)
      {
        var boxRect = rect.ContractedBy(ModSettingsMarginX, ModSettingsMarginY);
        var vGrid = boxRect.ContractedBy(GUIPlus.XLPadding).GetVGrid(GUIPlus.SmallPadding, -1f, WidgetsPlus.ButtonHeight);
        DrawBox(boxRect);

        if (!IsComplete) { WidgetsPlus.DrawMultilineText(vGrid[0], $"{Lang.Get("Interface.Settings.Tutorial").Bold()}\n\n{Lang.Get("Interface.Settings.TutorialStart").Italic()}", alignment: TextAnchor.MiddleCenter); }
        else
        {
          WidgetsPlus.DrawMultilineText(vGrid[1], $"{Lang.Get("Interface.Settings.Tutorial").Bold()}\n\n{Lang.Get("Interface.Settings.TutorialComplete").Italic()}", alignment: TextAnchor.MiddleCenter);
          if (WidgetsPlus.DrawButton(vGrid[2], Lang.Get("Interface.Settings.ResetTutorial"))) { Reset(); }
        }
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

        WidgetsPlus.DrawMultilineText(rect.ContractedBy(GUIPlus.LargePadding, 0f), text, alignment: TextAnchor.MiddleLeft);
      }

      private static void DrawLine(Rect notice)
      {
        if (_lineFocus == default) { return; }

        Widgets.DrawLine(_lineFocus, notice.center, OutlineColor, LineThickness);
        _lineFocus = default;
      }

      private static void SetFocus(Rect focus, Vector2 anchor)
      {
        var xOffset = (focus.width.Half() + GUIPlus.MediumPadding) * anchor.x;
        var yOffset = (focus.height.Half() + GUIPlus.MediumPadding) * anchor.y;
        var center = focus.center;

        _lineFocus = new Vector2(center.x + xOffset, center.y - yOffset);
      }
    }

    public static bool IsComplete { get; private set; }

    public static void Initialize(XElement xml) => IsComplete = xml.Element("TutorialComplete") is not null;

    private static void Complete()
    {
      IsComplete = true;
      Persistent.Save();
    }

    private static void Reset()
    {
      Theme.DockedMode.Value = true;
      IsComplete = false;
      Persistent.Save();
    }
  }
}
