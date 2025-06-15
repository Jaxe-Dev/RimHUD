using System;
using RimHUD.Configuration.Settings;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets;

public abstract class StandardWidget(string? label, Func<string?>? tooltip, TextStyle textStyle) : IWidget
{
  public abstract bool Draw(Rect rect);

  protected string? Label { get; } = label;
  protected Func<string?>? Tooltip { get; } = tooltip;

  public float GetMaxHeight { get; } = textStyle.LineHeight;

  protected void DrawText(Rect rect, string? text, Color? color = null, TextAnchor? alignment = null) => WidgetsPlus.DrawText(rect, text, textStyle, color, alignment);
}
