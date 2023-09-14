using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog.Tabs;
using RimHUD.Interface.Hud;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  public sealed class Dialog_Config : WindowPlus
  {
    private const float TabWidth = 160f;
    private const float TabHeight = 30f;
    private const float ButtonWidth = 120f;
    private const float ButtonHeight = 40f;

    private static readonly Tab[] TabOrder =
    {
      new Tab_ConfigDesign(),
      new Tab_ConfigContent(),
      new Tab_ConfigColors(),
      new Tab_ConfigCredits()
    };

    private readonly TabManager _tabs;

    private Dialog_Config() : base(new Vector2(800f, 700f), Lang.Get("Interface.Dialog_Config.Title"), Lang.HasKey("Language.TranslatedBy") ? Lang.Get("Language.TranslatedBy") : null)
    {
      onlyOneOfTypeAllowed = true;
      absorbInputAroundWindow = false;
      preventCameraMotion = false;
      doCloseButton = false;

      _tabs = new TabManager(TabWidth, TabHeight, TabOrder);
      if (!Tutorial.IsComplete) { Tutorial.Presentation.Stages.DoDialogConfigTab(_tabs); }
    }

    public static void Toggle()
    {
      var current = Find.WindowStack!.WindowOfType<Dialog_Config>();
      if (current is null) { Open(); }
      else { current.Close(); }
    }

    public static void Open() => Find.WindowStack!.Add(new Dialog_Config());

    public override void PostClose()
    {
      Tutorial.Presentation.Stages.CheckComplete(true);

      if (!State.Activated) { return; }
      Persistent.Save();
    }

    protected override void DrawContent(Rect rect)
    {
      try
      {
        var grid = rect.GetVGrid(GUIPlus.MediumPadding, -1f, ButtonHeight);
        var hGrid = grid[2].GetHGrid(GUIPlus.MediumPadding, ButtonWidth, ButtonWidth, ButtonWidth);

        if (!Tutorial.IsComplete)
        {
          Tutorial.Presentation.Stages.DoDialogConfigTab(_tabs);

          Tutorial.Presentation.Stages.SetDialogConfigCloseButton(hGrid[3]);
          Tutorial.Presentation.Stages.DoDialogConfigEarly();
        }

        _tabs.Draw(grid[1]);

        if (WidgetsPlus.DrawButton(hGrid[1], Lang.Get("Interface.Dialog_Config.SetToDefault"))) { ConfirmSetToDefault(); }
        else if (WidgetsPlus.DrawButton(hGrid[2], Lang.Get("Interface.Dialog_Config.OpenFolder"))) { Persistent.OpenConfigFolder(); }
        else if (WidgetsPlus.DrawButton(hGrid[3], Lang.Get("Interface.Button.Close"))) { Close(); }

        WidgetsPlus.DrawText(grid[2], $"Version {Mod.Version}{(Prefs.DevMode && Mod.DevMode ? "[DEV MODE - Click to disable]".Colorize(Color.yellow) : null)}", GameFont.Tiny, alignment: TextAnchor.LowerRight);

        if (!Event.current!.shift || !Widgets.ButtonInvisible(grid[2]) || !Prefs.DevMode) { return; }

        Mod.DevMode = !Mod.DevMode;
        HudTimings.Restart();
      }
      catch (Exception exception)
      {
        Report.HandleError(exception);
        Close();
      }
    }

    protected override void LateWindowOnGUI(Rect inRect)
    {
      if (!Tutorial.IsComplete) { Tutorial.Presentation.Stages.DoDialogConfigLate(windowRect.AtZero()); }
    }

    private void ConfirmSetToDefault() => Dialog_Alert.Open(Lang.Get("Interface.Alert.SetToDefault"), Dialog_Alert.Buttons.YesNo, SetToDefault);

    private void SetToDefault()
    {
      Persistent.SettingsToDefault();
      _tabs.Reset();
    }
  }
}
