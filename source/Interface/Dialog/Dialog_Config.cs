using System;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Storage;
using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal class Dialog_Config : WindowPlus
  {
    private const float Padding = 8f;
    private const float TabWidth = 160f;
    private const float TabHeight = 30f;
    private const float ButtonWidth = 120f;
    private const float ButtonHeight = 40f;

    private readonly TabManager _tabs;

    private Dialog_Config() : base(Lang.Get("Dialog_Config.Title").Bold(), new Vector2(800f, 650f))
    {
      onlyOneOfTypeAllowed = true;
      absorbInputAroundWindow = false;
      preventCameraMotion = false;
      doCloseButton = false;

      if (Lang.HasKey("Language.TranslatedBy")) { Subtitle = Lang.Get("Language.TranslatedBy").Italic(); }

      _tabs = Persistent.GetCredits() == null ? new TabManager(TabWidth, TabHeight, new Tab_ConfigDesign(), new Tab_ConfigColors(), new Tab_ConfigContent()) : new TabManager(TabWidth, TabHeight, new Tab_ConfigDesign(), new Tab_ConfigColors(), new Tab_ConfigContent(), new Tab_ConfigCredits());
    }

    public static void Open() => Find.WindowStack.Add(new Dialog_Config());

    public override void PostClose()
    {
      if (!State.Activated) { return; }
      Persistent.Save();
    }

    protected override void DrawContent(Rect rect)
    {
      try
      {
        var grid = rect.GetVGrid(Padding, -1f, ButtonHeight);

        _tabs.Draw(grid[1]);

        var button = GUIPlus.DrawButtonRow(grid[2], ButtonWidth, Padding, Lang.Get("Dialog_Config.SetToDefault"), Lang.Get("Dialog_Config.OpenFolder"), Lang.Get("Button.Close"));

        GUIPlus.DrawText(grid[2], "Version " + Mod.Version + (Prefs.DevMode && Mod.DevMode ? "[DEV MODE - Click to disable]" : null), style: Theme.SmallTextStyle, alignment: TextAnchor.LowerRight);
        if (Widgets.ButtonInvisible(grid[2]) && Prefs.DevMode)
        {
          Mod.DevMode = !Mod.DevMode;
          HudTimings.Restart();
        }

        if (button == 1) { ConfirmSetToDefault(); }
        else if (button == 2) { Persistent.OpenConfigFolder(); }
        else if (button == 3) { Close(); }
      }
      catch (Exception exception)
      {
        Mod.HandleError(exception);
        Close();
      }
    }

    private void ConfirmSetToDefault() => Dialog_Alert.Open(Lang.Get("Alert.SetToDefault"), Dialog_Alert.Buttons.YesNo, SetToDefault);

    private void SetToDefault()
    {
      Persistent.AllToDefault();
      _tabs.Reset();
    }
  }
}
