using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog;

public sealed class Dialog_Error : Window
{
  public override Vector2 InitialSize { get; } = new(800f, 360f);
  private Vector2 _scrollPosition = Vector2.zero;
  private Rect _scrollView;

  private readonly Report.ErrorInfo _info;

  private Dialog_Error(Report.ErrorInfo info)
  {
    doCloseButton = false;
    closeOnAccept = true;
    closeOnClickedOutside = false;
    absorbInputAroundWindow = true;
    draggable = true;

    _info = info;
    Report.Warning($"RimHUD Auto-deactivation reason:\n{_info.Message}");
  }

  public static void Open(Report.ErrorInfo info) => Find.WindowStack!.Add(new Dialog_Error(info));

  public override void DoWindowContents(Rect inRect)
  {
    const float buttonWidth = 120f;

    var listing = new ListingPlus();
    listing.Begin(inRect);
    listing.Label($"RimHUD v{Mod.Version} has automatically deactivated due to the following error(s):".Bold());
    listing.Label(_info.Message);

    listing.Gap();

    if (_info.Notice is not null)
    {
      listing.Label(_info.Notice);
      listing.Gap();
    }

    listing.Label($"{nameof(_info.Trace)}:".Bold(), font: GameFont.Tiny);
    listing.End();

    var grid = inRect.GetVGrid(0f, listing.CurHeight, -1f, WidgetsPlus.SmallButtonHeight + GUIPlus.MediumPadding);
    Widgets.DrawMenuSection(grid[2]);

    var traceRect = grid[2].ContractedBy(GUIPlus.SmallPadding);
    WidgetsPlus.DrawScrollableText(traceRect, _info.Trace, ref _scrollPosition, ref _scrollView, GameFont.Tiny);

    grid[3].yMin += GUIPlus.MediumPadding;
    var buttonGrid = grid[3].GetHGrid(GUIPlus.MediumPadding, buttonWidth, -1f, buttonWidth, buttonWidth);

    if (!_info.IsDuplicate && WidgetsPlus.DrawButton(buttonGrid[1], "Copy to clipboard", font: GameFont.Tiny))
    {
      _info.CopyToClipboard();
      Report.Alert("RimHUD Auto-deactivation details copied to clipboard");
    }

    if (WidgetsPlus.DrawButton(buttonGrid[3], "Reactivate", font: GameFont.Tiny))
    {
      Close();

      if (_info.IsResetOnly) { Persistent.Reset(); }
      State.Activated = true;
    }

    if (WidgetsPlus.DrawButton(buttonGrid[4], "Close", font: GameFont.Tiny)) { Close(); }
  }
}
