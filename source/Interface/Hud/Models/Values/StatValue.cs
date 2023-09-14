using RimHUD.Extensions;
using RimWorld;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class StatValue : ValueModel
  {
    protected override string? Value { get; }

    public StatValue(StatDef def)
    {
      if (def.Worker?.IsDisabledFor(Active.Pawn) ?? true) { return; }

      Value = $"{def.GetDefNameOrLabel()}: {def.ValueToString(Active.Pawn.GetStatValue(def))}";
    }
  }
}
