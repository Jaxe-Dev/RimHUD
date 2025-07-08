using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class EquippedValue : ValueModel
{
  protected override string Value { get; } = GetValue();

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleGear;

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string GetValue()
  {
    if (RestraintsUtility.ShouldShowRestraintsInfo(Active.Pawn)) { return "InRestraints".TranslateSimple(); }

    var equipped = Active.Pawn.equipment?.Primary?.LabelCap;
    return equipped is null ? string.Empty : Lang.Get("Model.Info.Equipped", equipped.Bold());
  }
}
