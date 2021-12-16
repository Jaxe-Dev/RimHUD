using System;
using System.Text;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class NeedModel : IBarModel
  {
    public PawnModel Model { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover { get; }
    public Action OnClick { get; }

    public float Max { get; }
    public float Value { get; }
    public HudBar.ValueStyle ValueStyle { get; }
    public float[] Thresholds { get; }

    public NeedModel(PawnModel model, NeedDef def)
    {
      Model = model;

      var need = model.Base.needs?.TryGetNeed(def);
      if (need == null)
      {
        Hidden = true;
        return;
      }

      Label = def.LabelCap;

      Max = 1f;
      Value = need.CurLevelPercentage;
      ValueStyle = HudBar.ValueStyle.Percentage;

      if (def == Access.NeedDefOfMood)
      {
        Tooltip = model.Mind.Tooltip;
        Thresholds = new[] { model.MoodThresholdMinor, model.MoodThresholdMajor, model.MoodThresholdExtreme };
      }
      else if (def == NeedDefOf.Food) { Tooltip = GetFoodTooltip(); }
      else if (def == NeedDefOf.Rest) { Tooltip = GetRestTooltip(); }
      else if (def == NeedDefOf.Joy) { Tooltip = GetJoyTooltip(); }

      OnClick = InspectPanePlus.ToggleNeedsTab;
    }

    private Func<string> GetFoodTooltip() => () =>
    {
      var builder = new StringBuilder();
      if (Model.Base.RaceProps?.foodType != null)
      {
        builder.AppendLine("Diet".Translate() + ": " + Model.Base.RaceProps.foodType.ToHumanString().CapitalizeFirst());
        builder.AppendLine();
      }
      HudModel.BuildStatString(this, builder, StatDefOf.EatingSpeed);
      HudModel.BuildStatString(this, builder, StatDefOf.HungerRateMultiplier);

      return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
    };

    private Func<string> GetRestTooltip() => () =>
    {
      var builder = new StringBuilder();
      HudModel.BuildStatString(this, builder, StatDefOf.RestRateMultiplier);

      return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
    };

    private Func<string> GetJoyTooltip() => () =>
    {
      var builder = new StringBuilder();
      if (Model.Base.needs?.beauty != null) { builder.AppendLine($"{Access.NeedDefOfBeauty.LabelCap}: {Model.Base.needs.beauty.CurLevelPercentage.ToStringPercent()}"); }
      if (Model.Base.needs?.comfort != null) { builder.AppendLine($"{Access.NeedDefOfComfort.LabelCap}: {Model.Base.needs.comfort.CurLevelPercentage.ToStringPercent()}"); }
      if (Model.Base.needs?.outdoors != null) { builder.AppendLine($"{Access.NeedDefOfOutdoors.LabelCap}: {Model.Base.needs.outdoors.CurLevelPercentage.ToStringPercent()}"); }

      return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
    };
  }
}
