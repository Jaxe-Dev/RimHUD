using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Tooltips;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Screen
{
  public static class InspectPanePlus
  {
    public static Rect GetBounds(Rect bounds) => bounds.ContractedBy(GUIPlus.MediumPadding);

    public static void DrawPane(Rect rect, IInspectPane pane)
    {
      Theme.CheckFontChange();

      pane.RecentHeight = Theme.InspectPaneHeight.Value - WidgetsPlus.MainButtonHeight;

      if (State.SelectedPawn is null || !pane.AnythingSelected) { return; }

      try
      {
        var bounds = GetBounds(rect);

        var offset = 0f;

        var headerHeight = Math.Max(Theme.LargeTextStyle.LineHeight, GenUI.SmallIconSize);
        InspectPaneButtons.Draw(bounds.TopPartPixels(headerHeight), pane, ref offset);

        var labelRect = new Rect(bounds.x, bounds.y, bounds.width - offset, headerHeight);
        WidgetsPlus.DrawText(labelRect, Active.Name, Theme.LargeTextStyle, Active.FactionRelationColor);
        TooltipsPlus.DrawCompact(labelRect, BioTooltip.Get);
        if (Widgets.ButtonInvisible(labelRect)) { InspectPaneTabs.ToggleSocial(); }

        if (!pane.ShouldShowPaneContents) { return; }

        var contentRect = bounds.BottomPartPixels(bounds.height - headerHeight - GUIPlus.TinyPadding);
        DrawContent(contentRect);
      }
      catch (Exception exception) { Report.HandleError(exception); }

      if (!Tutorial.IsComplete) { Tutorial.Presentation.Stages.DoInspectPane(rect); }
    }

    public static void DrawContent(Rect rect)
    {
      if (Theme.DockedMode.Value) { HudLayout.DrawDocked(rect); }
      else if (Theme.InspectPaneTabAddLog.Value) { InspectPaneLog.Draw(Active.Pawn, rect); }
    }
  }
}
