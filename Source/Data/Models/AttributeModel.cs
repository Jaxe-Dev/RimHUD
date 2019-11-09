using System;
using System.Text;
using RimWorld;
using Verse;

namespace RimHUD.Data.Models
{
    internal abstract class AttributeModel
    {
        public abstract bool Hidden { get; }

        public abstract string Label { get; }
        public abstract TipSignal? Tooltip { get; }
        public Action OnClick { get; protected set; }

        protected PawnModel Model { get; }

        protected AttributeModel(PawnModel model) => Model = model;

        protected void BuildStatString(StringBuilder builder, StatDef def)
        {
            if (def.Worker.IsDisabledFor(Model.Base)) { return; }
            try { builder.AppendLine($"{def.LabelCap}: {def.ValueToString(Model.Base.GetStatValue(def))}"); }
            catch { }
        }
    }
}
