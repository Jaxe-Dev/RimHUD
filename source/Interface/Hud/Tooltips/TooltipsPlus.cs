using System;
using RimHUD.Configuration;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips
{
  public static class TooltipsPlus
  {
    private const float MaxRectHeight = 260f;

    private static GUIStyle HudTooltipStyle => Theme.RegularTextStyle.GUIStyle;

    public static bool IsFromHud(int id) => id is (int)TooltipId.Compact;

    public static void DrawSimple(Rect rect, string? text) => Draw(rect, () => text, TooltipId.Standard);
    public static void DrawStandard(Rect rect, Func<string?>? getter) => Draw(rect, getter, TooltipId.Standard);
    public static void DrawCompact(Rect rect, Func<string?>? getter) => Draw(rect, getter, TooltipId.Compact);
    public static void DrawFinal(Rect rect, string? text) => Draw(rect, () => text, TooltipId.Final);

    public static void DrawInner(Rect rect, string text)
    {
      Verse.Widgets.DrawAtlas(rect, ActiveTip.TooltipBGAtlas);
      WidgetsPlus.DrawMultilineText(rect.ContractedBy(4f), text, HudTooltipStyle);
    }

    public static Rect GetRect(string text)
    {
      var textSize = GUIPlus.GetTextSize(HudTooltipStyle, text, true);
      return new Rect(0f, 0f, Mathf.Min(textSize.x, MaxRectHeight), textSize.x > MaxRectHeight ? GUIPlus.GetTextHeight(HudTooltipStyle, text, MaxRectHeight, true) : textSize.y).ExpandedBy(GUIPlus.SmallPadding).RoundedCeil();
    }

    private static void Draw(Rect rect, Func<string?>? getter, TooltipId id)
    {
      if (getter is null || !Mouse.IsOver(rect)) { return; }

      var text = getter.Invoke();
      if (text.NullOrWhitespace()) { return; }

      TooltipHandler.TipRegion(rect, new TipSignal(text, (int)id));
    }

    private enum TooltipId
    {
      Standard = 10001111,
      Compact = 1000222,
      Final = 10003333
    }
  }
}
