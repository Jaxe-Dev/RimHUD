using System.Collections.Generic;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips;

public static class MentalTooltip
{
  public static string? Get()
  {
    if (Active.Pawn.needs?.mood?.thoughts is null) { return null; }

    var thoughts = new List<Thought>();
    PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(Active.Pawn.needs.mood, thoughts);

    var builder = new StringBuilder();
    foreach (var thought in thoughts)
    {
      var offset = thought.MoodOffset();
      var color = offset switch
      {
        <= -10 => Theme.CriticalColor.Value,
        < 0 => Theme.WarningColor.Value,
        >= 10 => Theme.ExcellentColor.Value,
        > 0 => Theme.GoodColor.Value,
        _ => Theme.NeutralColor.Value
      };

      var similar = new List<Thought>();
      Active.Pawn.needs.mood.thoughts.GetMoodThoughts(thought, similar);

      var thoughtLabel = thought.LabelCap;
      if (similar.Count > 1) { thoughtLabel += $" x{similar.Count}"; }

      var line = thoughtLabel.WithValue((offset * similar.Count).ToStringWithSign()).Colorize(color);
      builder.AppendLine(line);
    }

    builder.AppendLine();

    if (Active.Pawn.Inspired && Active.Pawn.Inspiration != null) { builder.AppendLine(Active.Pawn.Inspiration.InspectLine.Colorize(Theme.ExcellentColor.Value)); }

    return builder.ToStringTrimmedOrNull();
  }
}
