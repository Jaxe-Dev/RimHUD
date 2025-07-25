using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class PrisonerInfoValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override Action OnClick { get; } = InspectPaneTabs.TogglePrisoner;

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    if (!Active.Pawn.IsPrisonerOfColony || Active.Pawn.guest is null) { return null; }

    var resistance = Active.Pawn.guest.Recruitable ? "RecruitmentResistance".TranslateSimple().WithValue(Active.Pawn.guest.resistance.ToString("F1").Bold()) : (Active.Pawn.Faction is null ? "UnrecruitableNoFaction" : "Unrecruitable").TranslateSimple().CapitalizeFirst().Bold();
    var will = "WillLevel".TranslateSimple().WithValue(Active.Pawn.guest.will.ToString("F1").Bold());

    return ModsConfig.IdeologyActive ? $"{resistance} / {will}" : resistance;
  }
}
