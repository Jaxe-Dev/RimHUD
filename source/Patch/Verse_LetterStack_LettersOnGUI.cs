using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(LetterStack), "LettersOnGUI")]
  public static class Verse_LetterStack_LettersOnGUI
  {
    private static bool Prefix(float baseY) => !State.CompressLetters || LetterStackPlus.DrawLetters(baseY);
  }
}
