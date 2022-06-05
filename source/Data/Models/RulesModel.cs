using System;
using RimHUD.Data.Integration;
using UnityEngine;

namespace RimHUD.Data.Models
{
  internal class RulesModel : ISelectorModel
  {
    public PawnModel Model { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover { get; }
    public Action OnClick { get; }

    public RulesModel(PawnModel model)
    {
      try
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
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        Hidden = true;
      }
    }
  }
}
