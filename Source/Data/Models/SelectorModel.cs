using System;
using UnityEngine;

namespace RimHUD.Data.Models
{
    internal abstract class SelectorModel : AttributeModel
    {
        public abstract Color? Color { get; }
        public abstract Action OnHover { get; }
        public override string Label { get; } = null;

        protected SelectorModel(PawnModel model) : base(model) { }
    }
}
