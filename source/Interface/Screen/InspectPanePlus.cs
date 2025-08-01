using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Tooltips;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Screen;

public static class InspectPanePlus
{
  public static Rect GetBounds(Rect bounds) => bounds.ContractedBy(GUIPlus.MediumPadding);

  public static void DrawPane(Rect rect, IInspectPane pane)
  {
    Theme.CheckFontChange();

    pane.RecentHeight = Theme.InspectPaneHeight.Value;

    var bounds = GetBounds(rect);
    bounds.height += 1f;

    var offset = 0f;
    var headerHeight = Math.Max(Theme.LargeTextStyle.LineHeight, GenUI.SmallIconSize);

    try
    {
      InspectPaneButtons.Draw(bounds.TopPartPixels(headerHeight), pane, ref offset);

      if (State.SelectedPawn is null || !pane.AnythingSelected) { return; }

      var labelRect = new Rect(bounds.x, bounds.y, bounds.width - offset, headerHeight);
      WidgetsPlus.DrawText(labelRect, Active.Name, Theme.LargeTextStyle, Active.FactionRelationColor);
      TooltipsPlus.DrawCompact(labelRect, BioTooltip.Get);
      if (Widgets.ButtonInvisible(labelRect)) { InspectPaneTabs.ToggleSocial(); }

      if (!pane.ShouldShowPaneContents) { return; }
    }
    catch (Exception exception) { Report.HandleError(exception); }

    var contentRect = bounds.BottomPartPixels(bounds.height - headerHeight - GUIPlus.TinyPadding);
    if (Theme.DockedMode.Value) { HudLayout.DrawDocked(contentRect); }
    else if (Theme.InspectPaneTabAddLog.Value) { InspectPaneLog.Draw(Active.Pawn, contentRect); }

    if (!Tutorial.IsComplete) { Tutorial.Presentation.Stages.DoInspectPane(rect); }
  }
}
