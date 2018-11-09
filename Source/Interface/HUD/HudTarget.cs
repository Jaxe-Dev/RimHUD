using System;

namespace RimHUD.Interface.HUD
{
    [Flags]
    internal enum HudTarget
    {
        Invalid = 1 << 0,
        PlayerHumanlike = 1 << 1,
        PlayerCreature = 1 << 2,
        OtherHumanlike = 1 << 3,
        OtherCreature = 1 << 4,
        All = PlayerHumanlike | PlayerCreature | OtherHumanlike | OtherCreature
    }
}
