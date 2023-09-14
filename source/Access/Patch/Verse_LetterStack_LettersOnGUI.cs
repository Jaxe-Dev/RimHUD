using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(LetterStack), "LettersOnGUI")]
  public static class Verse_LetterStack_LettersOnGUI
  {
    private static bool Prefix(LetterStack __instance, ref float baseY)
    {
      if (!State.CompressLetters) { return true; }

      LetterStackPlus.Draw(__instance, baseY);
      return false;
    }
  }
}
