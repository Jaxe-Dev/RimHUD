using System.Collections;
using System.Text;
using RimHUD.Data.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Extensions
{
    internal static class BaseExtensions
    {
        public static string Italic(this string self) => "<i>" + self + "</i>";
        public static string Bold(this string self) => "<b>" + self + "</b>";
        public static string Color(this string self, string hex) => $"<color=#{hex}>{self}</color>";
        public static string Color(this string self, Color color) => $"<color=#{color.ToHex()}>{self}</color>";
        public static TaggedString Color(this TaggedString self, Color color) => $"<color=#{color.ToHex()}>{self}</color>";
        public static string Size(this string self, int size) => $"<size={size}>{self}</size>";

        public static void TryAppendLine(this StringBuilder self, string text)
        {
            if (text != null) { self.AppendLine(text); }
        }

        public static string ToStringTrimmed(this StringBuilder self) => self.ToString().TrimEndNewlines();
        public static int LastIndex(this IList self) => self.Count - 1;
        public static int? ToInt(this string self) => int.TryParse(self, out var result) ? result : (int?) null;
        public static bool? ToBool(this string self) => bool.TryParse(self, out var result) ? result : (bool?) null;
        public static string ToDecimalString(this int self, int remainder) => !Theme.ShowDecimals.Value ? self.ToString().Bold() : $"{self.ToString().Bold()}.{(remainder >= 100 ? "99" : remainder.ToString("D2")).Size(Theme.SmallTextStyle.ActualSize)}";
        public static int ToPercentageInt(this float self) => Mathf.RoundToInt(self * 100f);
        public static float ToPercentageFloat(this int self) => self / 100f;
    }
}
