using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;
using Verse;

namespace RimHUD;

public sealed class CustomWidgetDef : ExternalWidgetDef, IWidget
{
  [Unsaved]
  private ExternalMethodHandler<bool>? _onDraw;
  [Unsaved]
  private ExternalMethodHandler<float>? _getMaxHeight;

  public float GetMaxHeight => _getMaxHeight?.Invoke() ?? 0f;
  public bool Draw(Rect rect) => _onDraw?.Invoke(Active.Pawn, rect) ?? false;

  protected override void InitializeV1()
  {
    _onDraw = GetHandler<bool>(true, "OnDraw", typeof(Pawn), typeof(Rect));
    _getMaxHeight = GetHandler<float>(true, "GetMaxHeight");
  }
}
