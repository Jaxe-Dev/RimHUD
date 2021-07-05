using System;
using RimHUD.Data.Integration;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal struct RulesModel : ISelectorModel
    {
        public PawnModel Model { get; }
        public bool Hidden { get; }

        public string Label { get; }
        public Color? Color { get; }
        public TipSignal? Tooltip { get; }

        public Action OnHover { get; }
        public Action OnClick { get; }

        public RulesModel(PawnModel model) : this()
        {
            Model = model;

            if (!Mod_PawnRules.Instance.IsActive || !Mod_PawnRules.CanHaveRules(model.Base))
            {
                Hidden = true;
                return;
            }

            Label = Lang.Get("Integration.PawnRules.RuleNameFormat", Mod_PawnRules.GetRules(model.Base));

            OnClick = () => Mod_PawnRules.OpenRules(model.Base);
        }
    }
}
