using RimHUD.Interface.HUD;

namespace RimHUD.Data.Models
{
  internal interface IBarModel : IAttributeModel
  {
    float Max { get; }
    float Value { get; }
    HudBar.ValueStyle ValueStyle { get; }
    float[] Thresholds { get; }
  }
}
