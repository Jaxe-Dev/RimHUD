using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimHUD.Interface
{
  public static class WidgetsPlus
  {
    public const int StandardTooltipId = 10001111;
    public const int HudTooltipId = 10002222;

    public const float ScrollbarWidth = 20f;
    public const float ButtonHeight = 30f;
    public const float SmallButtonHeight = 18f;
    public const float TinyPadding = 2f;
    public const float SmallPadding = 4f;
    public const float MediumPadding = 8f;
    public const float LargePadding = 16f;
    public const float XLPadding = 32f;

    public const int SmallFontSize = 11;

    private const string TruncateSymbol = " ...";

    private const int ExpandedTooltipId = 10003333;

    private static readonly GUIStyle HudTooltipStyle = Theme.RegularTextStyle.GUIStyle;

    private static float? _cachedEllipsesWidth;

    public static void DrawText(Rect rect, string text, GUIStyle style = null, Color? color = null, TextAnchor? alignment = null, bool? wrap = null, TipSignal? tooltip = null)
    {
      if (text.NullOrEmpty()) { return; }
      var finalText = text;

      GUIPlus.SetColor(color);
      var textRect = rect;
      var finalStyle = style ?? Text.CurFontStyle;

      var originalWrap = finalStyle.wordWrap;
      finalStyle.wordWrap = wrap ?? originalWrap;

      var originalAlignment = finalStyle.alignment;
      finalStyle.alignment = alignment ?? originalAlignment;

      var textSize = finalStyle.wordWrap ? new Vector2(rect.width, GUIPlus.GetTextHeight(finalStyle, finalText, rect.width)) : GUIPlus.GetTextSize(finalStyle, finalText);

      if (!finalStyle.wordWrap && textSize.y > rect.height && finalText.LastIndexOf('\n') > 0)
      {
        finalText = finalText.FlattenWithSeparator(" | ");
        textSize = GUIPlus.GetTextSize(finalStyle, finalText);
      }

      if (textSize.x > rect.width || textSize.y > rect.height)
      {
        var ellipsesWidth = _cachedEllipsesWidth ?? (_cachedEllipsesWidth = (float)Math.Floor(GUIPlus.GetTextSize(finalStyle, TruncateSymbol).x)).Value;
        textRect.width -= ellipsesWidth;

        var originalClipping = finalStyle.clipping;
        var previousAlignment = finalStyle.alignment;

        finalStyle.clipping = TextClipping.Clip;
        finalStyle.alignment = TextAnchor.MiddleLeft;

        GUI.Label(rect.RightPartPixels(ellipsesWidth), TruncateSymbol, finalStyle);

        finalStyle.clipping = originalClipping;
        finalStyle.alignment = previousAlignment;

        DrawTooltip(rect, () => text, ExpandedTooltipId);
      }

      GUI.Label(textRect, finalText, finalStyle);
      DrawTooltip(rect, tooltip);

      finalStyle.wordWrap = originalWrap;
      finalStyle.alignment = originalAlignment;

      GUIPlus.ResetColor();
    }

    public static void DrawText(Rect rect, string text, TextStyle style, Color? color = null, TextAnchor? alignment = null, bool wrap = false, TipSignal? tooltip = null) => DrawText(rect, text, style.GUIStyle, color, alignment, wrap, tooltip);

    public static void DrawMultilineText(Rect rect, string text, GUIStyle style = null, Color? color = null, TextAnchor? alignment = null, TipSignal? tooltip = null) => DrawText(rect, text, style, color, alignment, true, tooltip);

    public static bool DrawButton(Rect rect, string label, TipSignal? tooltip = null, GameFont? font = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      GUIPlus.SetFont(font);
      var result = Widgets.ButtonText(rect, label, active: enabled);
      DrawTooltip(rect, tooltip);
      GUIPlus.ResetFont();
      GUIPlus.ResetColor();

      return result & enabled;
    }

    public static bool DrawToggle(Rect rect, bool value, TipSignal? tooltip = null, bool enabled = true, Texture2D onTex = null, Texture2D offTex = null)
    {
      GUIPlus.SetEnabledColor(enabled);

      var result = value;
      if (enabled && Widgets.ButtonInvisible(rect))
      {
        result = !value;
        if (result) { SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(); }
        else { SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(); }
      }

      DrawTooltip(rect, tooltip);

      var image = result ? onTex ? onTex : Widgets.CheckboxOnTex : offTex ? offTex : Widgets.CheckboxOffTex;
      GUI.DrawTexture(rect, image);
      GUIPlus.ResetColor();

      return result;
    }

    public static void DrawBar(Rect rect, float percentage, Color color)
    {
      Widgets.DrawBoxSolid(rect, Theme.BarBackgroundColor.Value);
      Widgets.DrawBoxSolid(rect.LeftPart(percentage), color);
    }

    public static void DrawContainer(Rect rect)
    {
      GUIPlus.SetColor(Theme.ContainerColor);
      GUI.DrawTexture(rect, BaseContent.WhiteTex);
      Widgets.DrawBox(rect);
      GUIPlus.ResetColor();
    }

    public static void DrawTooltip(Rect rect, Func<string> tooltip, bool smallText) => DrawTooltip(rect, tooltip, smallText ? HudTooltipId : StandardTooltipId);

    public static void DrawTooltip(Rect rect, Func<string> tooltip, int uniqueId = StandardTooltipId)
    {
      if (tooltip == null || !Mouse.IsOver(rect)) { return; }

      var text = tooltip?.Invoke();
      if (text.NullOrEmpty()) { return; }

      TooltipHandler.TipRegion(rect, new TipSignal(text, uniqueId));
    }

    public static void DrawTooltip(Rect rect, TipSignal? tipSignal)
    {
      if (tipSignal == null || !Mouse.IsOver(rect)) { return; }
      TooltipHandler.TipRegion(rect, tipSignal.Value);
    }

    public static void DrawTooltipInner(Rect rect, string text)
    {
      Widgets.DrawAtlas(rect, ActiveTip.TooltipBGAtlas);
      DrawMultilineText(rect.ContractedBy(4f), text, HudTooltipStyle);
    }

    public static Rect GetTooltipRect(string text)
    {
      const float maxHeight = 260f;

      var textSize = GUIPlus.GetTextSize(HudTooltipStyle, text, true);
      return new Rect(0f, 0f, Mathf.Min(textSize.x, maxHeight), textSize.x > maxHeight ? GUIPlus.GetTextHeight(HudTooltipStyle, text, maxHeight, true) : textSize.y).ExpandedBy(SmallPadding).RoundedCeil();
    }
  }
}
