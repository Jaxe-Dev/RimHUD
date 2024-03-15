using System;
using System.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Integration.Multiplayer;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Selectors
{
  public sealed class FoodSelector : SelectorModel
  {
    protected override string? Label { get; }

    protected override Action? OnClick { get; }

    public FoodSelector()
    {
      if (!Active.Pawn.IsPlayerManaged() || Active.Pawn.foodRestriction?.CurrentFoodPolicy is null || !Active.Pawn.foodRestriction.Configurable) { return; }

      Label = Lang.Get("Model.Selector.FoodFormat", Active.Pawn.foodRestriction.CurrentFoodPolicy.label);

      OnClick = DrawFloatMenu;
    }

    private static void DrawFloatMenu()
    {
      var options = (from food in Current.Game!.foodRestrictionDatabase!.AllFoodRestrictions select new FloatMenuOption(food.label, () => Mod_Multiplayer.SetFoodRestriction(Active.Pawn, food))).ToList();
      options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), static () => Find.WindowStack!.Add(new Dialog_ManageFoodPolicies(Active.Pawn.foodRestriction!.CurrentFoodPolicy))));

      Find.WindowStack!.Add(new FloatMenu(options));
    }
  }
}
