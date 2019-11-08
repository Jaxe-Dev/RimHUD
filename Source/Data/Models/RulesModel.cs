using System;
using RimHUD.Data.Integration;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class RulesModel : SelectorModel
    {
        public override bool Hidden { get; }
        public override string Label { get; }
        public override TipSignal? Tooltip { get; }
        public override Color? Color { get; }
        public override Action OnHover { get; }

        public RulesModel(PawnModel model) : base(model)
        {
            if (!PawnRules.Instance.IsActive || !PawnRules.CanHaveRules(model.Base))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Integration.PawnRules.RuleNameFormat", PawnRules.GetRules(model.Base));
            Tooltip = null;
            Color = null;

            OnClick = () => PawnRules.OpenRules(model.Base);
            OnHover = null;
        }
    }
}
