using UnityEngine;

namespace RimHUD.Data.Models
{
    internal abstract class ValueModel : AttributeModel
    {
        public abstract string Value { get; }
        public abstract Color? Color { get; }

        protected ValueModel(PawnModel model) : base(model) { }
    }
}
