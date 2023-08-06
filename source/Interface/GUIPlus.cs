using System.Collections;
using System.Collections.Generic;
using RimHUD.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
  public static class GUIPlus
  {
    private static readonly Stack SavedFonts = new Stack();
    private static readonly Stack SavedColors = new Stack();

    private static readonly Dictionary<string, (int, string)> TruncateCache = new Dictionary<string, (int, string)>();

    public static readonly GUIContent TempContent = new GUIContent();

    public static void SetFont(GameFont? font)
    {
      if (font == null)
      {
        SavedFonts.Push(null);
        return;
      }

      SavedFonts.Push((GameFont?)Text.Font);

      Text.Font = font.Value;
    }

    public static void ResetFont()
    {
      if (SavedFonts.Count == 0) { return; }
      var font = (GameFont?)SavedFonts.Pop();
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

      SavedColors.Push((Color?)GUI.color);

      GUI.color = color.Value;
    }

    public static void ResetColor()
    {
      var color = (Color?)SavedColors.Pop();
      if (color == null) { return; }

      GUI.color = color.Value;
    }

    public static void SetEnabledColor(bool enabled) => SetColor(enabled ? (Color?)null : Theme.DisabledColor.Value);

    public static Color HexToColor(string hex) => ColorUtility.TryParseHtmlString("#" + hex.TrimStart('#'), out var color) ? color : default;

    public static Vector2 GetTextSize(GUIStyle style, string text, bool? wrap = null)
    {
      var originalWrap = style.wordWrap;
      style.wordWrap = wrap ?? originalWrap;

      TempContent.text = text.StripTags();
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

    public static GUIStyle GetGameFontStyle(GameFont font) => Text.fontStyles[(int)font];
  }
}
