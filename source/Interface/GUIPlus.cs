using System.Collections.Generic;
using RimHUD.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
  public static class GUIPlus
  {
    public const float TinyPadding = 2f;
    public const float SmallPadding = 4f;
    public const float MediumPadding = 8f;
    public const float LargePadding = 16f;
    public const float XLPadding = 32f;

    private static readonly Stack<GameFont?> SavedFonts = new();
    private static readonly Stack<Color?> SavedColors = new();
    private static readonly Stack<TextAnchor?> SavedAnchors = new();
    private static readonly Stack<bool?> SavedWraps = new();

    private static readonly GUIContent TempContent = new();

    public static void SetFont(GameFont? font)
    {
      if (font is null)
      {
        SavedFonts.Push(null);
        return;
      }

      SavedFonts.Push(Text.Font);

      Text.Font = font.Value;
    }

    public static void ResetFont()
    {
      if (SavedFonts.Count is 0) { return; }
      var font = SavedFonts.Pop();
      if (font is null) { return; }

      Text.Font = font.Value;
    }

    public static void SetColor(Color? color)
    {
      if (color is null)
      {
        SavedColors.Push(null);
        return;
      }

      SavedColors.Push(GUI.color);

      GUI.color = color.Value;
    }

    public static void ResetColor()
    {
      var color = SavedColors.Pop();
      if (color is null) { return; }

      GUI.color = color.Value;
    }

    public static void SetAnchor(TextAnchor? anchor)
    {
      if (anchor is null)
      {
        SavedAnchors.Push(null);
        return;
      }

      SavedAnchors.Push(Text.Anchor);

      Text.Anchor = anchor.Value;
    }

    public static void ResetAnchor()
    {
      var anchor = SavedAnchors.Pop();
      if (anchor is null) { return; }

      Text.Anchor = anchor.Value;
    }

    public static void SetWrap(bool? wrap)
    {
      if (wrap is null)
      {
        SavedWraps.Push(null);
        return;
      }

      SavedWraps.Push(Text.WordWrap);

      Text.WordWrap = wrap.Value;
    }

    public static void ResetWrap()
    {
      var wrap = SavedWraps.Pop();
      if (wrap is null) { return; }

      Text.WordWrap = wrap.Value;
    }

    public static void SetEnabledColor(bool enabled) => SetColor(enabled ? null : Theme.DisabledColor.Value);

    public static Color HexToColor(string hex) => ColorUtility.TryParseHtmlString($"#{hex.TrimStart('#')}", out var color) ? color : default;

    public static Vector2 GetTextSize(GameFont font, string text, bool? wrap = null, bool stripTags = true) => GetTextSize(GetGameFontStyle(font), text, wrap, stripTags);

    public static Vector2 GetTextSize(GUIStyle style, string text, bool? wrap = null, bool stripTags = true)
    {
      var originalWrap = style.wordWrap;
      style.wordWrap = wrap ?? originalWrap;

      TempContent.text = stripTags ? text.StripTags() : text;
      var size = style.CalcSize(TempContent);
      TempContent.text = null;

      style.wordWrap = originalWrap;

      return size;
    }

    public static float GetTextHeight(GUIStyle style, string text, float width, bool? wrap = null)
    {
      var originalWrap = style.wordWrap;
      style.wordWrap = wrap ?? originalWrap;

      TempContent.text = text.StripTags();
      var height = style.CalcHeight(TempContent, width);
      TempContent.text = null;

      style.wordWrap = originalWrap;

      return height;
    }

    public static GUIStyle GetGameFontStyle(GameFont font) => Text.fontStyles![(int)font];
  }
}
