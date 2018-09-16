using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    public static class LetterStackPlus
    {
        private const float HudPadding = 8f;

        private const float MinimumCompressedHeight = 200f;

        private const float ControlsWidth = 200f;
        private const float LetterHeight = 30f;
        private const float AlertHeight = 28f;

        public static bool DrawLetters(float baseY)
        {
            if (!Hud.Activated || !Theme.LetterCompress) { return true; }

            var letters = Access.Field_Verse_LetterStack_Letters_Get();
            if (letters.Count == 0) { return true; }

            var alertsHeight = Access.Field_RimWorld_AlertsReadout_ActiveAlerts_Get().Count * AlertHeight;

            var hudRect = Hud.Bounds.ExpandedBy(HudPadding);
            var regularLettersHeight = letters.Count * (LetterHeight + Theme.LetterPadding.Value);
            var controlsHeight = regularLettersHeight + alertsHeight;
            var controlsRect = new Rect(UI.screenWidth - ControlsWidth, baseY - controlsHeight, ControlsWidth, controlsHeight);
            var needsCompression = hudRect.Overlaps(controlsRect) && (hudRect.yMax < baseY - MinimumCompressedHeight);

            var minY = (needsCompression ? hudRect.yMax : 0f) + alertsHeight;

            var letterHeightCompressed = Mathf.Min((baseY - minY) / letters.Count, LetterHeight + Theme.LetterPadding.Value);
            var topY = (baseY - (letterHeightCompressed * letters.Count)) + Theme.LetterPadding.Value;
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
