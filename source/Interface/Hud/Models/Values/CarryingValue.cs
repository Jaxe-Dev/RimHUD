using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class CarryingValue : ValueModel
{
  protected override string Value { get; } = GetValue();

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleGear;

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string GetValue()
  {
    var carried = Active.Pawn.carryTracker?.CarriedThing?.LabelCap;
    return carried is null ? string.Empty : Lang.Get("Model.Info.Carrying", carried.Bold());
  }
}
