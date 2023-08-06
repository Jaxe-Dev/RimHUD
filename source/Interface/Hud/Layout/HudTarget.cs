using System;

namespace RimHUD.Interface.Hud.Layout
{
  [Flags]
  public enum HudTarget
  {
    None = 0,
    PlayerHumanlike = 1,
    PlayerCreature = 2,
    OtherHumanlike = 4,
    OtherCreature = 8,
    All = PlayerHumanlike | PlayerCreature | OtherHumanlike | OtherCreature
  }
}
