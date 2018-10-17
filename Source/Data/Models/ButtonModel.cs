using System;
using UnityEngine;

namespace RimHUD.Data.Models
{
    internal abstract class ButtonModel : AttributeModel
    {
        public abstract Texture2D Texture { get; }
        public abstract Action OnClick { get; }
        public abstract Action OnHover { get; }
        public override string Label { get; } = null;

        protected ButtonModel(PawnModel model) : base(model)
        { }
    }
}
