using System;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal abstract class StatModel
    {
        public abstract bool Hidden { get; }

        public abstract string Label { get; }
        public abstract string Level { get; }
        public abstract Color Color { get; }
        public abstract Func<string> Tooltip { get; }

        protected Pawn Pawn { get; }

        protected StatModel(Pawn pawn) => Pawn = pawn;
    }
}
