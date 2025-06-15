using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class NameHeaderValue : ValueModel
{
  protected override string Value { get; } = Active.Name.Colorize(Active.FactionRelationColor);

  protected override Func<string?> Tooltip { get; } = BioTooltip.Get;

  protected override Action OnClick { get; } = InspectPaneTabs.ToggleSocial;

  protected override TextStyle TextStyle => Theme.LargeTextStyle;
}
