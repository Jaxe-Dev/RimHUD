using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimHUD.Extensions
{
  public static class GUIExtensions
  {
    public static string ToHex(this Color color) => ColorUtility.ToHtmlStringRGBA(color);

    public static GUIStyle SetTo(this GUIStyle self, int? size = null, TextAnchor? alignment = null, bool? wrap = null) => new(self) { fontSize = size ?? self.fontSize, alignment = alignment ?? self.alignment, wordWrap = wrap ?? self.wordWrap };

    public static GUIStyle ResizedBy(this GUIStyle self, int size = 0) => new(self) { fontSize = self.fontSize + size };

    public static Rect Round(this Rect self) => new(Mathf.Round(self.x), Mathf.Round(self.y), Mathf.Round(self.width), Mathf.Round(self.height));

    public static Rect[] GetHGrid(this Rect self, float padding, params float[] widths)
    {
      var unfixedCount = 0;
      var currentX = self.x;
      var fixedWidths = 0f;

      var rects = new List<Rect> { self };

      for (var index = 0; index < widths.Length; index++)
      {
        var width = widths[index];
        if (width >= 0f) { fixedWidths += width; }
        else { unfixedCount++; }

        if (index != widths.LastIndex()) { fixedWidths += padding; }
      }

      var unfixedWidth = unfixedCount > 0 ? Mathf.Max(0f, (self.width - fixedWidths) / unfixedCount) : 0f;

      foreach (var width in widths)
      {
        float newWidth;

        if (width >= 0f)
        {
          newWidth = width;
          rects.Add(new Rect(currentX, self.y, newWidth, self.height).Round());
        }
        else
        {
          newWidth = unfixedWidth;
          rects.Add(new Rect(currentX, self.y, newWidth, self.height).Round());
        }

        currentX = Mathf.Min(self.xMax, currentX + newWidth + (newWidth > 0f ? padding : 0f));
      }

      return rects.ToArray();
    }

    public static Rect[] GetVGrid(this Rect self, float padding, params float[] heights)
    {
      var unfixedCount = 0;
      var currentY = self.y;
      var fixedHeights = 0f;

      var rects = new List<Rect> { self };

      for (var index = 0; index < heights.Length; index++)
      {
        var height = heights[index];
        if (height >= 0f) { fixedHeights += height; }
        else { unfixedCount++; }

        if (index != heights.LastIndex()) { fixedHeights += padding; }
      }

      var unfixedHeight = unfixedCount > 0 ? Mathf.Max(0f, (self.height - fixedHeights) / unfixedCount) : 0f;

      foreach (var height in heights)
      {
        float newHeight;

        if (height >= 0f)
        {
          newHeight = height;
          rects.Add(new Rect(self.x, currentY, self.width, newHeight).Round());
        }
        else
        {
          newHeight = unfixedHeight;
          rects.Add(new Rect(self.x, currentY, self.width, newHeight).Round());
        }

        currentY = Mathf.Min(self.yMax, currentY + newHeight + (newHeight > 0f ? padding : 0f));
      }

      return rects.ToArray();
    }

    public static Rect GetCenteredRect(this Vector2 self, float width, float height) => new(self.x - width.Half(), self.y - height.Half(), width, height);
    public static float GetCenteredY(this Rect self, float height) => self.y + (self.height - height).Half();

    public static void ShowMenu(this IEnumerable<FloatMenuOption> self) => Find.WindowStack!.Add(new FloatMenu(self.ToList()));

    public static Vector2 ToGUIPoint(this Vector2 self) => GUIUtility.ScreenToGUIPoint(self * Prefs.UIScale);
    public static Vector2 ToScreenPoint(this Vector2 self) => UI.GUIToScreenPoint(self);
    public static Rect ToGUIRect(this Rect self) => new() { min = self.min.ToGUIPoint(), max = self.max.ToGUIPoint() };
    public static Rect ToScreenRect(this Rect self) => UI.GUIToScreenRect(self);
  }
}
