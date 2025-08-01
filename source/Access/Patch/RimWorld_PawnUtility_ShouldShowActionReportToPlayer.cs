using HarmonyLib;
using RimHUD.Engine;
using RimWorld;
using Verse;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(PawnUtility), "ShouldShowActionReportToPlayer")]
public static class RimWorld_PawnUtility_ShouldShowActionReportToPlayer
{
  private static bool Prefix(Pawn p, ref bool __result)
  {
    if (!State.Activated) { return true; }

    __result = !p.InMentalState && (!p.IsMutant || !p.mutant!.Def!.overrideInspectString);

    return false;
  }
}
