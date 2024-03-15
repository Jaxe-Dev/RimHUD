using System;
using System.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Integration.Multiplayer;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Selectors
{
  public sealed class OutfitSelector : SelectorModel
  {
    protected override string? Label { get; }

    protected override Action? OnClick { get; }

    public OutfitSelector()
    {
      if (!Active.Pawn.IsPlayerFaction() || Active.Pawn.outfits?.CurrentApparelPolicy is null) { return; }

      Label = Lang.Get("Model.Selector.OutfitFormat", Active.Pawn.outfits?.CurrentApparelPolicy.label!);

      OnClick = DrawFloatMenu;
    }

    private static void DrawFloatMenu()
    {
      try
      {
        var options = (from outfit in Current.Game!.outfitDatabase!.AllOutfits select new FloatMenuOption(outfit.label, () => Mod_Multiplayer.SetOutfit(Active.Pawn, outfit))).ToList();
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), static () => Find.WindowStack!.Add(new Dialog_ManageApparelPolicies(Active.Pawn.outfits!.CurrentApparelPolicy))));

        Find.WindowStack!.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Report.HandleError(exception); }
    }
  }
}
