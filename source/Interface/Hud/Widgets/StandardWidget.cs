using System;
using RimHUD.Configuration;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public abstract class StandardWidget : IWidget
  {
    protected string Label { get; }
    protected Func<string> Tooltip { get; }
    private readonly TextStyle _textStyle;

    public float Height { get; }

    protected StandardWidget(string label, Func<string> tooltip, TextStyle textStyle)
    {
      Label = label;
      Tooltip = tooltip;
      _textStyle = textStyle;
      Height = _textStyle.LineHeight;
    }

    public abstract bool Draw(Rect rect);

    protected void DrawText(Rect rect, string text, Color? color = null, TextAnchor? alignment = null) => WidgetsPlus.DrawText(rect, text, _textStyle, color, alignment);
  }
}
