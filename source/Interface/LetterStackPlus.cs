using System.Collections.Generic;
using RimHUD.Data.Configuration;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
  internal static class LetterStackPlus
  {
    private const float HudPadding = 8f;

    private const float MinimumCompressedHeight = 200f;

    private const float ControlsWidth = 200f;
    private const float LetterHeight = 30f;
    private const float AlertHeight = 28f;

    private static List<Alert> GetActiveAlerts() => (List<Alert>) Access.Field_RimWorld_AlertsReadout_ActiveAlerts.GetValue(((UIRoot_Play) Find.UIRoot).alerts);
    private static List<Letter> GetLetters() => (List<Letter>) Access.Field_Verse_LetterStack_Letters.GetValue(Find.LetterStack);
    private static void SetLastTopYInt(float value) => Access.Field_Verse_LetterStack_LastTopYInt.SetValue(Find.LetterStack, value);

    public static bool DrawLetters(float baseY)
    {
      var letters = GetLetters();
      if (letters.Count == 0) { return true; }

      var alertsHeight = GetActiveAlerts().Count * AlertHeight;

      var hudRect = Theme.GetHudBounds().ExpandedBy(HudPadding);
      var regularLettersHeight = letters.Count * (LetterHeight + Theme.LetterPadding.Value);
      var controlsHeight = regularLettersHeight + alertsHeight;
      var controlsRect = new Rect(UI.screenWidth - ControlsWidth, baseY - controlsHeight, ControlsWidth, controlsHeight);
      var needsCompression = !Theme.HudDocked.Value && hudRect.Overlaps(controlsRect) && hudRect.yMax < baseY - MinimumCompressedHeight;

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

      SetLastTopYInt(topY);
      return false;
    }
  }
}
