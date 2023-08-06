using System;
using RimHUD.Configuration;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets
{
  public class SelectorWidget : StandardWidget
  {
    private readonly Color? _color;
    private readonly Action _onClick;
    private readonly Action _onHover;

    public SelectorWidget(string label, Func<string> tooltip, TextStyle textStyle, Color? color, Action onClick, Action onHover) : base(label, tooltip, textStyle)
    {
      _color = color;
      _onClick = onClick;
      _onHover = onHover;
    }

    private SelectorWidget(IModelSelector model, TextStyle textStyle) : this(model.Label, model.Tooltip, textStyle, model.Color, model.OnClick, model.OnHover)
    { }

    public static SelectorWidget FromSelectorModel(IModelSelector model, TextStyle textStyle) => model == null ? null : new SelectorWidget(model, textStyle);

    public override bool Draw(Rect rect)
    {
      if (Label.NullOrEmpty()) { return true; }

      Verse.Widgets.DrawBoxSolid(rect, _color ?? Theme.SelectorBackgroundColor.Value);
      DrawText(rect.ContractedBy(WidgetsPlus.SmallPadding, 0f), Label, Theme.SelectorTextColor.Value);

      if (Mouse.IsOver(rect))
      {
        var border = rect.ContractedBy(-1f);
        Verse.Widgets.DrawBox(border);
        _onHover?.Invoke();
      }

      if (Verse.Widgets.ButtonInvisible(rect)) { _onClick?.Invoke(); }
      return true;
    }
  }
}
