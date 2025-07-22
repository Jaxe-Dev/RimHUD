using System;
using System.Text;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Screen;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public class SkillValue : ValueModel
{
  protected override string? Label { get; }
  protected override string? Value { get; }

  protected override Func<string?>? Tooltip { get; }

  protected override Action? OnClick { get; }

  private readonly SkillRecord? _skill;

  public SkillValue(SkillDef? def)
  {
    if (def is null) { return; }

    var skill = Active.Pawn.skills?.GetSkill(def);
    if (skill is null) { return; }

    _skill = skill;

    var isActive = Active.Pawn.jobs?.curDriver?.ActiveSkill == def;
    var isSaturated = skill.LearningSaturatedToday;

    var color = skill.TotallyDisabled ? Theme.DisabledColor.Value : isSaturated ? Theme.SkillSaturatedColor.Value : isActive ? Theme.SkillActiveColor.Value : GetColor(skill);
    Label = (def.GetDefNameOrLabel() + new StringBuilder().Insert(0, Lang.Get("Model.Skill.PassionIndicator"), (int)skill.passion)).Colorize(color);

    Value = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(Math.Max(0, Math.Min(99, skill.XpProgressPercent.ToPercentageInt()))).Colorize(color);

    Tooltip = GetTooltip;

    OnClick = InspectPaneTabs.ToggleBio;
  }

  private static Color GetColor(SkillRecord skill) => skill.passion switch
  {
    Passion.Minor => Theme.SkillMinorPassionColor.Value,
    Passion.Major => Theme.SkillMajorPassionColor.Value,
    _ => Theme.MainTextColor.Value
  };

  protected StringBuilder PrepareBuilder()
  {
    var builder = new StringBuilder();
    builder.AppendLine(Reflection.RimWorld_SkillUI_GetSkillDescription.InvokeStatic<string>(_skill!));
    builder.AppendLine();

    if (_skill!.TotallyDisabled) { return builder; }

    builder.AppendStatLine(StatDefOf.WorkSpeedGlobal);
    builder.AppendStatLine(StatDefOf.GeneralLaborSpeed);
    builder.AppendLine();

    return builder;
  }

  private string? GetTooltip() => PrepareBuilder().ToStringTrimmedOrNull();
}
