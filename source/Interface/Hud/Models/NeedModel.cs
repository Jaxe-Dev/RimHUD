using System;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Patch;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class NeedModel : IModelBar
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover => null;
    public Action OnClick { get; }

    public float Max { get; }
    public float Value { get; }
    public float[] Thresholds { get; }

    public NeedModel(PawnModel owner, NeedDef def)
    {
      Owner = owner;

      var need = owner.Base.needs?.TryGetNeed(def);
      if (need == null)
      {
        Hidden = true;
        return;
      }

      Label = def.GetLabelCap();

      Max = 1f;
      Value = need.CurLevelPercentage;

      if (def == Access.NeedDefOfMood)
      {
        Tooltip = owner.Mind.Tooltip;
        Thresholds = new[] { owner.MoodThresholdMinor, owner.MoodThresholdMajor, owner.MoodThresholdExtreme };
      }
      else if (def == NeedDefOf.Food) { Tooltip = GetFoodTooltip; }
      else if (def == NeedDefOf.Rest) { Tooltip = GetRestTooltip; }
      else if (def == Access.NeedDefOfEnergy) { Tooltip = GetEnergyTooltip; }
      else if (def == NeedDefOf.Joy) { Tooltip = GetJoyTooltip; }

      if (def == Access.NeedDefOfSuppression)
      {
        Tooltip = GetSuppressionTooltip;
        OnClick = InspectPanePlus.ToggleSlaveTab;
      }
      else { OnClick = InspectPanePlus.ToggleNeedsTab; }
    }

    private string GetFoodTooltip()
    {
      var builder = new StringBuilder();
      if (Owner.Base.RaceProps?.foodType != null)
      {
        builder.AppendLine("Diet".Translate() + ": " + Owner.Base.RaceProps.foodType.ToHumanString().CapitalizeFirst());
        builder.AppendLine();
      }

      HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.EatingSpeed);
      HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.BedHungerRateFactor);

      return builder.ToTooltip();
    }

    private string GetRestTooltip()
    {
      var builder = new StringBuilder();
      HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.RestRateMultiplier);

      return builder.ToTooltip();
    }

    private string GetEnergyTooltip()
    {
      if (Owner.Base.needs?.energy?.FallPerDay == null) { return null; }

      var builder = new StringBuilder();
      builder.AppendInNewLine("CurrentMechEnergyFallPerDay".Translate() + ": " + (Owner.Base.needs.energy.FallPerDay / 100f).ToStringPercent());

      return builder.ToTooltip();
    }

    private string GetJoyTooltip()
    {
      var builder = new StringBuilder();
      if (Owner.Base.needs?.beauty != null) { builder.AppendLine($"{Access.NeedDefOfBeauty.LabelCap}: {Owner.Base.needs.beauty.CurLevelPercentage.ToStringPercent()}"); }
      if (Owner.Base.needs?.comfort != null) { builder.AppendLine($"{Access.NeedDefOfComfort.LabelCap}: {Owner.Base.needs.comfort.CurLevelPercentage.ToStringPercent()}"); }
      if (Owner.Base.needs?.outdoors != null) { builder.AppendLine($"{Access.NeedDefOfOutdoors.LabelCap}: {Owner.Base.needs.outdoors.CurLevelPercentage.ToStringPercent()}"); }

      return builder.ToTooltip();
    }

    private string GetSuppressionTooltip()
    {
      var builder = new StringBuilder();

      HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.SlaveSuppressionFallRate);
      HudBuilder.BuildStatString(Owner.Base, builder, StatDefOf.Terror);

      return builder.ToTooltip();
    }
  }
}
