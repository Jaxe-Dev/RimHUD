using RimHUD.Configuration;
using RimWorld;
using Verse;

namespace RimHUD.Engine
{
  public static class State
  {
    public static bool Activated { get; set; } = true;
    public static bool Active => Activated && Current.ProgramState == ProgramState.Playing;

    public static bool ModifyPane => ResizePane || (ShowPane && Theme.InspectPaneTabModify.Value);
    public static bool CompressLetters => Active && Theme.LetterCompress.Value;
    public static bool HudFloatingVisible => !Theme.HudDocked.Value && ShowPane;

    public static Pawn SelectedPawn => GetSelectedPawn();

    private static bool ShowPane => Active && MainButtonDefOf.Inspect.TabWindow.IsOpen && SelectedPawn != null;

    public static bool ResizePane { get; set; }

    private static Pawn GetSelectedPawn()
    {
      var thing = Find.Selector.SingleSelectedThing;
      return thing is Pawn pawn ? pawn : null;
    }
  }
}
