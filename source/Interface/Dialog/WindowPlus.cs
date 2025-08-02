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

  protected string Title { get; set; }
  protected string? Subtitle { get; set; }

  protected WindowPlus(Vector2 size, string title, string? subtitle = null)
  {
    draggable = true;
    doCloseX = true;
    doCloseButton = true;
    absorbInputAroundWindow = true;
    closeOnClickedOutside = false;
    closeOnAccept = false;

    InitialSize = size == default ? new Vector2(800f, 600f) : size;

    Title = title;
    Subtitle = subtitle;
  }

  public override void DoWindowContents(Rect rect) => DrawContent(DrawTitle(rect));

  private Rect DrawTitle(Rect rect)
  {
    if (Title.NullOrEmpty()) { return rect; }

    GUIPlus.SetWrap(false);

    var header = new ListingPlus();

    header.Begin(rect);

    header.Label(Title.Bold(), font: GameFont.Medium);

    if (!Subtitle.NullOrWhitespace())
    {
      var titleSize = GUIPlus.GetTextSize(GUIPlus.GetGameFontStyle(GameFont.Medium), Title);
      var titleOffset = titleSize.x + GUIPlus.MediumPadding;
      var subtitleRect = new Rect(rect.x + titleOffset, rect.y + 1f, rect.width - titleOffset, titleSize.y);

      WidgetsPlus.DrawText(subtitleRect, Subtitle, Theme.TinyUITextStyle, alignment: TextAnchor.MiddleLeft);
    }

    header.GapLine();
    header.End();

    var contentRect = new Rect(rect.x, rect.y + header.CurHeight, rect.width, rect.height - header.CurHeight);

    if (doCloseButton) { contentRect.height -= CloseButtonOffset; }

    GUIPlus.ResetWrap();

    return contentRect;
  }
}
