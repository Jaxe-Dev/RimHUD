using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class GenderRaceAndAgeValue : ValueModel
{
  protected override string Value { get; }

  protected override Func<string?> Tooltip { get; }

  protected override Action OnClick { get; }

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  public GenderRaceAndAgeValue()
  {
    Value = GetValue().Colorize(Active.FactionRelationColor);

    Tooltip = BioTooltip.Get;

    OnClick = InspectPaneTabs.ToggleBio;
  }

  private static string? GetRace()
  {
    if (Active.Pawn.IsMutant && Active.Pawn.mutant?.Def is not null) { return Active.Pawn.mutant.Def.label; }
    if (!ModsConfig.IdeologyActive || !Active.Pawn.IsHumanlike()) { return Active.Pawn.kindDef?.race?.label; }
    var race = Active.Pawn.Ideo?.memberName ?? Active.Pawn.kindDef?.race?.label;

    return race?.Trim();
  }

  private string GetValue()
  {
    var gender = Active.Pawn.gender is Gender.None ? null : Active.Pawn.GetGenderLabel();

    var genderRace = Lang.AdjectiveNoun(gender, GetRace());

    if (Active.Pawn.ageTracker is null) { return genderRace.Trim().CapitalizeFirst(); }

    Active.Pawn.ageTracker.AgeBiologicalTicks.TicksToPeriod(out var years, out var quadrums, out var days, out _);
    var ageDays = (quadrums * GenDate.DaysPerQuadrum) + days;

    var age = $"{years} {ageDays switch { 0 => Lang.Get("Model.Age.Birthday"), GenDate.DaysPerYear => Lang.Get("Model.Age.Birthday"), 1 => Lang.Get("Model.Age.Day"), _ => Lang.Get("Model.Age.Days", ageDays) }}";

    return Lang.Get("Model.GenderRaceAndAge", genderRace, age).Trim().CapitalizeFirst();
  }
}
