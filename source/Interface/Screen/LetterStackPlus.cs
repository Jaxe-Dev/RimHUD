using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Screen;

public static class LetterStackPlus
{
  private static readonly Lazy<List<Letter>> Letters = new(static () => Reflection.Verse_LetterStack_Letters.GetValue<List<Letter>>(Find.LetterStack));
  private static readonly Lazy<List<Letter>> TmpBundledLetters = new(static () => Reflection.Verse_LetterStack_TmpBundledLetters.GetValue<List<Letter>>(Find.LetterStack));

  public static void Draw(LetterStack instance, float baseY)
  {
    const float letterHeight = 30f;

    var letters = Letters.Value!;
    var tmpBundledLetters = TmpBundledLetters.Value!;

    var hudRect = Theme.GetHudBounds().ExpandedBy(GUIPlus.SmallPadding);
    var excess = Math.Max(letters.Count - Mathf.FloorToInt((baseY - Find.Alerts!.AlertsHeight - hudRect.yMax) / (Theme.LetterPadding.Value + letterHeight)), 0);

    if (excess <= 0)
    {
      SetLastTopYInt(baseY);
      return;
    }

    var isNotRepaint = Event.current!.type is not EventType.Repaint;

    var curY = baseY;

    for (var index = letters.LastIndex(); index > excess; --index)
    {
      curY -= letterHeight;

      var letter = letters[index]!;
      letter.DrawButtonAt(curY);
      if (isNotRepaint) { letter.CheckForMouseOverTextAt(curY); }

      curY -= Theme.LetterPadding.Value;
    }

    tmpBundledLetters.Clear();
    tmpBundledLetters.AddRange(letters.Take(excess + 1));

    instance.BundleLetter!.SetLetters(tmpBundledLetters);

    curY -= letterHeight;

    instance.BundleLetter!.DrawButtonAt(curY);
    if (isNotRepaint) { instance.BundleLetter!.CheckForMouseOverTextAt(curY); }

    curY -= Theme.LetterPadding.Value;

    tmpBundledLetters.Clear();

    SetLastTopYInt(curY);
  }

  private static void SetLastTopYInt(float value) => Reflection.Verse_LetterStack_LastTopYInt.SetValue(Find.LetterStack, value);
}
