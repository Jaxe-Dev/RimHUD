using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class RelationKindAndFactionValue : ValueModel
  {
    protected override string Value { get; }

    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public RelationKindAndFactionValue()
    {
      Value = GetValue();

      Tooltip = BioTooltip.Get;

      OnClick = InspectPaneTabs.ToggleBio;
    }

    private static string GetValue() => Lang.Get("Model.RelationKindAndFaction", Lang.AdjectiveNoun(GetRelation(), GetKind()), GetFaction()).Trim().CapitalizeFirst().Colorize(Active.FactionRelationColor);

    private static string? GetRelation()
    {
      if (Active.Pawn.IsPrisoner) { return Lang.Get("Model.Faction.Prisoner"); }
      if (Active.Pawn.IsSlave) { return Active.Pawn.story?.title is null ? Lang.Get("Model.Faction.Slave") : null; }
      if (Active.Pawn.Faction is null) { return Active.Pawn.IsHumanlike() ? Lang.Get("Model.Faction.Independent") : Active.Pawn.kindDef == PawnKindDefOf.WildMan ? null : Lang.Get("Model.Faction.Wild"); }
      if (Active.Pawn.Faction.IsPlayer) { return null; }

      return Active.Pawn.Faction.PlayerRelationKind switch
      {
        FactionRelationKind.Hostile => Active.Pawn.RaceProps!.IsMechanoid ? Lang.Get("Model.Faction.Hostile") : Lang.Get("Model.Faction.Enemy"),
        FactionRelationKind.Ally => Lang.Get("Model.Faction.Allied"),
        _ => null
      };
    }

    private static string? GetFaction()
    {
      if (Active.Pawn.Faction is null || !Active.Pawn.Faction.HasName || (ModsConfig.AnomalyActive && Active.Pawn.Faction == Faction.OfEntities)) { return null; }

      if (Active.Pawn.IsPrisoner || Active.Pawn.IsSlave) { return Active.Pawn.HostFaction is null || Active.Pawn.HostFaction.HasName || Active.Pawn.HostFaction == Faction.OfPlayer ? null : Lang.Get("Model.OfFaction", Active.Pawn.HostFaction.Name); }

      return Lang.Get("Model.OfFaction", Active.Pawn.Faction.Name);
    }

    private static string GetKind()
    {
      if (Active.Pawn.IsHumanlike()) { return Active.Pawn.Faction == Faction.OfPlayer && !Active.Pawn.IsPrisoner ? Active.Pawn.story?.Title ?? Active.Pawn.KindLabel : Active.Pawn.TraderKind?.label ?? Active.Pawn.KindLabel; }

      if (Active.Pawn.RaceProps!.IsMechanoid) { return Active.Pawn.Faction == Faction.OfPlayer ? Lang.Get("Model.Creature.Mechanoid") : Lang.Get("Model.Creature.Unit"); }

      if (Active.Pawn.Faction is null)
      {
        if (Active.Pawn.RaceProps.petness > 0.5f) { return Lang.Get("Model.Creature.Stray"); }
        if (Active.Pawn.RaceProps.predator) { return Lang.Get("Model.Creature.Predator"); }
        if (Active.Pawn.kindDef!.race!.tradeTags?.Contains("AnimalInsect") ?? false) { return Lang.Get("Model.Creature.Insect"); }
        return Lang.Get("Model.Creature.Wild");
      }

      if (Active.Pawn.Faction == Faction.OfEntities) { return Lang.Get("Model.Creature.Entity"); }

      switch (Active.Pawn.RaceProps.petness)
      {
        case > 0.5f: return Lang.Get("Model.Creature.Pet");
        case > 0f: return Lang.Get("Model.Creature.ExoticPet");
      }

      if (Active.Pawn.RaceProps.predator) { return Lang.Get("Model.Creature.Hunt"); }
      if (Active.Pawn.RaceProps.packAnimal) { return Lang.Get("Model.Creature.Pack"); }
      if (Active.Pawn.kindDef!.race!.tradeTags?.Contains("AnimalFarm") ?? false) { return Lang.Get("Model.Creature.Farm"); }
      if (Active.Pawn.RaceProps.herdAnimal) { return Lang.Get("Model.Creature.Herd"); }
      if (Active.Pawn.kindDef.race.tradeTags?.Contains("AnimalInsect") ?? false) { return Lang.Get("Model.Creature.Insect"); }
      return Lang.Get(Active.Pawn.RaceProps.Animal ? "Model.Creature.Tame" : "Model.Creature.Entity");
    }
  }
}
