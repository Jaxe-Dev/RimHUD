﻿using System;
using System.Collections;
using System.Linq;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimHUD.Interface
{
  internal static class GUIPlus
  {
    public const int TooltipId = 10001000;
    public const int TooltipExpandedId = 10001001;
    public const float ScrollbarWidth = 20f;
    public const float ButtonHeight = 30f;
    public const float SmallButtonHeight = 18f;
    public const float TinyPadding = 2f;
    public const float SmallPadding = 4f;
    public const float MediumPadding = 8f;
    public const float LargePadding = 16f;

    private static readonly Stack SavedColors = new Stack();
    private static readonly Stack SavedFonts = new Stack();

    public static readonly Color ButtonSelectedColor = new Color(0.5f, 1f, 0.5f);
    public static readonly Color ItemSelectedColor = new Color(0.25f, 0.4f, 0.1f);

    private static readonly GUIContent TextContent = new GUIContent();

    public static void SetFont(GameFont? font)
    {
      if (font == null)
      {
        SavedFonts.Push(null);
        return;
      }

      SavedFonts.Push((GameFont?) Text.Font);

      Text.Font = font.Value;
    }

    public static void ResetFont()
    {
      if (SavedFonts.Count == 0) { return; }
      var font = (GameFont?) SavedFonts.Pop();
      if (font == null) { return; }

      Text.Font = font.Value;
    }

    public static void SetColor(Color? color)
    {
      if (color == null)
      {
        SavedColors.Push(null);
        return;
      }

      SavedColors.Push((Color?) GUI.color);

      GUI.color = color.Value;
    }

    public static void ResetColor()
    {
      var color = (Color?) SavedColors.Pop();
      if (color == null) { return; }

      GUI.color = color.Value;
    }

    public static void SetEnabledColor(bool enabled) => SetColor(enabled ? (Color?) null : Theme.DisabledColor.Value);

    public static void DrawText(Rect rect, string text, Color? color = null, TextStyle style = null, TextAnchor? alignment = null, TipSignal? tooltip = null)
    {
      if (text.NullOrEmpty()) { return; }
      var textFinal = text;

      SetColor(color);
      var textRect = rect;
      var guiStyle = style?.GUIStyle ?? Text.CurFontStyle;
      var originalAlignment = guiStyle.alignment;
      if (alignment != null) { guiStyle.alignment = alignment.Value; }

      TextContent.text = textFinal;
      var calcSize = guiStyle.CalcSize(TextContent);

      if (calcSize.y > rect.height && textFinal.LastIndexOf('\n') > 0)
      {
        textFinal = textFinal.FlattenWithSeparator(" | ");

        TextContent.text = textFinal;
        calcSize = guiStyle.CalcSize(TextContent);
      }

      if (calcSize.x > rect.width || calcSize.y > rect.height)
      {
        TextContent.text = "...";
        var ellipsesLength = guiStyle.CalcSize(TextContent);
        textRect.width -= ellipsesLength.x;
        GUI.Label(new Rect(rect.RightPartPixels(ellipsesLength.x)), TextContent.text, guiStyle);
        DrawTooltip(rect, () => text.Size(guiStyle.fontSize), false, TooltipExpandedId);
      }

      GUI.Label(textRect, textFinal, guiStyle);
      DrawTooltip(rect, tooltip, false);

      TextContent.text = "";
      guiStyle.alignment = originalAlignment;
      ResetColor();
    }

    public static bool DrawButton(Rect rect, Texture2D texture, TipSignal? tooltip = null, bool enabled = true)
    {
      SetEnabledColor(enabled);
      var result = Widgets.ButtonImage(rect, texture);
      ResetColor();

      DrawTooltip(rect, tooltip, false);

      return result & enabled;
    }

    public static bool DrawButton(Rect rect, string label, TipSignal? tooltip = null, GameFont? font = null, bool enabled = true)
    {
      SetEnabledColor(enabled);
      SetFont(font);
      var result = Widgets.ButtonText(rect, label, active: enabled);
      DrawTooltip(rect, tooltip, true);
      ResetFont();
      ResetColor();

      return result & enabled;
    }

    public static int DrawButtonRow(Rect rect, float width, float padding, params string[] labels)
    {
      var grid = rect.GetHGrid(padding, Enumerable.Repeat(width, labels.Length).ToArray());

      for (var index = 0; index < labels.Length; index++)
      {
        if (DrawButton(grid[index + 1], labels[index])) { return index + 1; }
      }

      return 0;
    }

    public static bool DrawToggle(Rect rect, bool value, TipSignal? tooltip = null, bool enabled = true, Texture2D onTex = null, Texture2D offTex = null, bool highlight = true)
    {
      SetEnabledColor(enabled);

      var result = value;
      if (enabled && Widgets.ButtonInvisible(rect))
      {
        result = !value;
        if (result) { SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(); }
        else { SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(); }
      }

      DrawTooltip(rect, tooltip, highlight);

      var image = result ? onTex ? onTex : Widgets.CheckboxOnTex : offTex ? offTex : Widgets.CheckboxOffTex;
      GUI.DrawTexture(rect, image);
      ResetColor();

      return result;
    }

    public static void DrawBar(Rect rect, float percentage, Color color)
    {
      Widgets.DrawBoxSolid(rect, Theme.BarBackgroundColor.Value);
      Widgets.DrawBoxSolid(rect.LeftPart(percentage), color);
    }

    public static TipSignal? PrepareTipSignal(Func<string> textGetter, int uniqueId = TooltipId) => textGetter == null ? null : new TipSignal(textGetter, uniqueId);

    public static void DrawTooltip(Rect rect, Func<string> tooltip, bool highlight, int uniqueId = TooltipId)
    {
      if (tooltip == null || !Mouse.IsOver(rect)) { return; }

      var text = tooltip?.Invoke();
      if (text == null) { return; }

      var tipSignal = new TipSignal(text, uniqueId);

      if (highlight) { Widgets.DrawHighlight(rect); }
      TooltipHandler.TipRegion(rect, tipSignal);
    }

    public static void DrawTooltip(Rect rect, TipSignal? tipSignal, bool highlight)
    {
      if (tipSignal == null || !Mouse.IsOver(rect)) { return; }

      if (highlight) { Widgets.DrawHighlight(rect); }
      TooltipHandler.TipRegion(rect, tipSignal.Value);
    }

    public static Color HexToColor(string hex) => ColorUtility.TryParseHtmlString("#" + hex.TrimStart('#'), out var color) ? color : default;

    public static Vector2 GetTextSize(string text, GUIStyle style) => style.CalcSize(new GUIContent(text));

    public static GUIStyle GetGameFontStyle(GameFont font) => Text.fontStyles[(int) font];
    public static GUIStyle GetGameReadOnlyFontStyle(GameFont font) => Text.textAreaReadOnlyStyles[(int) font];
  }
}
