using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal abstract class Tab
  {
    public abstract string Label { get; }
    public abstract TipSignal? Tooltip { get; }

    public bool Enabled { get; set; } = true;

    public abstract void Reset();
    public abstract void Draw(Rect rect);
  }
}
