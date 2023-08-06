using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  public class Dialog_Config : WindowPlus
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
      Persistent.HasCredits ? new Tab_ConfigCredits() : null
    };

    private readonly TabManager _tabs;

    private Dialog_Config() : base(Lang.Get("Interface.Dialog_Config.Title").Bold(), new Vector2(800f, 700f))
    {
      onlyOneOfTypeAllowed = true;
      absorbInputAroundWindow = false;
      preventCameraMotion = false;
      doCloseButton = false;

      if (Lang.HasKey("Language.TranslatedBy")) { Subtitle = Lang.Get("Language.TranslatedBy").Italic(); }

      _tabs = new TabManager(TabWidth, TabHeight, TabOrder);
      if (!Persistent.TutorialComplete) { Tutorial.DoDialogConfigTab(_tabs); }
    }

    public static void Toggle()
    {
      var current = Find.WindowStack.WindowOfType<Dialog_Config>();
      if (current == null) { Open(); }
      else { current.Close(); }
    }

    public static void Open() => Find.WindowStack.Add(new Dialog_Config());

    public override void PostClose()
    {
      Tutorial.CheckComplete(true);

      if (!State.Activated) { return; }
      Persistent.Save();
    }

    protected override void DrawContent(Rect rect)
    {
      try
      {
        var grid = rect.GetVGrid(WidgetsPlus.MediumPadding, -1f, ButtonHeight);
        var hGrid = grid[2].GetHGrid(WidgetsPlus.MediumPadding, ButtonWidth, ButtonWidth, ButtonWidth);

        if (!Persistent.TutorialComplete)
        {
          Tutorial.DoDialogConfigTab(_tabs);

          Tutorial.SetDialogConfigCloseButton(hGrid[3]);
          Tutorial.DoDialogConfigEarly();
        }

        _tabs.Draw(grid[1]);

        if (WidgetsPlus.DrawButton(hGrid[1], Lang.Get("Interface.Dialog_Config.SetToDefault"))) { ConfirmSetToDefault(); }
        else if (WidgetsPlus.DrawButton(hGrid[2], Lang.Get("Interface.Dialog_Config.OpenFolder"))) { Persistent.OpenConfigFolder(); }
        else if (WidgetsPlus.DrawButton(hGrid[3], Lang.Get("Interface.Button.Close"))) { Close(); }

        WidgetsPlus.DrawText(grid[2], "Version " + Mod.Version + (Prefs.DevMode && Mod.DevMode ? "[DEV MODE - Click to disable]".Colorize(Color.yellow) : null), Theme.SmallTextStyle, alignment: TextAnchor.LowerRight);

        if (!Event.current.shift || !Widgets.ButtonInvisible(grid[2]) || !Prefs.DevMode) { return; }

        Mod.DevMode = !Mod.DevMode;
        HudTimings.Restart();
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleError(exception);
        Close();
      }
    }

    private void ConfirmSetToDefault() => Dialog_Alert.Open(Lang.Get("Interface.Alert.SetToDefault"), Dialog_Alert.Buttons.YesNo, SetToDefault);

    private void SetToDefault()
    {
      Persistent.AllToDefault();
      _tabs.Reset();
    }

    protected override void LateWindowOnGUI(Rect inRect)
    {
      if (!Persistent.TutorialComplete) { Tutorial.DoDialogConfigLate(windowRect.AtZero()); }
    }
  }
}
