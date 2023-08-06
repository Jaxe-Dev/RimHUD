using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class HealthConditionModel
  {
    public PawnModel Owner { get; }

    public HealthModel Bar => new HealthModel(Owner);
    public TextModel Condition => Owner.Base.health?.hediffSet?.hediffs == null ? null : TextModel.Create(GetCondition().CapitalizeFirst(), GetHealthTooltip, OnClick);
    public Func<string> ConditionTooltip => GetHealthTooltip;

    public HealthConditionModel(PawnModel owner) => Owner = owner;

    private static void OnClick() => InspectPanePlus.ToggleHealthTab();

    private string GetCondition()
    {
      if (Owner.Base.health?.hediffSet?.hediffs == null) { return null; }

      if (Owner.Base.Dead) { return Lang.Get("Model.Health.Dead").Colorize(Theme.InfoColor.Value); }

      var bleeding = new List<Hediff>();
      var untended = new List<Hediff>();
      var threatening = new List<Hediff>();
      var affected = new List<Hediff>();

      foreach (var hediff in Owner.Base.health.hediffSet.hediffs.Where(hediff => hediff.Visible).ToArray())
      {
        try
        {
          var isTended = hediff.IsTended();

          if (hediff.Bleeding && !isTended) { bleeding.Add(hediff); }

          if (!isTended && hediff.TendableNow()) { untended.Add(hediff); }

          if (hediff.CurStage?.lifeThreatening ?? false) { threatening.Add(hediff); }
          else if (hediff is Hediff_MissingPart || hediff.IsPermanent() || hediff.FullyImmune() || !hediff.def.isBad) { }
          else if (hediff.def.makesSickThought || hediff.def.lethalSeverity > 0f) { affected.Add(hediff); }
        }
        catch { }
      }

      if (bleeding.Count > 0)
      {
        var bleedText = GetBleedWarning();
        if (bleedText != null) { return bleedText; }
      }
      if (untended.Count > 0) { return GetUntendedWarning(untended.ToArray()); }
      if (threatening.Count > 0) { return GetThreateningWarning(threatening.ToArray()); }
      if (affected.Count > 0) { return GetAffectedWarning(affected.ToArray()); }

      return GetIncapacitatedWarning() ?? Lang.Get("Model.Health.Stable").Colorize(Theme.GoodColor.Value);
    }

    private string GetBleedWarning()
    {
      var bloodLossTicksRemaining = HealthUtility.TicksUntilDeathDueToBloodLoss(Owner.Base);
      return bloodLossTicksRemaining < GenDate.TicksPerDay ? Lang.Get("Model.Health.Bleed", bloodLossTicksRemaining.ToStringTicksToPeriod()).Colorize(Theme.CriticalColor.Value) : null;
    }

    private static string GetUntendedWarning(Hediff[] hediffs)
    {
      var worst = hediffs.MaxBy(hediff => hediff.TendPriority);
      var text = hediffs.Length == 1 ? Lang.Get("Model.Health.Tend", worst is Hediff_MissingPart ? $"{worst.LabelBase} {worst.Part.Label}" : worst.Label) : Lang.Get("Model.Health.TendPlural", hediffs.Length);

      return text.Colorize(hediffs.Any(hediff => hediff.CurStage?.lifeThreatening ?? false) ? Theme.CriticalColor.Value : Theme.WarningColor.Value);
    }

    private static string GetThreateningWarning(Hediff[] hediffs)
    {
      var worst = hediffs.MinBy(hediff => hediff.CurStage.deathMtbDays);
      var text = hediffs.Length == 1 ? Lang.Get("Model.Health.Threat", worst.Label) : Lang.Get("Model.Health.ThreatPlural", hediffs.Length);

      return text.Colorize(Theme.CriticalColor.Value);
    }

    private static string GetAffectedWarning(Hediff[] hediffs)
    {
      var worst = hediffs.OrderByDescending(hediff => hediff.PainFactor).ThenByDescending(hediff => hediff.Severity).First();

      string text;
      if (hediffs.Length == 1) { text = Lang.Get("Model.Health.Affected", worst.Label); }
      else if (hediffs.Length == 2) { text = Lang.Get("Model.Health.AffectedTwo", worst.LabelBase); }
      else { text = Lang.Get("Model.Health.AffectedPlural", worst.LabelBase, hediffs.Length - 1); }

      return text.Colorize(Theme.WarningColor.Value);
    }

    private string GetIncapacitatedWarning() => !Owner.Base.health.Downed ? null : Lang.Get("Model.Health.Incapacitated").Colorize(Theme.WarningColor.Value);

    private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss) => (IEnumerable<IGrouping<BodyPartRecord, Hediff>>)Access.Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder.Invoke(null, pawn, showBloodLoss);
    private static IEnumerable<Hediff> VisibleHediffs(Pawn pawn, bool showBloodLoss) => (IEnumerable<Hediff>)Access.Method_RimWorld_HealthCardUtility_VisibleHediffs.Invoke(null, pawn, showBloodLoss);

    private string GetHealthTooltip()
    {
      try
      {
        if (Owner.Base.health?.hediffSet?.hediffs == null) { return null; }

        var builder = new StringBuilder();

        foreach (var hediffs in VisibleHediffGroupsInOrder(Owner.Base, true))
        {
          foreach (var hediff in hediffs.Where(hediff => hediff.Visible)) { builder.AppendLine(GetHealthTooltipLine(hediff)); }
        }

        if (builder.Length == 0) { builder.AppendLine("NoHealthConditions".Translate().CapitalizeFirst().Colorize(Theme.DisabledColor.Value)); }

        return builder.ToTooltip();
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        return null;
      }
    }

    private static string GetHealthTooltipLine(Hediff hediff)
    {
      try
      {
        var part = hediff.Part?.LabelCap ?? "WholeBody".Translate();

        var condition = hediff.LabelCap;

        Color color;
        if (!hediff.def.isBad) { color = Theme.GoodColor.Value; }
        else if (hediff.IsPermanent() || hediff.FullyImmune()) { color = Theme.InfoColor.Value; }
        else if (hediff.def.IsAddiction || hediff.IsTended()) { color = Theme.WarningColor.Value; }
        else { color = Theme.CriticalColor.Value; }

        return $"{part}: {condition}".Colorize(color);
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        return null;
      }
    }
  }
}
