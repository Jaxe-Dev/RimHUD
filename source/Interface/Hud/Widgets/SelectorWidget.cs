using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class SelectorWidget(string? label, Func<string?>? tooltip, Action? onClick, Action? onHover, TextStyle textStyle, Color? backColor) : StandardWidget(label, tooltip, textStyle)
{
  public override bool Draw(Rect rect)
  {
    if (Label.NullOrWhitespace()) { return true; }

    Verse.Widgets.DrawBoxSolid(rect, backColor ?? Theme.SelectorBackgroundColor.Value);
    DrawText(rect.ContractedBy(GUIPlus.SmallPadding, 0f), Label, Theme.SelectorTextColor.Value);

    if (Mouse.IsOver(rect))
    {
      var border = rect.ContractedBy(-1f);
      Verse.Widgets.DrawBox(border);
      onHover?.Invoke();
    }

    if (Verse.Widgets.ButtonInvisible(rect)) { onClick?.Invoke(); }
    return true;
  }
}
