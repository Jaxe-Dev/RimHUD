using HarmonyLib;
using RimHUD.Engine;
using Verse.Profile;

namespace RimHUD.Access.Patch
{
  [HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
  public static class Verse_Profile_MemoryUtility_ClearAllMapsAndWorld
  {
    private static void Prefix() => State.ClearCache();
  }
}
