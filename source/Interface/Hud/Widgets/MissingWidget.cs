using RimHUD.Configuration;
using UnityEngine;

namespace RimHUD.Interface.Hud.Widgets
{
  public class MissingWidget : IWidget
  {
    private static readonly Color Color = new Color(0.5f, 0f, 0f);

    public float Height { get; }

    private readonly string _id;

    private MissingWidget(string id)
    {
      _id = id;
      Height = Theme.SmallTextStyle.LineHeight;
    }

    public static MissingWidget Get(string id, string defName) => new MissingWidget($"<{id}{(defName == null ? null : ":" + defName)}>");

    public bool Draw(Rect rect)
    {
      Verse.Widgets.DrawBoxSolid(rect, Color);
      WidgetsPlus.DrawText(rect, _id, Theme.SmallTextStyle);
      return true;
    }
  }
}
