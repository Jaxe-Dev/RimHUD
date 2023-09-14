using RimHUD.Configuration;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Engine
{
  public static class State
  {
    public static bool Activated { get; set; } = true;
    private static bool Active => Activated && Current.ProgramState is ProgramState.Playing;

    public static Pawn? SelectedPawn => Find.Selector?.SingleSelectedThing as Pawn;

    public static bool ForceModifyPane { get; set; }

    public static bool ModifyPane => ForceModifyPane || (ShowPane && Theme.InspectPaneTabModify.Value);
    public static bool CompressLetters => Active && !Theme.DockedMode.Value && Theme.LetterCompress.Value;
    public static bool HudFloatingVisible => !Theme.DockedMode.Value && ShowPane;

    private static bool ShowPane => Active && MainButtonDefOf.Inspect!.TabWindow!.IsOpen && SelectedPawn is not null;

    public static LayoutLayer CurrentLayout => Theme.DockedMode.Value ? LayoutLayer.Docked : LayoutLayer.Floating;

    public static void ClearCache()
    {
      InspectPaneLog.ClearCache();
      LayoutLayer.Docked.Flush();
      LayoutLayer.Floating.Flush();
    }
  }
}
