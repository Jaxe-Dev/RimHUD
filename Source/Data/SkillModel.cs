using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal class SkillModel
    {
        public bool Disabled { get; }

        public string Label { get; }
        public string Level { get; }
        public Color Color { get; }

        public SkillModel(Pawn pawn, SkillDef def)
        {
            var skill = pawn.skills?.GetSkill(def);

            if (skill == null)
            {
                Disabled = true;
                return;
            }

            Label = def.LabelCap + new string('+', (int) skill.passion);
            Level = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(skill.XpProgressPercent.ToPercentageInt()) + (skill.LearningSaturatedToday ? "*" : null);
            Color = skill.TotallyDisabled ? Theme.SkillDisabledColor : GetSkillPassionColor(skill.passion);
        }

        private static Color GetSkillPassionColor(Passion passion)
        {
            if (passion == Passion.None) { return Theme.SkillNoPassionColor; }
            if (passion == Passion.Minor) { return Theme.SkillMinorPassionColor; }
            if (passion == Passion.Major) { return Theme.SkillMajorPassionColor; }

            throw new Mod.Exception("Invalid skill passion level.");
        }
    }
}
