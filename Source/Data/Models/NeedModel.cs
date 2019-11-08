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
            else
            {
                Tooltip = null;
                Thresholds = null;
            }

            OnClick = InspectPanePlus.ToggleNeedsTab;
        }
    }
}
