using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class AreaModel : ISelectorModel
  {
    public PawnModel Model { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }

    public Func<string> Tooltip { get; }

    public Action OnHover { get; }
    public Action OnClick { get; }

    public AreaModel(PawnModel model)
    {
      try
      {
        Model = model;

        if ((!model.Base.Faction?.IsPlayer ?? true) || model.Base.playerSettings == null || (!model.Base.IsColonist && !model.Base.playerSettings.RespectsAllowedArea))
        {
          Hidden = true;
          return;
        }

        Label = Lang.Get("Model.Selector.AreaFormat", AreaUtility.AreaAllowedLabel(model.Base));
        Color = model.Base.playerSettings?.EffectiveAreaRestriction?.Color;

        OnClick = DrawFloatMenu;

        OnHover = () => model.Base.playerSettings.EffectiveAreaRestriction?.MarkForDraw();
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
        var model = Model;

        var options = new List<FloatMenuOption> { new FloatMenuOption("NoAreaAllowed".Translate(), () => Mod_Multiplayer.SetArea(model.Base, null)) };
        options.AddRange(from area in Find.CurrentMap.areaManager.AllAreas.Where(area => area.AssignableAsAllowed()) select new FloatMenuOption(area.Label, () => Mod_Multiplayer.SetArea(model.Base, area)));
        options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), () => Find.WindowStack.Add(new Dialog_ManageAreas(Find.CurrentMap))));

        Find.WindowStack.Add(new FloatMenu(options));
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }
    }
  }
}
