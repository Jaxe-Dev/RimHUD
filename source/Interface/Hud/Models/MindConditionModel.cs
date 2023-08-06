using System;
using System.Collections.Generic;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class MindConditionModel
  {
    private const float MoodHappyLevel = 0.9f;
    private const float MoodContentLevel = 0.65f;

    public PawnModel Owner { get; }

    public TextModel Condition => GetCondition();
    public Func<string> Tooltip { get; }

    public MindConditionModel(PawnModel owner)
    {
      Owner = owner;
      Tooltip = GetTooltip;
    }

    private static void OnClick() => InspectPanePlus.ToggleNeedsTab();

    private TextModel GetCondition()
    {
      if (Owner.Base.mindState?.mentalStateHandler == null) { return null; }
      if (Owner.Base.mindState.mentalStateHandler.InMentalState) { return TextModel.Create(Owner.Base.mindState.mentalStateHandler.CurState.InspectLine.Colorize(Owner.Base.mindState.mentalStateHandler.CurState.def.IsAggro || Owner.Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value), GetTooltip, OnClick); }

      if (Owner.Base.needs?.mood == null || Owner.Base.mindState?.mentalBreaker == null) { return null; }

      if (Owner.Base.mindState.mentalBreaker?.BreakExtremeIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.ExtremeBreakImminent").Colorize(Theme.CriticalColor.Value), GetTooltip, OnClick); }
      if (Owner.Base.mindState.mentalBreaker?.BreakMajorIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.MajorBreakImminent").Colorize(Theme.WarningColor.Value), GetTooltip, OnClick); }
      if (Owner.Base.mindState.mentalBreaker?.BreakMinorIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.MinorBreakImminent").Colorize(Theme.WarningColor.Value), GetTooltip, OnClick); }

      var inspiration = GetInspiration();
      if (inspiration != null) { return inspiration; }

      if (Owner.Base.needs.mood.CurLevel > MoodHappyLevel) { return TextModel.Create(Lang.Get("Model.Mood.Happy").Colorize(Theme.ExcellentColor.Value), GetTooltip, OnClick); }
      return Owner.Base.needs.mood.CurLevel > MoodContentLevel ? TextModel.Create(Lang.Get("Model.Mood.Content").Colorize(Theme.GoodColor.Value), GetTooltip, OnClick) : TextModel.Create(Lang.Get("Model.Mood.Indifferent").Colorize(Theme.InfoColor.Value), GetTooltip, OnClick);
    }

    private TextModel GetInspiration()
    {
      var inspiration = GetInspirationInspectLine();
      return inspiration == null ? null : TextModel.Create(inspiration.Colorize(Theme.ExcellentColor.Value), GetTooltip, OnClick);
    }

    private string GetInspirationInspectLine()
    {
      try { return !Owner.Base.Inspired ? null : Owner.Base.Inspiration.InspectLine; }
      catch { return null; }
    }

    private string GetTooltip()
    {
      if (Owner.Base.needs?.mood?.thoughts == null) { return ""; }

      var thoughts = new List<Thought>();
      try { PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(Owner.Base.needs.mood, thoughts); }
      catch (Exception exception) { Troubleshooter.HandleWarning(exception); }

      var builder = new StringBuilder();
      foreach (var thought in thoughts)
      {
        float offset;
        try { offset = thought.MoodOffset(); }
        catch (Exception exception)
        {
          Troubleshooter.HandleWarning(exception);
          offset = 0;
        }

        Color color;
        if (offset <= -10) { color = Theme.CriticalColor.Value; }
        else if (offset < 0) { color = Theme.WarningColor.Value; }
        else if (offset >= 10) { color = Theme.ExcellentColor.Value; }
        else if (offset > 0) { color = Theme.GoodColor.Value; }
        else { color = Theme.InfoColor.Value; }

        try
        {
          var similar = new List<Thought>();
          Owner.Base.needs.mood.thoughts.GetMoodThoughts(thought, similar);

          var thoughtLabel = thought.LabelCap;
          if (similar.Count > 1) { thoughtLabel += " x" + similar.Count; }

          var line = $"{thoughtLabel}: {offset * similar.Count}".Colorize(color);
          builder.AppendLine(line);
        }
        catch (Exception exception) { Troubleshooter.HandleWarning(exception); }
      }

      builder.AppendLine();

      try
      {
        if (Owner.Base.Inspired) { builder.AppendLine(Owner.Base.Inspiration.InspectLine.Colorize(Theme.ExcellentColor.Value)); }
      }
      catch { }

      return builder.ToTooltip();
    }
  }
}
