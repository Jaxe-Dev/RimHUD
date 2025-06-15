using HarmonyLib;
using RimHUD.Configuration;
using Verse;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(Game), "FinalizeInit")]
public static class Verse_Game_FinalizeInit
{
  private static void Postfix() => LongEventHandler.ExecuteWhenFinished(Persistent.ReportIfReset);
}
