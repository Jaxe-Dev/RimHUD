using HarmonyLib;
using Verse.Profile;

namespace RimHUD.Patch
{
  [HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
  public static class Verse_Profile_MemoryUtility_ClearAllMapsAndWorld
  {
    private static void Prefix() => Mod.ClearCache();
  }
}
