using UnityEngine;

namespace RimHUD.Interface.Dialog.Tabs;

public abstract class Tab
{
  public abstract string Label { get; }

  public abstract void Draw(Rect rect);

  public virtual void Reset()
  { }
}
