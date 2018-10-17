using System;

namespace RimHUD.Interface.HUD
{
    [Flags]
    internal enum HudTarget
    {
        Invalid = 1 << 0,
        PlayerColonist = 1 << 1,
        PlayerAnimal = 1 << 2,
        OtherColonist = 1 << 3,
        OtherAnimal = 1 << 4,
        All = PlayerColonist | PlayerAnimal | OtherColonist | OtherAnimal
    }
}
