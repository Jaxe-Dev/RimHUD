using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips;

public static class HealthTooltip
{
  public static string? Get()
  {
    try
    {
      var builder = new StringBuilder();

      foreach (var hediff in VisibleHediffGroupsInOrder(true).SelectMany(static hediffs => hediffs.Where(static hediff => hediff.Visible))) { builder.AppendLine(GetHealthTooltipLine(hediff)); }

      if (builder.Length is 0) { builder.AppendLine("NoHealthConditions".TranslateSimple().CapitalizeFirst().Colorize(Theme.DisabledColor.Value)); }

      return builder.ToStringTrimmedOrNull();
    }
    catch (Exception exception)
    {
      Report.HandleWarning(exception);
      return null;
    }
  }

  private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(bool showBloodLoss) => Reflection.RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder.InvokeStatic<IEnumerable<IGrouping<BodyPartRecord, Hediff>>>(Active.Pawn, showBloodLoss);

  private static string? GetHealthTooltipLine(Hediff hediff)
  {
    try
    {
      var part = hediff.Part?.LabelCap ?? "WholeBody".TranslateSimple();

      var condition = hediff.LabelCap;

      Color color;
      if (!hediff.def!.isBad) { color = Theme.GoodColor.Value; }
      else if (hediff.IsPermanent() || hediff.FullyImmune()) { color = Theme.InfoColor.Value; }
      else if (hediff.def.IsAddiction || hediff.IsTended()) { color = Theme.WarningColor.Value; }
      else { color = Theme.CriticalColor.Value; }

      return $"{part}: {condition}".Colorize(color);
    }
    catch (Exception exception)
    {
      Report.HandleWarning(exception);
      return null;
    }
  }
}
