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
            if (!Mod_PawnRules.Instance.IsActive || !Mod_PawnRules.CanHaveRules(model.Base))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Integration.PawnRules.RuleNameFormat", Mod_PawnRules.GetRules(model.Base));
            Tooltip = null;
            Color = null;

            OnClick = () => Mod_PawnRules.OpenRules(model.Base);
            OnHover = null;
        }
    }
}
