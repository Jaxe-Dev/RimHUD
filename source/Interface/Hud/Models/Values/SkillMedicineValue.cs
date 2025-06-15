using System;
using RimHUD.Extensions;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class SkillMedicineValue : SkillValue
{
  private static readonly StatDef MedicalOperationSpeed = DefDatabase<StatDef>.GetNamed("MedicalOperationSpeed");

  protected override Func<string?> Tooltip { get; }

  public SkillMedicineValue() : base(SkillDefOf.Medicine) => Tooltip = GetTooltip;

  private string? GetTooltip()
  {
    var builder = PrepareBuilder();

    builder.AppendStatLine(MedicalOperationSpeed);
    builder.AppendStatLine(StatDefOf.MedicalSurgerySuccessChance);
    builder.AppendStatLine(StatDefOf.MedicalTendSpeed);
    builder.AppendStatLine(StatDefOf.MedicalTendQuality);

    return builder.ToStringTrimmedOrNull();
  }
}
