using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class RecordValue : ValueModel
{
  protected override string? Value { get; }

  public RecordValue(RecordDef def)
  {
    if (Active.Pawn.records is null) { return; }
    Value = $"{def.LabelCap}: {(def.type is RecordType.Time ? Active.Pawn.records.GetAsInt(def).ToStringTicksToPeriod() : Active.Pawn.records!.GetValue(def).ToString("0.##"))}";
  }
}
