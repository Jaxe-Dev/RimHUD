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
  public class OutfitModel : IModelSelector
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Func<string> Tooltip => null;
    public Color? Color => null;

    public Action OnHover => null;
    public Action OnClick { get; }

    public OutfitModel(PawnModel owner)
    {
      try
      {
        Owner = owner;

        if (!owner.IsPlayerFaction || owner.Base?.outfits?.CurrentOutfit == null)
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.OutfitFormat", owner.Base.outfits?.CurrentOutfit.label);

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

        var options = (from outfit in Current.Game.outfitDatabase.AllOutfits select new FloatMenuOption(outfit.label, () => Mod_Multiplayer.SetOutfit(model.Base, outfit))).ToList();
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageOutfits(model.Base.outfits.CurrentOutfit))));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }
    }
  }
}
