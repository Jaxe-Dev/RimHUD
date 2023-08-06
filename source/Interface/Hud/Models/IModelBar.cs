namespace RimHUD.Interface.Hud.Models
{
  public interface IModelBar : IModelOwned
  {
    float Max { get; }
    float Value { get; }
    float[] Thresholds { get; }
  }
}
