using System;
using System.Linq;
using RimHUD.Compatibility.Multiplayer;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class FoodModel : IModelSelector
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color => null;
    public Func<string> Tooltip => null;

    public Action OnHover => null;
    public Action OnClick { get; }

    public FoodModel(PawnModel owner)
    {
      try
      {
        Owner = owner;

        if (!owner.IsPlayerManaged || owner.Base.foodRestriction == null || !owner.Base.foodRestriction.Configurable)
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.FoodFormat", owner.Base.foodRestriction.CurrentFoodRestriction?.label);

        OnClick = DrawFloatMenu;
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        Hidden = true;
      }
    }

    private void DrawFloatMenu()
    {
      try
      {
        var model = Owner;

        var options = (from food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions select new FloatMenuOption(food.label, () => Mod_Multiplayer.SetFoodRestriction(model.Base, food))).ToList();
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(model.Base.foodRestriction.CurrentFoodRestriction))));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }
    }
  }
}
