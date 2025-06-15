using RimHUD.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Widgets;

public sealed class MissingWidget : IWidget
{
  private static readonly Color Color = new(0.5f, 0f, 0f);

  private readonly string _label;

  public float GetMaxHeight { get; }

  private MissingWidget(string label)
  {
    _label = label;
    GetMaxHeight = Theme.SmallTextStyle.LineHeight;
  }

  public static MissingWidget Get(string id, HudArgs args) => new(args.DefName.NullOrEmpty() ? $"<{id}>" : $"<{id}[{args.DefName}]>");

  public bool Draw(Rect rect)
  {
    Verse.Widgets.DrawBoxSolid(rect, Color);
    WidgetsPlus.DrawText(rect, _label, Theme.SmallTextStyle);
    return true;
  }
}
