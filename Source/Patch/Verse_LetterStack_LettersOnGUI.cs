using Harmony;
using RimHUD.Interface;
using Verse;

namespace RimHUD.Patch
{
    [HarmonyPatch(typeof(LetterStack), "LettersOnGUI", typeof(float))]
    internal static class Verse_LetterStack_LettersOnGUI
    {
        private static bool Prefix(float baseY) => LetterStackPlus.DrawLetters(baseY);
    }
}
