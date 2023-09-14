using System;

namespace RimHUD.Interface.Hud.Layers
{
  [Flags]
  public enum LayerTarget
  {
    PlayerHumanlike = 1,
    PlayerCreature = 2,
    OtherHumanlike = 4,
    OtherCreature = 8,
    All = PlayerHumanlike | PlayerCreature | OtherHumanlike | OtherCreature
  }
}
