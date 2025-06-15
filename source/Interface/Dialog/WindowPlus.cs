using RimHUD.Configuration;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog;

public abstract class WindowPlus : Window
{
  private const float CloseButtonOffset = 55f;

  protected abstract void DrawContent(Rect rect);

  public override Vector2 InitialSize { get; }

  private readonly string _title;
  private readonly string? _subtitle;

  protected WindowPlus(Vector2 size, string title, string? subtitle = null)
  {
    draggable = true;
    doCloseX = true;
    doCloseButton = true;
    absorbInputAroundWindow = true;
    closeOnClickedOutside = false;
    closeOnAccept = false;

    InitialSize = size == default ? new Vector2(800f, 600f) : size;

    _title = title;
    _subtitle = subtitle;
  }

  public override void DoWindowContents(Rect rect) => DrawContent(DrawTitle(rect));

  private Rect DrawTitle(Rect rect)
  {
    if (_title.NullOrEmpty()) { return rect; }

    GUIPlus.SetWrap(false);

    var header = new ListingPlus();

    header.Begin(rect);

    header.Label(_title.Bold(), font: GameFont.Medium);

    if (!_subtitle.NullOrWhitespace())
    {
      var titleSize = GUIPlus.GetTextSize(GUIPlus.GetGameFontStyle(GameFont.Medium), _title);
      var titleOffset = titleSize.x + GUIPlus.MediumPadding;
      var subtitleRect = new Rect(rect.x + titleOffset, rect.y, rect.width - titleOffset, titleSize.y);
      WidgetsPlus.DrawText(subtitleRect, _subtitle.Italic(), Theme.SmallTextStyle);
    }

    header.GapLine();
    header.End();

    var contentRect = new Rect(rect.x, rect.y + header.CurHeight, rect.width, rect.height - header.CurHeight);

    if (doCloseButton) { contentRect.height -= CloseButtonOffset; }

    GUIPlus.ResetWrap();

    return contentRect;
  }
}
