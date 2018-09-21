using UnityEngine;

namespace RimHUD.Interface
{
    internal abstract class Tab
    {
        public abstract string Label { get; }
        public abstract string Tooltip { get; }

        public bool Enabled { get; set; } = true;

        public abstract void Draw(Rect rect);
    }
}
