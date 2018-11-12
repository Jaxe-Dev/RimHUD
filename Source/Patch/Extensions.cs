using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Interface;
using UnityEngine;
using Verse;

namespace RimHUD.Patch
{
    internal static class Extensions
    {
        public static string Italic(this string self) => "<i>" + self + "</i>";
        public static string Bold(this string self) => "<b>" + self + "</b>";
        public static string Color(this string self, Color color) => $"<color=#{color.ToHex()}>{self}</color>";
        public static string Size(this string self, int size) => $"<size={size}>{self}</size>";
        public static void TryAppendLine(this StringBuilder self, string text)
        {
            if (text != null) { self.AppendLine(text); }
        }
        public static string ToStringTrimmed(this StringBuilder self) => self.ToString().TrimEnd('\n');
        public static int LastIndex(this IList self) => self.Count - 1;
        public static string ToHex(this Color color) => ColorUtility.ToHtmlStringRGBA(color);
        public static int? ToInt(this string self) => int.TryParse(self, out var result) ? result : (int?) null;
        public static bool? ToBool(this string self) => bool.TryParse(self, out var result) ? result : (bool?) null;
        public static string ToDecimalString(this int self, int remainder) => !Theme.ShowDecimals.Value ? self.ToString().Bold() : $"{self.ToString().Bold()}.{(remainder >= 100 ? "99" : remainder.ToString("D2")).Size(Theme.SmallTextStyle.ActualSize)}";
        public static int ToPercentageInt(this float self) => Mathf.RoundToInt(self * 100f);
        public static float ToPercentageFloat(this int self) => self / 100f;

        public static GUIStyle SetTo(this GUIStyle self, int? size = null, TextAnchor? alignment = null, bool? wrap = null) => new GUIStyle(self) { fontSize = size ?? self.fontSize, alignment = alignment ?? self.alignment, wordWrap = wrap ?? self.wordWrap };
        public static GUIStyle ResizedBy(this GUIStyle self, int size = 0) => new GUIStyle(self) { fontSize = self.fontSize + size };

        public static Rect Round(this Rect self) => new Rect(Mathf.Round(self.x), Mathf.Round(self.y), Mathf.Round(self.width), Mathf.Round(self.height));
        public static Rect ContractedBy(this Rect self, float x, float y) => new Rect(self.x + x, self.y + y, self.width - (x * 2f), self.height - (y * 2f));
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

        public static int ComparePartial(this Version self, Version other)
        {
            if (other == null) { return 1; }

            if (self.Major > other.Major) { return 1; }
            if (self.Major < other.Major) { return -1; }

            if ((self.Minor == -1) || (other.Minor == -1)) { return 0; }
            if (self.Minor > other.Minor) { return 1; }
            if (self.Minor < other.Minor) { return -1; }

            if ((self.Build == -1) || (other.Build == -1)) { return 0; }
            if (self.Build > other.Build) { return 1; }
            if (self.Build < other.Build) { return -1; }

            if ((self.Revision == -1) || (other.Revision == -1)) { return 0; }
            if (self.Revision > other.Revision) { return 1; }
            if (self.Revision < other.Revision) { return -1; }

            return 0;
        }

        public static string GetName(this Pawn self) => self.Name?.ToStringFull.CapitalizeFirst() ?? self.LabelCap;
        public static void ShowMenu(this IEnumerable<FloatMenuOption> self) => Find.WindowStack.Add(new FloatMenu(self.ToList()));
    }
}
