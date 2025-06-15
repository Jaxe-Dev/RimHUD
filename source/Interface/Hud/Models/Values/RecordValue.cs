using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class RecordValue(RecordDef def) : ValueModel
{
  protected override string Value { get; } = $"{def.LabelCap}: {(def.type is RecordType.Time ? Active.Pawn.records!.GetAsInt(def).ToStringTicksToPeriod() : Active.Pawn.records!.GetValue(def).ToString("0.##"))}";
}
