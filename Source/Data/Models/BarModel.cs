using RimHUD.Interface.HUD;

namespace RimHUD.Data.Models
{
    internal abstract class BarModel : AttributeModel
    {
        public abstract float Max { get; }
        public abstract float Value { get; }
        public abstract HudBar.ValueStyle ValueStyle { get; }
        public abstract float[] Thresholds { get; }

        protected BarModel(PawnModel model) : base(model) { }
    }
}
