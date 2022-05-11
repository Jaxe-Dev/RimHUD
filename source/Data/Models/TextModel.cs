using System;

namespace RimHUD.Data.Models
{
  internal class TextModel
  {
    public string Text { get; }
    public Func<string> Tooltip { get; }
    public Action OnClick { get; }

    private TextModel(string text, Func<string> tooltip, Action onClick)
    {
      Text = text;
      Tooltip = tooltip;
      OnClick = onClick;
    }

    public static TextModel Create(string text, Func<string> tooltip = null, Action onClick = null) => text == null ? null : new TextModel(text, tooltip, onClick);
  }
}
