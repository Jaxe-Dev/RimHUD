﻿using HarmonyLib;
using Verse;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(Game), "FinalizeInit")]
  internal static class Verse_Game_FinalizeInit
  {
    private static void Postfix() => LongEventHandler.ExecuteWhenFinished(Mod.OnEnteredGame);
  }
}
