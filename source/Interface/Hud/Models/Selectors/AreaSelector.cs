using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Integration.Multiplayer;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models.Selectors;

public sealed class AreaSelector : SelectorModel
{
  protected override string? Label { get; }

  protected override Action? OnHover { get; }
  protected override Action? OnClick { get; }

  protected override Color? Color { get; }

  public AreaSelector()
  {
    if ((!Active.Pawn.Faction?.IsPlayer ?? true) || Active.Pawn.playerSettings is null || (!Active.Pawn.IsColonist && !Active.Pawn.playerSettings.RespectsAllowedArea)) { return; }

    Label = Lang.Get("Model.Selector.AreaFormat", AreaUtility.AreaAllowedLabel(Active.Pawn));
    Color = Active.Pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap?.Color;

    OnHover = static () => Active.Pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap?.MarkForDraw();
    OnClick = DrawFloatMenu;
  }

  private static void DrawFloatMenu()
  {
    var options = new List<FloatMenuOption> { new("NoAreaAllowed".Translate(), static () => Mod_Multiplayer.SetArea(Active.Pawn, null)) };
    options.AddRange(from area in Find.CurrentMap!.areaManager!.AllAreas.Where(static area => area.AssignableAsAllowed()) select new FloatMenuOption(area.Label, () => Mod_Multiplayer.SetArea(Active.Pawn, area)));
    options.Add(new FloatMenuOption(Lang.Get("Model.Selector.Manage").Italic(), static () => Find.WindowStack!.Add(new Dialog_ManageAreas(Find.CurrentMap))));

    Find.WindowStack!.Add(new FloatMenu(options));
  }
}
