using RimHUD.Interface.HUD;
using Verse;

namespace RimHUD.Data.Models
{
    internal class HealthBarModel : BarModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override float Max { get; }
        public override float Value { get; }
        public override HudBar.ValueStyle ValueStyle { get; }
        public override float[] Thresholds { get; }
        public override TipSignal? Tooltip { get; }

        public HealthBarModel(PawnModel model) : base(model)
        {
            if (model.Base.health == null)
            {
                Hidden = true;
                return;
            }

            Label = "Health".Translate();

            Max = 1f;
            Value = model.Health.Percent;
            ValueStyle = HudBar.ValueStyle.Percentage;

            Tooltip = model.Health.Tooltip;
            Thresholds = null;
        }
    }
}
