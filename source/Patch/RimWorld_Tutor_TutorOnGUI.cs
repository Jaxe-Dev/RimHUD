using HarmonyLib;
using RimHUD.Engine;
using RimWorld;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(Tutor), "TutorOnGUI")]
  public static class RimWorld_Tutor_TutorOnGUI
  {
    private static bool Prefix() => !State.HudFloatingVisible;
  }
}
