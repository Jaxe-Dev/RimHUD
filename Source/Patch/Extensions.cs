using UnityEngine;

namespace RimHUD.Patch
{
    internal static class Extensions
    {
        public static string Italic(this string self) => "<i>" + self + "</i>";
        public static string Bold(this string self) => "<b>" + self + "</b>";

        public static string ToDecimalString(this int self, int remainder) => $"{self.ToString().Bold()}.{remainder.ToString("D2").Italic()}";
        public static string ToPercentageString(this float self) => self.ToPercentageInt() + "%";
        public static int ToPercentageInt(this float self) => Mathf.RoundToInt(self * 100f);
        public static int WrapTo(this int self, int max) => ((self % max) + max) % max;
    }
}
