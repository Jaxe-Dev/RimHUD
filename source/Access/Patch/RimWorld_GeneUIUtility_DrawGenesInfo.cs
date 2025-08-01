using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace RimHUD.Access.Patch;

[HarmonyPatch(typeof(GeneUIUtility), nameof(GeneUIUtility.DrawGenesInfo))]
public static class RimWorld_GeneUIUtility_DrawGenesInfo
{
  [HarmonyTranspiler]
  public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    var codes = new List<CodeInstruction>(instructions);

    for (var i = 0; i < codes.Count; i++)
    {
      if (codes[i]!.opcode == OpCodes.Ldc_R4 && codes[i]!.operand is float operand && Mathf.Approximately(operand, 165f)) { codes[i] = new CodeInstruction(OpCodes.Call, Reflection.Theme_InspectPaneHeight_Value); }
    }

    return codes;
  }
}
