using System;
using UnityEngine;

namespace RimHUD.Interface.Dialog
{
    internal abstract class Tab
    {
        public abstract string Label { get; }
        public abstract Func<string> Tooltip { get; }

        public bool Enabled { get; set; } = true;

        public abstract void Draw(Rect rect);
    }
}
