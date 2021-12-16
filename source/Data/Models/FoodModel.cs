using System;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class FoodModel : ISelectorModel
  {
    public PawnModel Model { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover { get; }
    public Action OnClick { get; }

    public FoodModel(PawnModel model)
    {
      try
      {
        Model = model;

        if (!model.IsPlayerManaged || model.Base.foodRestriction == null || !model.Base.foodRestriction.Configurable)
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.FoodFormat", model.Base.foodRestriction.CurrentFoodRestriction?.label);

        OnClick = DrawFloatMenu;
      }
      catch (Exception exception)
      {
        Mod.HandleWarning(exception);
        Hidden = true;
      }
    }

    private void DrawFloatMenu()
    {
      try
      {
        var model = Model;

        var options = (from food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions select new FloatMenuOption(food.label, () => Mod_Multiplayer.SetFoodRestriction(model.Base, food))).ToList();
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(model.Base.foodRestriction.CurrentFoodRestriction))));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Mod.HandleError(exception); }
    }
  }
}
