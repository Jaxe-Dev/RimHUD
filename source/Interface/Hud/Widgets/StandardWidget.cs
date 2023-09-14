using System;
using RimHUD.Configuration.Settings;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public abstract class StandardWidget : IWidget
  {
    public abstract bool Draw(Rect rect);
    private readonly TextStyle _textStyle;

    protected string? Label { get; }
    protected Func<string?>? Tooltip { get; }

    public float GetMaxHeight { get; }

    protected StandardWidget(string? label, Func<string?>? tooltip, TextStyle textStyle)
    {
      Label = label;
      Tooltip = tooltip;

      _textStyle = textStyle;
      GetMaxHeight = textStyle.LineHeight;
    }

    protected void DrawText(Rect rect, string? text, Color? color = null, TextAnchor? alignment = null) => WidgetsPlus.DrawText(rect, text, _textStyle, color, alignment);
  }
}
