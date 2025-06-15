using RimHUD.Engine;
using UnityEngine;

namespace RimHUD.Interface.Dialog.Tabs;

public sealed class Tab_ConfigCredits : Tab
{
  public override bool Show => Credits.Exists;

  public override string Label => "Credits";

  public override void Draw(Rect rect) => Credits.Draw(rect);
}
