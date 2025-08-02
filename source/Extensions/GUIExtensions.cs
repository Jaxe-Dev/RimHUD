using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimHUD.Extensions;

public static class GUIExtensions
{
  public static string ToHex(this Color color) => ColorUtility.ToHtmlStringRGBA(color);

  public static GUIStyle SetTo(this GUIStyle self, int? size = null, TextAnchor? alignment = null, bool? wrap = null) => new(self) { fontSize = size ?? self.fontSize, alignment = alignment ?? self.alignment, wordWrap = wrap ?? self.wordWrap };

  public static GUIStyle ResizedBy(this GUIStyle self, int size = 0) => new(self) { fontSize = self.fontSize + size };

  public static Rect Round(this Rect self) => new(Mathf.Round(self.x), Mathf.Round(self.y), Mathf.Round(self.width), Mathf.Round(self.height));

  public static Rect[] GetGrid(this Rect self, float padding, float[] widths, float[] heights)
  {
    var grid = new Rect[1 + (widths.Length * heights.Length)];
    grid[0] = self;

    var remainingWidth = self.width - (padding * (widths.Length - 1));
    var columns = new float[widths.Length];
    var fillColumns = 0;
    foreach (var width in widths)
    {
      if (width < 0f) { fillColumns++; }
      else { remainingWidth -= width; }
    }
    for (var i = 0; i < widths.Length; i++) { columns[i] = widths[i] < 0f ? fillColumns > 0 ? Mathf.Max(0f, remainingWidth / fillColumns) : 0f : widths[i]; }

    var rows = new float[heights.Length];
    var remainingHeight = self.height - (padding * (heights.Length - 1));
    var fillRows = 0;
    foreach (var height in heights)
    {
      if (height < 0f) { fillRows++; }
      else { remainingHeight -= height; }
    }
    for (var i = 0; i < heights.Length; i++) { rows[i] = heights[i] < 0f ? fillRows > 0 ? Mathf.Max(0f, remainingHeight / fillRows) : 0f : heights[i]; }

    var y = self.y;
    var index = 1;
    for (var row = 0; row < heights.Length; row++)
    {
      var x = self.x;
      for (var column = 0; column < widths.Length; column++)
      {
        var w = columns[column];
        var cell = new Rect(x, y, w, rows[row]);
        grid[index++] = w <= 0f || rows[row] <= 0f ? new Rect(0, 0, 0, 0) : cell;
        x += w + (w > 0f ? padding : 0f);
      }
      y += rows[row] + (rows[row] > 0f ? padding : 0f);
    }

    return grid;
  }

  public static Rect[] GetHGrid(this Rect self, float padding, params float[] widths) => GetGrid(self, padding, widths, [self.height]);
  public static Rect[] GetVGrid(this Rect self, float padding, params float[] heights) => GetGrid(self, padding, [self.width], heights);

  public static Rect GetCenteredRect(this Vector2 self, float width, float height) => new(self.x - width.Half(), self.y - height.Half(), width, height);
  public static float GetCenteredY(this Rect self, float height) => self.y + (self.height - height).Half();

  public static void ShowMenu(this IEnumerable<FloatMenuOption> self) => Find.WindowStack!.Add(new FloatMenu(self.ToList()));

  public static Rect ToGUIRect(this Rect self) => new() { min = GUIUtility.ScreenToGUIPoint(self.min * Prefs.UIScale), max = GUIUtility.ScreenToGUIPoint(self.max * Prefs.UIScale) };
  public static Rect ToScreenRect(this Rect self) => UI.GUIToScreenRect(self);
}
