using System;
using System.Collections.Generic;
using System.Text;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class MindModel
  {
    public const float MoodHappyLevel = 0.9f;
    public const float MoodContentLevel = 0.65f;

    public PawnModel Model { get; }
    public TextModel Condition => GetCondition();
    public Func<string> Tooltip { get; }

    public MindModel(PawnModel model)
    {
      Model = model;
      Tooltip = GetTooltip;
    }

    private static void OnClick() => InspectPanePlus.ToggleNeedsTab();

    private TextModel GetCondition()
    {
      if (Model.Base.mindState?.mentalStateHandler == null) { return null; }
      if (Model.Base.mindState.mentalStateHandler.InMentalState) { return TextModel.Create(Model.Base.mindState.mentalStateHandler.CurState.InspectLine.Colorize(Model.Base.mindState.mentalStateHandler.CurState.def.IsAggro || Model.Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value), GetTooltip, OnClick); }

      if (Model.Base.needs?.mood == null || Model.Base.mindState?.mentalBreaker == null) { return null; }

      if (Model.Base.mindState.mentalBreaker?.BreakExtremeIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.ExtremeBreakImminent").Colorize(Theme.CriticalColor.Value), GetTooltip, OnClick); }
      if (Model.Base.mindState.mentalBreaker?.BreakMajorIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.MajorBreakImminent").Colorize(Theme.WarningColor.Value), GetTooltip, OnClick); }
      if (Model.Base.mindState.mentalBreaker?.BreakMinorIsImminent ?? true) { return TextModel.Create(Lang.Get("Model.Mood.MinorBreakImminent").Colorize(Theme.WarningColor.Value), GetTooltip, OnClick); }

      var inspiration = GetInspiration();
      if (inspiration != null) { return inspiration; }

      if (Model.Base.needs.mood.CurLevel > MoodHappyLevel) { return TextModel.Create(Lang.Get("Model.Mood.Happy").Colorize(Theme.ExcellentColor.Value), GetTooltip, OnClick); }
      return Model.Base.needs.mood.CurLevel > MoodContentLevel ? TextModel.Create(Lang.Get("Model.Mood.Content").Colorize(Theme.GoodColor.Value), GetTooltip, OnClick) : TextModel.Create(Lang.Get("Model.Mood.Indifferent").Colorize(Theme.InfoColor.Value), GetTooltip, OnClick);
    }

    private TextModel GetInspiration()
    {
      if (!Model.Base.Inspired) { return null; }

      var inspiration = Model.Base.Inspiration.InspectLine;
      return TextModel.Create(inspiration.Colorize(Theme.ExcellentColor.Value), GetTooltip, OnClick);
    }

    private string GetTooltip()
    {
      if (Model.Base.needs?.mood?.thoughts == null) { return ""; }

      var thoughts = new List<Thought>();
      try { PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(Model.Base.needs.mood, thoughts); }
      catch (Exception exception) { Mod.HandleWarning(exception); }

      var builder = new StringBuilder();
      foreach (var thought in thoughts)
      {
        float offset;
        try { offset = thought.MoodOffset(); }
        catch (Exception exception)
        {
          Mod.HandleWarning(exception);
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
          Model.Base.needs.mood.thoughts.GetMoodThoughts(thought, similar);

          var thoughtLabel = thought.LabelCap;
          if (similar.Count > 1) { thoughtLabel += " x" + similar.Count; }

          var line = $"{thoughtLabel}: {offset * similar.Count}".Color(color);
          builder.AppendLine(line);
        }
        catch (Exception exception) { Mod.HandleWarning(exception); }
      }

      builder.AppendLine();
      if (Model.Base.Inspired) { builder.AppendLine(Model.Base.Inspiration.InspectLine.Color(Theme.ExcellentColor.Value)); }

      return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : "";
    }
  }
}
