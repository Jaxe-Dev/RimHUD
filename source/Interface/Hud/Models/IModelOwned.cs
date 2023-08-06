namespace RimHUD.Interface.Hud.Models
{
  public interface IModelOwned : IModelBase
  {
    PawnModel Owner { get; }
  }
}
