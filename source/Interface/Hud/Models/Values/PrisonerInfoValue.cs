using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class PrisonerInfoValue : ValueModel
  {
    protected override string? Value { get; }

    protected override Action OnClick { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public PrisonerInfoValue()
    {
      Value = GetValue();

      OnClick = InspectPaneTabs.TogglePrisoner;
    }

    private static string? GetValue()
    {
      if (!Active.Pawn.IsPrisonerOfColony || Active.Pawn.guest is null) { return null; }

      var resistance = "RecruitmentResistance".Translate().WithValue(Active.Pawn.guest.resistance.ToString("F1").Bold());
      var will = "WillLevel".Translate().WithValue(Active.Pawn.guest.will.ToString("F1").Bold());

      return ModsConfig.IdeologyActive ? $"{resistance} / {will}" : resistance;
    }
  }
}
