namespace RimHUD.Interface.HUD
{
    internal abstract class HudContainer : HudComponent
    {
        //protected abstract string ElementName { get; }

        public abstract bool FillHeight { get; }

        public abstract void Flush();
    }
}
