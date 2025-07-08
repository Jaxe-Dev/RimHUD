using System;
using System.Linq;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Screen;

public static class InspectPaneTabs
{
  public static void Draw(IInspectPane pane)
  {
    const float fixedRecentHeight = 700f;

    try
    {
      var tabsTopY = pane.PaneTopY - WidgetsPlus.TabButtonHeight;
      var curTabX = InspectPaneUtility.PaneWidthFor(pane) - Theme.InspectPaneTabWidth.Value;
      var leftEdge = 0f;
      var drewOpen = false;

      if (pane.CurTabs is null) { return; }

      foreach (var tab in pane.CurTabs)
      {
        if (!tab.IsVisible) { continue; }

        var open = tab.GetType() == pane.OpenTabType;

        if (!tab.Hidden)
        {
          var rect = new Rect(curTabX, tabsTopY, Theme.InspectPaneTabWidth.Value, WidgetsPlus.TabButtonHeight);
          leftEdge = curTabX;

          Text.Font = GameFont.Small;

          if (Widgets.ButtonText(rect, tab.labelKey.TranslateSimple())) { Reflection.RimWorld_InspectPaneUtility_InterfaceToggleTab.InvokeStatic(tab, pane); }
          if (!open && !tab.TutorHighlightTagClosed.NullOrEmpty()) { UIHighlighter.HighlightOpportunity(rect, tab.TutorHighlightTagClosed); }

          curTabX -= Theme.InspectPaneTabWidth.Value;
        }
        if (!open) { continue; }

        tab.DoTabGUI();
        pane.RecentHeight = fixedRecentHeight;
        drewOpen = true;
      }

      if (drewOpen) { GUI.DrawTexture(new Rect(0f, tabsTopY, leftEdge, WidgetsPlus.TabButtonHeight), Reflection.RimWorld_InspectPaneUtility_InspectTabButtonFillTex.GetValueStatic<Texture2D>()); }
    }
    catch (Exception exception) { Report.HandleError(exception); }
  }

  public static void ToggleBio() => Toggle(typeof(ITab_Pawn_Character));
  public static void ToggleGear() => Toggle(typeof(ITab_Pawn_Gear));
  public static void ToggleHealth() => Toggle(typeof(ITab_Pawn_Health));
  public static void ToggleNeeds() => Toggle(typeof(ITab_Pawn_Needs));
  public static void ToggleSocial() => Toggle(typeof(ITab_Pawn_Social));
  public static void ToggleTraining() => Toggle(typeof(ITab_Pawn_Training));
  public static void TogglePrisoner() => Toggle(typeof(ITab_Pawn_Prisoner));
  public static void ToggleSlave() => Toggle(typeof(ITab_Pawn_Slave));

  private static void Toggle(Type tabType)
  {
    var pane = (MainTabWindow_Inspect)MainButtonDefOf.Inspect!.TabWindow;
    var tab = (from t in pane.CurTabs where tabType.IsInstanceOfType(t) select t).FirstOrDefault();
    if (tab is null) { return; }

    if (Find.MainTabsRoot!.OpenTab != MainButtonDefOf.Inspect) { Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Inspect); }

    Reflection.RimWorld_InspectPaneUtility_ToggleTab.InvokeStatic(tab, pane);
  }
}
