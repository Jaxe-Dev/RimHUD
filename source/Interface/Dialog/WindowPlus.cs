using RimHUD.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  public abstract class WindowPlus : Window
  {
    private const float CloseButtonOffset = 55f;

    public override Vector2 InitialSize { get; }
    protected string Title { get; set; }
    protected string Subtitle { get; set; }

    protected WindowPlus(Vector2 size) : this(null, size)
    { }

    protected WindowPlus(string title = null, Vector2 size = default)
    {
      draggable = true;
      doCloseX = true;
      doCloseButton = true;
      absorbInputAroundWindow = true;
      closeOnClickedOutside = false;
      closeOnAccept = false;

      InitialSize = size == default ? new Vector2(800f, 600f) : size;
      Title = title;
    }

    protected abstract void DrawContent(Rect rect);

    public override void DoWindowContents(Rect rect) => DrawContent(DrawTitle(rect));

    private Rect DrawTitle(Rect rect)
    {
      if (Title.NullOrEmpty()) { return rect; }

      var wordWrap = Text.WordWrap;
      Text.WordWrap = false;

      var header = new ListingPlus();

      header.Begin(rect);

      header.Label(Title, font: GameFont.Medium);

      if (!string.IsNullOrEmpty(Subtitle))
      {
        var titleSize = GUIPlus.GetTextSize(GUIPlus.GetGameFontStyle(GameFont.Medium), Title);
        var titleOffset = titleSize.x + WidgetsPlus.MediumPadding;
        var subtitleRect = new Rect(rect.x + titleOffset, rect.y, rect.width - titleOffset, titleSize.y);
        WidgetsPlus.DrawText(subtitleRect, Subtitle, Theme.SmallTextStyle);
      }

      header.GapLine();
      header.End();

      var contentRect = new Rect(rect.x, rect.y + header.CurHeight, rect.width, rect.height - header.CurHeight);

      if (doCloseButton) { contentRect.height -= CloseButtonOffset; }

      Text.WordWrap = wordWrap;

      return contentRect;
    }
  }
}
