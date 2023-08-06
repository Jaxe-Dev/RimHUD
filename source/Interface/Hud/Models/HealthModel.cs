using System;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class HealthModel : IModelBar
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover => null;
    public Action OnClick { get; }

    public float Max { get; }
    public float Value { get; }
    public float[] Thresholds => null;

    public HealthModel(PawnModel owner)
    {
      Owner = owner;

      if (owner.Base.health == null)
      {
        Hidden = true;
        return;
      }

      Label = "Health".Translate();

      Max = 1f;
      Value = owner.Base.health?.summaryHealth?.SummaryHealthPercent ?? -1f;

      Tooltip = owner.Health.ConditionTooltip;

      OnClick = InspectPanePlus.ToggleHealthTab;
    }
  }
}
