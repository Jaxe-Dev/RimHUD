namespace RimHUD.Interface.HUD
{
  internal abstract class HudContainer : HudComponent
  {
    public abstract bool FillHeight { get; }

    public abstract void Flush();
  }
}
