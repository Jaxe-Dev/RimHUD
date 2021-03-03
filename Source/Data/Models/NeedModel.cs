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
    internal struct NeedModel : IBarModel
    {
        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public Color? Color { get; }
        public TipSignal? Tooltip { get; }

        public Action OnHover { get; }
        public Action OnClick { get; }

        public float Max { get; }
        public float Value { get; }
        public HudBar.ValueStyle ValueStyle { get; }
        public float[] Thresholds { get; }

        public NeedModel(PawnModel model, NeedDef def) : this()
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
            else if (def == NeedDefOf.Food)
            {
                var builder = new StringBuilder();
                if (model.Base.RaceProps?.foodType != null)
                {
                    builder.AppendLine("Diet".Translate() + ": " + model.Base.RaceProps.foodType.ToHumanString().CapitalizeFirst());
                    builder.AppendLine();
                }
                HudModel.BuildStatString(this, builder, StatDefOf.EatingSpeed);
                HudModel.BuildStatString(this, builder, StatDefOf.HungerRateMultiplier);

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            else if (def == NeedDefOf.Rest)
            {
                var builder = new StringBuilder();
                HudModel.BuildStatString(this, builder, StatDefOf.RestRateMultiplier);

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            else if (def == NeedDefOf.Joy)
            {
                var builder = new StringBuilder();
                if (model.Base.needs?.beauty != null) { builder.AppendLine($"{Access.NeedDefOfBeauty.LabelCap}: {model.Base.needs.beauty.CurLevelPercentage.ToStringPercent()}"); }
                if (model.Base.needs?.comfort != null) { builder.AppendLine($"{Access.NeedDefOfComfort.LabelCap}: {model.Base.needs.comfort.CurLevelPercentage.ToStringPercent()}"); }
                if (model.Base.needs?.outdoors != null) { builder.AppendLine($"{Access.NeedDefOfOutdoors.LabelCap}: {model.Base.needs.outdoors.CurLevelPercentage.ToStringPercent()}"); }

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            //else
            //{
            //    Tooltip = null;
            //    Thresholds = null;
            //}

            OnClick = InspectPanePlus.ToggleNeedsTab;
        }
    }
}
