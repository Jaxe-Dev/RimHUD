using System.Text;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using RimWorld;
using Verse;

namespace RimHUD.Data.Models
{
    internal class NeedModel : BarModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override float Max { get; }
        public override float Value { get; }
        public override HudBar.ValueStyle ValueStyle { get; }
        public override float[] Thresholds { get; }
        public override TipSignal? Tooltip { get; }

        public NeedModel(PawnModel model, NeedDef def) : base(model)
        {
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
                if (Model.Base.RaceProps?.foodType != null)
                {
                    builder.AppendLine("Diet".Translate() + ": " + Model.Base.RaceProps.foodType.ToHumanString().CapitalizeFirst());
                    builder.AppendLine();
                }
                BuildStatString(builder, StatDefOf.EatingSpeed);
                BuildStatString(builder, StatDefOf.HungerRateMultiplier);

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            else if (def == NeedDefOf.Rest)
            {
                var builder = new StringBuilder();
                BuildStatString(builder, StatDefOf.RestRateMultiplier);

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            else if (def == NeedDefOf.Joy)
            {
                var builder = new StringBuilder();
                if (Model.Base.needs?.beauty != null) { builder.AppendLine($"{Access.NeedDefOfBeauty.LabelCap}: {Model.Base.needs.beauty.CurLevelPercentage.ToStringPercent()}"); }
                if (Model.Base.needs?.comfort != null) { builder.AppendLine($"{Access.NeedDefOfComfort.LabelCap}: {Model.Base.needs.comfort.CurLevelPercentage.ToStringPercent()}"); }
                if (Model.Base.needs?.outdoors != null) { builder.AppendLine($"{Access.NeedDefOfOutdoors.LabelCap}: {Model.Base.needs.outdoors.CurLevelPercentage.ToStringPercent()}"); }

                Tooltip = builder.Length == 0 ? null : new TipSignal(() => builder.ToStringTrimmed().Size(Theme.Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId);
            }
            else
            {
                Tooltip = null;
                Thresholds = null;
            }

            OnClick = InspectPanePlus.ToggleNeedsTab;
        }
    }
}
