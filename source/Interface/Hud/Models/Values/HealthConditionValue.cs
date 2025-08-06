using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class HealthConditionValue : ValueModel
{
  private const float HealthTopCondition = 0.9f;
  private const float HealthPoorCondition = 0.6f;
  private const float HealthCriticalCondition = 0.3f;

  protected override string? Value { get; }

  protected override Func<string?>? Tooltip { get; }

  protected override Action? OnClick { get; }

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  public HealthConditionValue()
  {
    if (Active.Pawn.health?.hediffSet?.hediffs is null) { return; }

    Value = GetValue().CapitalizeFirst();

    Tooltip = HealthTooltip.Get;

    OnClick = InspectPaneTabs.ToggleHealth;
  }

  private static string? GetValue()
  {
    if (Active.Pawn.Dead) { return "Dead".TranslateSimple().Colorize(Theme.CriticalColor.Value); }

    var bleeding = new List<Hediff>();
    var untended = new List<Hediff>();
    var threatening = new List<Hediff>();
    var affected = new List<Hediff>();
    var hasConcern = false;

    foreach (var hediff in Active.Pawn.health!.hediffSet!.hediffs.Where(static hediff => hediff.Visible))
    {
      var isTended = hediff.IsTended();

      if (hediff.def!.isBad) { hasConcern = true; }

      if (hediff.Bleeding && !isTended) { bleeding.Add(hediff); }

      if (!isTended && hediff.TendableNow()) { untended.Add(hediff); }

      if (hediff.CurStage?.lifeThreatening ?? false) { threatening.Add(hediff); }
      else if (hediff is Hediff_MissingPart || hediff.IsPermanent() || hediff.FullyImmune() || !hediff.def!.isBad) { }
      else if (hediff.def.makesSickThought || hediff.def.lethalSeverity > 0f) { affected.Add(hediff); }
    }

    if (bleeding.Count > 0)
    {
      var bleedText = GetBleedWarning();
      if (bleedText is not null) { return bleedText; }
    }
    if (untended.Count > 0) { return GetUntendedWarning(untended.ToArray()); }
    if (threatening.Count > 0) { return GetThreateningWarning(threatening.ToArray()); }
    if (affected.Count > 0) { return GetAffectedWarning(affected.ToArray()); }

    return GetIncapacitatedWarning() ?? Active.Pawn.health.summaryHealth?.SummaryHealthPercent switch
    {
      < HealthCriticalCondition => Lang.Get("Model.Health.Critical").Colorize(Theme.CriticalColor.Value),
      < HealthPoorCondition => Lang.Get("Model.Health.Poor").Colorize(Theme.WarningColor.Value),
      > HealthTopCondition when !hasConcern => Lang.Get("Model.Health.Good").Colorize(Theme.ExcellentColor.Value),
      _ => Lang.Get("Model.Health.Stable").Colorize(Theme.GoodColor.Value)
    };
  }

  private static string? GetBleedWarning()
  {
    var bloodLossTicksRemaining = HealthUtility.TicksUntilDeathDueToBloodLoss(Active.Pawn);
    return bloodLossTicksRemaining < GenDate.TicksPerDay ? Lang.Get("Model.Health.Bleed", bloodLossTicksRemaining.ToStringTicksToPeriod()).Colorize(Theme.CriticalColor.Value) : null;
  }

  private static string? GetUntendedWarning(IReadOnlyCollection<Hediff> hediffs)
  {
    var worst = hediffs.MaxBy(static hediff => hediff.TendPriority);
    var text = hediffs.Count is 1 ? Lang.Get("Model.Health.Tend", worst is Hediff_MissingPart ? $"{worst.LabelBase} {worst.Part!.Label}" : worst?.Label) : Lang.Get("Model.Health.TendPlural", hediffs.Count);

    return text.Colorize(hediffs.Any(static hediff => hediff.CurStage?.lifeThreatening ?? false) ? Theme.CriticalColor.Value : Theme.WarningColor.Value);
  }

  private static string? GetThreateningWarning(IReadOnlyCollection<Hediff> hediffs)
  {
    var worst = hediffs.MinBy(static hediff => hediff.CurStage?.deathMtbDays);
    var text = hediffs.Count is 1 ? Lang.Get("Model.Health.Threat", worst?.Label) : Lang.Get("Model.Health.ThreatPlural", hediffs.Count);

    return text.Colorize(Theme.CriticalColor.Value);
  }

  private static string? GetAffectedWarning(IReadOnlyCollection<Hediff> hediffs)
  {
    var worst = hediffs.OrderByDescending(static hediff => hediff.PainFactor).ThenByDescending(static hediff => hediff.Severity).First();

    var text = hediffs.Count switch
    {
      1 => Lang.Get("Model.Health.Affected", worst.Label),
      2 => Lang.Get("Model.Health.AffectedTwo", worst.LabelBase),
      _ => Lang.Get("Model.Health.AffectedPlural", worst.LabelBase, hediffs.Count - 1)
    };

    return text.Colorize(Theme.WarningColor.Value);
  }

  private static string? GetIncapacitatedWarning() => Active.Pawn.health!.Downed ? "Incapacitated".TranslateSimple().Colorize(Theme.WarningColor.Value) : null;
}
