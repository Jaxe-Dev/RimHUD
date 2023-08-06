using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Compatibility.Multiplayer;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class AreaModel : IModelSelector
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }

    public Func<string> Tooltip => null;

    public Action OnHover { get; }
    public Action OnClick { get; }

    public AreaModel(PawnModel owner)
    {
      try
      {
        Owner = owner;

        if ((!owner.Base.Faction?.IsPlayer ?? true) || owner.Base.playerSettings == null || (!owner.Base.IsColonist && !owner.Base.playerSettings.RespectsAllowedArea))
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.AreaFormat", AreaUtility.AreaAllowedLabel(owner.Base));
        Color = owner.Base.playerSettings?.EffectiveAreaRestriction?.Color;

        OnClick = DrawFloatMenu;

        OnHover = () => owner.Base.playerSettings.EffectiveAreaRestriction?.MarkForDraw();
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

        var options = new List<FloatMenuOption> { new FloatMenuOption("NoAreaAllowed".Translate(), () => Mod_Multiplayer.SetArea(model.Base, null)) };
        options.AddRange(from area in Find.CurrentMap.areaManager.AllAreas.Where(area => area.AssignableAsAllowed()) select new FloatMenuOption(area.Label, () => Mod_Multiplayer.SetArea(model.Base, area)));
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageAreas(Find.CurrentMap))));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }
    }
  }
}
