using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class EquippedValue : ValueModel
  {
    protected override string Value { get; }

    protected override Action OnClick { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public EquippedValue()
    {
      Value = GetValue();

      OnClick = InspectPaneTabs.ToggleGear;
    }

    private static string GetValue()
    {
      if (RestraintsUtility.ShouldShowRestraintsInfo(Active.Pawn)) { return "InRestraints".Translate(); }

      var equipped = Active.Pawn.equipment?.Primary?.LabelCap;
      return equipped is null ? string.Empty : Lang.Get("Model.Info.Equipped", equipped.Bold());
    }
  }
}
