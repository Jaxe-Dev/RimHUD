namespace RimHUD.Interface.HUD
{
    internal class HudStatValue : HudStat
    {
        public string Getter { get; private set; }
    }

    internal class HudStat
    {
        public string Label { get; private set; }
    }

    internal class HudStatBar : HudStat
    {
        public string Getter { get; private set; }
    }

    internal class HudStatDescription : HudStat
    {
        public string Getter { get; private set; }
    }
}
