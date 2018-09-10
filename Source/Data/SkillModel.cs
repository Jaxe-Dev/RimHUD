using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal class SkillModel
    {
        private static Color SkillDisabledColor { get; } = new Color(0.5f, 0.5f, 0.5f);
        private static Color SkillNoPassionColor { get; } = new Color(0.9f, 0.9f, 0.9f);
        private static Color SkillMinorPassionColor { get; } = new Color(1f, 0.9f, 0.7f);
        private static Color SkillMajorPassionColor { get; } = new Color(1f, 0.8f, 0.4f);

        public bool Disabled { get; }

        public string Label { get; }
        public string Level { get; }
        public Color Color { get; }

        public SkillModel(Pawn pawn, SkillDef def)
        {
            var skill = pawn.skills?.GetSkill(def);

            Disabled = skill == null;
            if (Disabled) { return; }

            Label = def.LabelCap + new string('+', (int) skill.passion);
            Level = skill.TotallyDisabled ? "-" : skill.Level.ToDecimalString(skill.XpProgressPercent.ToPercentageInt()) + (skill.LearningSaturatedToday ? "*" : null);
            Color = skill.TotallyDisabled ? SkillDisabledColor : GetSkillPassionColor(skill.passion);
        }

        private static Color GetSkillPassionColor(Passion passion)
        {
            if (passion == Passion.None) { return SkillNoPassionColor; }
            if (passion == Passion.Minor) { return SkillMinorPassionColor; }
            if (passion == Passion.Major) { return SkillMajorPassionColor; }

            throw new Mod.Exception("Invalid skill passion level.");
        }
    }
}
