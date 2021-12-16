using RimHUD.Data;
using RimHUD.Data.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal class Dialog_Error : Window
  {
    private const float ButtonWidth = 120f;
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _scrollView;

    private readonly Mod.ExceptionInfo _info;

    public override Vector2 InitialSize { get; } = new Vector2(800f, 360f);

    private Dialog_Error(Mod.ExceptionInfo info)
    {
      doCloseButton = false;
      closeOnAccept = true;
      closeOnClickedOutside = false;
      absorbInputAroundWindow = true;
      draggable = true;

      _info = info;

      Mod.Warning("RimHUD Auto-deactivation reason:\n" + _info.Text);
    }

    public static void Open(Mod.ExceptionInfo info) => Find.WindowStack.Add(new Dialog_Error(info));

    public override void DoWindowContents(Rect inRect)
    {
      var listing = new ListingPlus();
      listing.Begin(inRect);
      listing.Label("RimHUD has automatically deactivated due to the following error(s):".Bold());
      listing.Label(_info.Message);
      if (_info.IsExternalError) { listing.Label(_info.PossibleMod == null ? "The error appears to be from outside RimHUD." : $"The error appears to be have been caused by the mod '{_info.PossibleMod.Bold()}'.", color: Color.yellow); }
      listing.Gap();
      listing.Label("Stacktrace:".Bold(), font: GameFont.Tiny);
      listing.End();

      var grid = inRect.GetVGrid(0f, listing.CurHeight, -1f, GUIPlus.SmallButtonHeight + GUIPlus.MediumPadding);

      Widgets.DrawMenuSection(grid[2]);

      var stacktraceRect = grid[2].ContractedBy(GUIPlus.SmallPadding);
      var stacktraceList = new ListingPlus();

      stacktraceList.BeginScrollView(stacktraceRect, ref _scrollPosition, ref _scrollView);
      stacktraceList.Label(_info.StackTrace, font: GameFont.Tiny);
      stacktraceList.EndScrollView(ref _scrollView);

      grid[3].yMin += GUIPlus.MediumPadding;
      var buttonGrid = grid[3].GetHGrid(GUIPlus.MediumPadding, ButtonWidth, -1f, ButtonWidth, ButtonWidth);

      if (GUIPlus.DrawButton(buttonGrid[1], "Copy to clipboard", font: GameFont.Tiny))
      {
        GUIUtility.systemCopyBuffer = $"[[RimHUD Auto-deactivation report]]\n{_info.Text}";
        Mod.Message("RimHUD Auto-deactivation details copied to clipboard");
      }
      if (GUIPlus.DrawButton(buttonGrid[3], "Reactivate", font: GameFont.Tiny))
      {
        Close();
        State.Activated = true;
      }
      if (GUIPlus.DrawButton(buttonGrid[4], "Close", font: GameFont.Tiny)) { Close(); }
    }
  }
}
