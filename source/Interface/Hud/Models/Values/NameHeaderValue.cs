using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class NameHeaderValue : ValueModel
  {
    protected override string Value { get; }

    protected override Func<string?> Tooltip { get; }

    protected override Action OnClick { get; }

    protected override TextStyle TextStyle => Theme.LargeTextStyle;

    public NameHeaderValue()
    {
      Value = Active.Name.Colorize(Active.FactionRelationColor);

      Tooltip = BioTooltip.Get;

      OnClick = InspectPaneTabs.ToggleSocial;
    }
  }
}
