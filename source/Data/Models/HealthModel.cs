using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class HealthModel
  {
    public PawnModel Model { get; }

    public HealthBarModel Bar => new HealthBarModel(Model);
    public float Percent => Model.Base.health?.summaryHealth?.SummaryHealthPercent ?? -1f;
    public TextModel Condition => GetHealthCondition();
    public Func<string> Tooltip { get; }

    public HealthModel(PawnModel model)
    {
      Model = model;
      Tooltip = GetHealthTooltip;
    }

    private static void OnClick() => InspectPanePlus.ToggleHealthTab();

    private string GetBleedWarning()
    {
      var bloodLossTicksRemaining = HealthUtility.TicksUntilDeathDueToBloodLoss(Model.Base);
      return bloodLossTicksRemaining < GenDate.TicksPerDay ? Lang.Get("Model.Health.Bleed", bloodLossTicksRemaining.ToStringTicksToPeriod()).Colorize(Theme.CriticalColor.Value) : null;
    }

    private string GetLifeThreateningWarning()
    {
      var threats = Model.Base.health.hediffSet.hediffs.Where(hediff => hediff.CurStage?.lifeThreatening ?? false).ToArray();
      var count = threats.Count();
      if (count == 0) { return null; }

      var worst = threats.MinBy(hediff => hediff.CurStage.deathMtbDays);
      var text = count == 1 ? Lang.Get("Model.Health.Threat", worst.LabelCap) : Lang.Get("Model.Health.ThreatPlural", worst.LabelCap, count);

      return text.Colorize(Theme.CriticalColor.Value);
    }

    private string GetTendWarning()
    {
      var count = Model.Base.health.hediffSet.hediffs.Count(hediff => hediff.TendableNow());
      if (count == 0) { return null; }

      var text = count == 1 ? Lang.Get("Model.Health.Tend", count) : Lang.Get("Model.Health.TendPlural", count);
      var hasLifeThreateningCondition = GetLifeThreateningWarning();

      return text.Colorize(hasLifeThreateningCondition == null ? Theme.WarningColor.Value : Theme.CriticalColor.Value);
    }

    private string GetAffectedWarning()
    {
      var affected = VisibleHediffs(Model.Base, false).Where(hediff => hediff.Visible && !hediff.IsPermanent() && !hediff.FullyImmune() && hediff.def != null && ((hediff.CurStage?.lifeThreatening ?? false) || hediff.def.makesSickThought)).ToArray();
      var count = affected.Count();
      if (count == 0) { return null; }

      var worst = affected.MaxBy(hediff => hediff.PainFactor);

      var text = count == 1 ? Lang.Get("Model.Health.Affected", worst.LabelCap) : Lang.Get("Model.Health.AffectedPlural", worst.LabelCap, count - 1);
      return text.Colorize(Theme.WarningColor.Value);
    }

    private string GetIncapacitatedWarning() => !Model.Base.health.Downed ? null : Lang.Get("Model.Health.Incapacitated").Colorize(Theme.WarningColor.Value);

    private TextModel GetHealthCondition()
    {
      if (Model.Base.health?.hediffSet?.hediffs == null) { return null; }

      var conditions = Model.Base.Dead ? Lang.Get("Model.Health.Dead").Colorize(Theme.InfoColor.Value) : GetBleedWarning() ?? GetTendWarning() ?? GetLifeThreateningWarning() ?? GetAffectedWarning() ?? GetIncapacitatedWarning();

      return TextModel.Create(conditions ?? Lang.Get("Model.Health.Stable").Colorize(Theme.GoodColor.Value), GetHealthTooltip, OnClick);
    }

    private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss) => (IEnumerable<IGrouping<BodyPartRecord, Hediff>>) Access.Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder.Invoke(null, pawn, showBloodLoss);
    private static IEnumerable<Hediff> VisibleHediffs(Pawn pawn, bool showBloodLoss) => (IEnumerable<Hediff>) Access.Method_RimWorld_HealthCardUtility_VisibleHediffs.Invoke(null, pawn, showBloodLoss);

    private string GetHealthTooltip()
    {
      try
      {
        if (Model.Base.health?.hediffSet?.hediffs == null) { return ""; }

        var builder = new StringBuilder();

        foreach (var hediffs in VisibleHediffGroupsInOrder(Model.Base, true))
        {
          foreach (var hediff in hediffs.Where(hediff => hediff.Visible)) { builder.AppendLine(GetHealthTooltipLine(hediff)); }
        }

        if (builder.Length == 0) { builder.AppendLine("NoHealthConditions".Translate().CapitalizeFirst().Colorize(Theme.DisabledColor.Value)); }

        return builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize);
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        return "";
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
