﻿using System;
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
            Tooltip = GetHealthTooltip();
        }

        private static void OnClick() => InspectPanePlus.ToggleHealthTab();

        private TextModel GetBleedWarning()
        {
            var bloodLossTicksRemaining = HealthUtility.TicksUntilDeathDueToBloodLoss(Model.Base);
            var text = bloodLossTicksRemaining < GenDate.TicksPerDay ? Lang.Get("Model.Health.Bleed", bloodLossTicksRemaining.ToStringTicksToPeriod()) : null;

            return TextModel.Create(text, GetHealthTooltip(), Theme.CriticalColor.Value, OnClick);
        }

        private TextModel GetTendWarning()
        {
            var count = Model.Base.health.hediffSet.hediffs.Count(hediff => hediff.TendableNow());
            if (count == 0) { return null; }

            var text = count == 1 ? Lang.Get("Model.Health.Tend", count) : Lang.Get("Model.Health.TendPlural", count);
            var hasLifeThreateningCondition = GetLifeThreateningWarning();

            return TextModel.Create(text, GetHealthTooltip(), hasLifeThreateningCondition?.Color ?? Theme.WarningColor.Value, OnClick);
        }

        private TextModel GetLifeThreateningWarning()
        {
            var threats = Model.Base.health.hediffSet.hediffs.Where(hediff => hediff.CurStage?.lifeThreatening ?? false).ToArray();
            var count = threats.Count();
            if (count == 0) { return null; }

            var worst = threats.MinBy(hediff => hediff.CurStage.deathMtbDays);
            var text = count == 1 ? Lang.Get("Model.Health.Threat", worst.LabelCap) : Lang.Get("Model.Health.ThreatPlural", worst.LabelCap, count);

            return TextModel.Create(text, GetHealthTooltip(), Theme.CriticalColor.Value, OnClick);
        }

        private TextModel GetAffectedWarning()
        {
            var affected = VisibleHediffs(Model.Base, false).Where(hediff => hediff.Visible && !hediff.IsPermanent() && !hediff.FullyImmune() && hediff.def != null && ((hediff.CurStage?.lifeThreatening ?? false) || hediff.def.makesSickThought)).ToArray();
            var count = affected.Count();
            if (count == 0) { return null; }

            var worst = affected.MaxBy(hediff => hediff.PainFactor);

            var text = count == 1 ? Lang.Get("Model.Health.Affected", worst.LabelCap) : Lang.Get("Model.Health.AffectedPlural", worst.LabelCap, count - 1);
            return TextModel.Create(text, GetHealthTooltip(), Theme.WarningColor.Value, OnClick);
        }

        private TextModel GetIncapacitatedWarning() => !Model.Base.health.Downed ? null : TextModel.Create(Lang.Get("Model.Health.Incapacitated"), GetHealthTooltip(), Theme.WarningColor.Value, OnClick);

        private TextModel GetHealthCondition()
        {
            if (Model.Base.health?.hediffSet?.hediffs == null) { return null; }
            if (Model.Base.Dead) { return TextModel.Create(Lang.Get("Model.Health.Dead"), GetHealthTooltip(), Theme.InfoColor.Value, OnClick); }

            return GetBleedWarning() ?? GetTendWarning() ?? GetLifeThreateningWarning() ?? GetAffectedWarning() ?? GetIncapacitatedWarning() ?? TextModel.Create(Lang.Get("Model.Health.Stable"), GetHealthTooltip(), Theme.GoodColor.Value, OnClick);
        }

        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss) => (IEnumerable<IGrouping<BodyPartRecord, Hediff>>) Access.Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder.Invoke(null, pawn, showBloodLoss);
        private static IEnumerable<Hediff> VisibleHediffs(Pawn pawn, bool showBloodLoss) => (IEnumerable<Hediff>) Access.Method_RimWorld_HealthCardUtility_VisibleHediffs.Invoke(null, pawn, showBloodLoss);

        private Func<string> GetHealthTooltip() => () =>
        {
            try
            {
                if (Model.Base.health?.hediffSet?.hediffs == null) { return ""; }

                var builder = new StringBuilder();

                foreach (var hediffs in VisibleHediffGroupsInOrder(Model.Base, true))
                {
                    foreach (var hediff in hediffs.Where(hediff => hediff.Visible)) { builder.AppendLine(GetHealthTooltipLine(hediff)); }
                }

                if (builder.Length == 0) { builder.AppendLine("NoHealthConditions".Translate().CapitalizeFirst().Color(Theme.DisabledColor.Value)); }

                return builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize);
            }
            catch (Exception exception)
            {
                Mod.HandleWarning(exception);
                return "";
            }
        };

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

                return $"{part}: {condition}".Color(color);
            }
            catch (Exception exception)
            {
                Mod.HandleWarning(exception);
                return null;
            }
        }
    }
}
