using System.Collections.Generic;
using System.Text;
using RimHUD.Interface;
using RimHUD.Patch;
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

        private TextModel GetCondition()
        {
            if (Model.Base.mindState?.mentalStateHandler == null) { return null; }
            if (Model.Base.mindState.mentalStateHandler.InMentalState) { return TextModel.Create(Model.Base.mindState.mentalStateHandler.CurState.InspectLine, GetTooltip(), Model.Base.mindState.mentalStateHandler.CurState.def.IsAggro || Model.Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value); }

            if ((Model.Base.needs?.mood == null) || (Model.Base.mindState?.mentalBreaker == null)) { return null; }

            if (Model.Base.mindState.mentalBreaker.BreakExtremeIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.ExtremeBreakImminent"), GetTooltip(), Theme.CriticalColor.Value); }
            if (Model.Base.mindState.mentalBreaker.BreakMajorIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.MajorBreakImminent"), GetTooltip(), Theme.WarningColor.Value); }
            if (Model.Base.mindState.mentalBreaker.BreakMinorIsImminent) { return TextModel.Create(Lang.Get("Model.Mood.MinorBreakImminent"), GetTooltip(), Theme.WarningColor.Value); }

            var inspiration = GetInspiration();
            if (inspiration != null) { return inspiration; }

            if (Model.Base.needs.mood.CurLevel > 0.9f) { return TextModel.Create(Lang.Get("Model.Mood.Happy"), GetTooltip(), Theme.ExcellentColor.Value); }
            return Model.Base.needs.mood.CurLevel > 0.65f ? TextModel.Create(Lang.Get("Model.Mood.Content"), GetTooltip(), Theme.GoodColor.Value) : TextModel.Create(Lang.Get("Model.Mood.Indifferent"), GetTooltip(), Theme.InfoColor.Value);
        }

        private TextModel GetInspiration()
        {
            if (!Model.Base.Inspired) { return null; }

            var inspiration = Model.Base.Inspiration.InspectLine;
            return TextModel.Create(inspiration, GetTooltip(), Theme.ExcellentColor.Value);
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
                if (offset <= -10) { color = Theme.CriticalColor.Value; }
                else if (offset < 0) { color = Theme.WarningColor.Value; }
                else if (offset >= 10) { color = Theme.ExcellentColor.Value; }
                else if (offset > 0) { color = Theme.GoodColor.Value; }
                else { color = Theme.InfoColor.Value; }

                var line = $"{thought.LabelCap}: {offset}".Color(color);
                builder.AppendLine(line);
            }

            builder.AppendLine();
            if (Model.Base.Inspired) { builder.AppendLine(Model.Base.Inspiration.InspectLine.Color(Theme.ExcellentColor.Value)); }

            return builder.Length > 0 ? new TipSignal(() => builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize), GUIPlus.TooltipId) : null;
        }
    }
}
