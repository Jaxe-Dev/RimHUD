using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class SelectorWidget : StandardWidget
{
  private readonly Action? _onClick;
  private readonly Action? _onHover;
  private readonly Color? _backColor;

  public SelectorWidget(string? label, Func<string?>? tooltip, Action? onClick, Action? onHover, TextStyle textStyle, Color? backColor) : base(label, tooltip, textStyle)
  {
    _onClick = onClick;
    _onHover = onHover;
    _backColor = backColor;
  }

  public override bool Draw(Rect rect)
  {
    if (Label.NullOrWhitespace()) { return true; }

    Verse.Widgets.DrawBoxSolid(rect, _backColor ?? Theme.SelectorBackgroundColor.Value);
    DrawText(rect.ContractedBy(GUIPlus.SmallPadding, 0f), Label, Theme.SelectorTextColor.Value);

    if (Mouse.IsOver(rect))
    {
      Verse.Widgets.DrawBox(rect.ExpandedBy(1f));
      _onHover?.Invoke();
    }

    if (Verse.Widgets.ButtonInvisible(rect)) { _onClick?.Invoke(); }
    return true;
  }
}
