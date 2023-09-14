using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class AnimalMasterValue : ValueModel
  {
    protected override string? Value { get; }

    protected override Func<string?>? Tooltip { get; }

    protected override Action? OnClick { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public AnimalMasterValue()
    {
      var master = Active.Pawn.playerSettings?.Master;
      if (master is null) { return; }

      var masterName = master.LabelShort;
      var relation = Active.Pawn.GetMostImportantRelation(master)?.LabelCap;

      var text = Lang.Get("Model.Bio.Master", masterName);

      Value = relation is null ? text : text.Colorize(Theme.SkillMinorPassionColor.Value);

      Tooltip = AnimalTooltip.Get;

      OnClick = InspectPaneTabs.ToggleSocial;
    }
  }
}
