using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface
{
    public static class LetterStackPlus
    {
        private const float Padding = 8f;

        private const float AlertHeight = 28f;
        private const float LetterHeight = 32f;

        public static bool DrawLetters(float baseY)
        {
            if (!Hud.Activated) { return true; }

            var letters = Access.Field_Verse_LetterStack_Letters_Get();
            if (letters.Count == 0) { return true; }

            var alertsHeight = Access.Field_RimWorld_AlertsReadout_ActiveAlerts_Get().Count * AlertHeight;

            var minY = Hud.Bounds.yMax + Padding + alertsHeight;

            var letterHeightCompressed = Mathf.Min((baseY - minY) / letters.Count, LetterHeight);
            var topY = baseY - (letterHeightCompressed * letters.Count);
            var curY = topY;

            foreach (var letter in letters.ToArray())
            {
                if (Event.current.type == EventType.Repaint) { letter.CheckForMouseOverTextAt(curY); }
                letter.DrawButtonAt(curY);
                curY += letterHeightCompressed;
            }

            Access.Field_Verse_LetterStack_LastTopYInt_Set(topY);
            return false;
        }
    }
}
