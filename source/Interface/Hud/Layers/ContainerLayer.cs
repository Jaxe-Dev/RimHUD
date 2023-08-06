namespace RimHUD.Interface.Hud.Layers
{
  public abstract class ContainerLayer : BaseLayer
  {
    public abstract bool FillHeight { get; }

    public abstract void Flush();
  }
}
