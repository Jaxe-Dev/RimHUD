using System;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimHUD.Interface
{
  public static class WidgetsPlus
  {
    public const float ScrollbarWidth = 20f;

    public const float ButtonHeight = 30f;
    public const float MainButtonHeight = 35f;
    public const float TabButtonHeight = ButtonHeight;
    public const float SmallButtonHeight = 18f;

    public const int SmallFontSize = 11;

    private static readonly string TruncateSymbol = " ...".Colorize(Color.gray);
    private static readonly string SeparatorSymbol = " | ".Colorize(Color.gray);

    private static float? _cachedEllipsesWidth;

    public static void DrawText(Rect rect, string? text, GUIStyle? style = null, Color? color = null, TextAnchor? alignment = null, bool? wrap = null)
    {
      if (text.NullOrWhitespace()) { return; }
      var finalText = text;

      GUIPlus.SetColor(color);
      var textRect = rect;
      var finalStyle = style ?? Text.CurFontStyle;

      var originalWrap = finalStyle!.wordWrap;
      finalStyle.wordWrap = wrap ?? originalWrap;

      var originalAlignment = finalStyle.alignment;
      finalStyle.alignment = alignment ?? originalAlignment;

      var textSize = finalStyle.wordWrap ? new Vector2(rect.width, GUIPlus.GetTextHeight(finalStyle, finalText!, rect.width)) : GUIPlus.GetTextSize(finalStyle, finalText!);

      if (!finalStyle.wordWrap && textSize.y > rect.height && finalText!.LastIndexOf('\n') > 0)
      {
        finalText = finalText.FlattenWithSeparator(SeparatorSymbol);
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

        TooltipsPlus.DrawFinal(rect, text);
      }

      GUI.Label(textRect, finalText, finalStyle);

      finalStyle.wordWrap = originalWrap;
      finalStyle.alignment = originalAlignment;

      GUIPlus.ResetColor();
    }

    public static void DrawText(Rect rect, string? text, TextStyle textStyle, Color? color = null, TextAnchor? alignment = null, bool wrap = false) => DrawText(rect, text, textStyle.GUIStyle, color, alignment, wrap);

    public static void DrawText(Rect rect, string? text, GameFont font, Color? color = null, TextAnchor? alignment = null, bool wrap = false) => DrawText(rect, text, GUIPlus.GetGameFontStyle(font), color, alignment, wrap);

    public static void DrawMultilineText(Rect rect, string text, GUIStyle? style = null, Color? color = null, TextAnchor? alignment = null) => DrawText(rect, text, style, color, alignment, true);

    public static void DrawScrollableText(Rect rect, string text, ref Vector2 scrollPosition, ref Rect scrollRect, GameFont? font = null)
    {
      if (text.NullOrWhitespace()) { return; }

      GUIPlus.SetFont(font);
      GUIPlus.SetWrap(false);

      var size = GUIPlus.GetTextSize(font ?? Text.Font, text, stripTags: false);

      scrollRect = new Rect(rect.position, size);

      Widgets.BeginScrollView(rect, ref scrollPosition, scrollRect);
      Widgets.Label(scrollRect, text);
      Widgets.EndScrollView();

      GUIPlus.ResetFont();
      GUIPlus.ResetWrap();
    }

    public static bool DrawButton(Rect rect, string label, Func<string>? tooltip = null, GameFont? font = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      GUIPlus.SetFont(font);
      var result = Widgets.ButtonText(rect, label, active: enabled);
      TooltipsPlus.DrawStandard(rect, tooltip);
      GUIPlus.ResetFont();
      GUIPlus.ResetColor();

      return result & enabled;
    }

    public static bool DrawToggle(Rect rect, bool value, Func<string?>? tooltip = null, bool enabled = true, Texture2D? onTex = null, Texture2D? offTex = null)
    {
      GUIPlus.SetEnabledColor(enabled);

      var result = value;
      if (enabled && Widgets.ButtonInvisible(rect))
      {
        result = !value;
        if (result) { SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(); }
        else { SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(); }
      }

      TooltipsPlus.DrawStandard(rect, tooltip);

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

    public static float DrawHorizontalSlider(Rect rect, float value, float min, float max)
    {
      const float railHeight = 8f;
      const float handleSize = 12f;

      var draggingId = Gen.HashCombine(Gen.HashCombine(Gen.HashCombine(Gen.HashCombine(new Vector2(rect.x, rect.y).ToScreenPoint().GetHashCode(), rect.width), rect.height), min), max);
      var draggableRect = new Rect(rect.x + handleSize.Half(), rect.y, rect.width - handleSize, rect.height);

      GUI.color = Reflection.Verse_Widgets_RangeControlTextColor.GetValueStatic<Color>();
      var railRect = new Rect(draggableRect.x, draggableRect.center.y - railHeight.Half(), draggableRect.width, railHeight);
      Widgets.DrawAtlas(railRect, Reflection.Verse_Widgets_SliderRailAtlas.GetValueStatic<Texture2D>());
      GUI.color = Color.white;

      var newValue = value;
      var handleRect = new Rect(Mathf.Clamp(rect.x + (draggableRect.width * Mathf.InverseLerp(min, max, newValue)), rect.x, draggableRect.xMax - handleSize.Half()), railRect.center.y - handleSize.Half(), handleSize, handleSize);
      GUI.DrawTexture(handleRect, Reflection.Verse_Widgets_SliderHandle.GetValueStatic<Texture2D>());

      if (Event.current!.type is EventType.MouseDown && Mouse.IsOver(rect) && Reflection.Verse_Widgets_SliderDraggingId.GetValueStatic<int>() != draggingId)
      {
        Reflection.Verse_Widgets_SliderDraggingId.SetValueStatic(draggingId);
        SoundDefOf.DragSlider.PlayOneShotOnCamera();
        Event.current.Use();
      }

      if (Reflection.Verse_Widgets_SliderDraggingId.GetValueStatic<int>() == draggingId && UnityGUIBugsFixer.MouseDrag())
      {
        newValue = Mathf.Clamp((((Event.current.mousePosition.x - draggableRect.x) / draggableRect.width) * (max - min)) + min, min, max);
        if (Event.current.type is EventType.MouseDrag) { Event.current.Use(); }
      }

      if (!value.Equals(newValue)) { Reflection.Verse_Widgets_CheckPlayDragSliderSound.InvokeStatic(); }
      return newValue;
    }
  }
}
