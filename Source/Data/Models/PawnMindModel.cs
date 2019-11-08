using System.Collections.Generic;
using System.Text;
using RimHUD.Data.Extensions;
using RimHUD.Interface;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal class PawnMindModel
    {
        public PawnModel Model { get; }
        public TextModel Condition => GetCondition();
        public TipSignal? Tooltip => GetTooltip();

        public PawnMindModel(PawnModel model) => Model = model;

        private static void OnClick() => InspectPanePlus.ToggleNeedsTab();

        private TextModel GetCondition()
        {
            if (Model.Base.mindState?.mentalStateHandler == null) { return null; }
            if (Model.Base.mindState.mentalStateHandler.InMentalState) { return TextModel.Create(Model.Base.mindState.mentalStateHandler.CurState.InspectLine, GetTooltip(), Model.Base.mindState.mentalStateHandler.CurState.def.IsAggro || Model.Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.Theme.CriticalColor.Value : Theme.Theme.WarningColor.Value, OnClick); }

            if ((Model.Base.needs?.mood == null) || (Model.Base.mindState?.mentalBreaker == null)) { return null; }

            if (Model.Base.mindState.mentalBreaker.BreakExtremeIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.ExtremeBreakImminent"), GetTooltip(), Theme.Theme.CriticalColor.Value, OnClick); }
            if (Model.Base.mindState.mentalBreaker.BreakMajorIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.MajorBreakImminent"), GetTooltip(), Theme.Theme.WarningColor.Value, OnClick); }
            if (Model.Base.mindState.mentalBreaker.BreakMinorIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.MinorBreakImminent"), GetTooltip(), Theme.Theme.WarningColor.Value, OnClick); }

            var inspiration = GetInspiration();
            if (inspiration != null) { return inspiration; }

            if (Model.Base.needs.mood.CurLevel > 0.9f) { return TextModel.Create(Lang.Get("Model.Mood.Happy"), GetTooltip(), Theme.Theme.ExcellentColor.Value, OnClick); }
            return Model.Base.needs.mood.CurLevel > 0.65f ? TextModel.Create(Lang.Get("Model.Mood.Content"), GetTooltip(), Theme.Theme.GoodColor.Value, OnClick) : TextModel.Create(Lang.Get("Model.Mood.Indifferent"), GetTooltip(), Theme.Theme.InfoColor.Value, OnClick);
        }

        private TextModel GetInspiration()
        {
            if (!Model.Base.Inspired) { return null; }

            var inspiration = Model.Base.Inspiration.InspectLine;
            return TextModel.Create(inspiration, GetTooltip(), Theme.Theme.ExcellentColor.Value, OnClick);
        }

        private TipSignal? GetTooltip()
        {
            if (Model.Base.needs?.mood?.thoughts == null) { return null; }

            var thoughts = new List<Thought>();
            PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(Model.Base.needs.mood, thoughts);

            var builder = new StringBuilder();
            foreach (var thought in thoughts)
            {
                var offset = thought.MoodOffset();

                Color color;
                if (offset <= -10) { color = Theme.Theme.CriticalColor.Value; }
                else if (offset < 0) { color = Theme.Theme.WarningColor.Value; }
                else if (offset >= 10) { color = Theme.Theme.ExcellentColor.Value; }
                else if (offset > 0) { color = Theme.Theme.GoodColor.Value; }
                else { color = Theme.Theme.InfoColor.Value; }

                var similar = new List<Thought>();
                Model.Base.needs.mood.thoughts.GetMoodThoughts(thought, similar);

                var thoughtLabel = thought.LabelCap;
                if (similar.Count > 1) { thoughtLabel += " x" + similar.Count; }

                var line = $"{thoughtLabel}: {offset * similar.Count}".Color(color);
                builder.AppendLine(line);
            }

            builder.AppendLine();
            if (Model.Base.Inspired) { builder.AppendLine(Model.Base.Inspiration.InspectLine.Color(Theme.Theme.ExcellentColor.Value)); }

            return builder.Length > 0 ? new TipSignal(() => builder.ToStringTrimmed().Size(Theme.Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId) : null;
        }
    }
}
