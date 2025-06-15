using RimHUD.Configuration;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Screen;
using RimWorld.Planet;
using Verse;

namespace RimHUD.Engine;

public static class State
{
  public static bool Activated { get; set; } = true;
  private static bool Active => Activated && Current.ProgramState is ProgramState.Playing;

  public static Pawn? SelectedPawn => Find.Selector?.SingleSelectedThing as Pawn;

  private static bool ShowPane => Active && WorldRendererUtility.CurrentWorldRenderMode is WorldRenderMode.None && SelectedPawn is not null;

  public static bool ModifyPane => ShowPane && Theme.InspectPaneTabModify.Value;
  public static bool CompressLetters => Active && !Theme.DockedMode.Value && Theme.LetterCompress.Value;
  public static bool HudFloatingVisible => !Theme.DockedMode.Value && ShowPane;

  public static LayoutLayer CurrentLayout => Theme.DockedMode.Value ? LayoutLayer.Docked : LayoutLayer.Floating;

  public static void ClearCache()
  {
    InspectPaneLog.ClearCache();
    LayoutLayer.Docked.Flush();
    LayoutLayer.Floating.Flush();
  }
}
