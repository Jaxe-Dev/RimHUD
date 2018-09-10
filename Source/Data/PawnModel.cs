using System.Linq;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Data
{
    internal class PawnModel
    {
        public static PawnModel Selected => GetSelected();

        public Pawn Base { get; }

        public string Name => Base.Name?.ToStringFull ?? Base.LabelCap;
        public string GenderAndAge => GetGenderAndAge();
        public Color FactionRelationColor => GetFactionRelationColor();
        public string RelationKindAndFaction => GetRelationKindAndFaction();

        public StringPlus HealthCondition => GetHealthCondition();
        public StringPlus MentalCondition => GetMentalCondition();

        public bool IsAnimal => Base.RaceProps.Animal;

        public float Health => Base.health?.summaryHealth?.SummaryHealthPercent ?? HudListing.DefaultNullValue;
        public float Rest => Base.needs?.rest?.CurLevelPercentage ?? HudListing.DefaultNullValue;
        public float Food => Base.needs?.food?.CurLevelPercentage ?? HudListing.DefaultNullValue;
        public float Recreation => Base.needs?.joy?.CurLevelPercentage ?? HudListing.DefaultNullValue;
        public float Mood => Base.needs?.mood?.CurLevelPercentage ?? HudListing.DefaultNullValue;
        public float MoodThresholdMinor => Base.mindState?.mentalBreaker?.BreakThresholdMinor ?? HudListing.DefaultNullValue;
        public float MoodThresholdMajor => Base.mindState?.mentalBreaker?.BreakThresholdMajor ?? HudListing.DefaultNullValue;
        public float MoodThresholdExtreme => Base.mindState?.mentalBreaker?.BreakThresholdExtreme ?? HudListing.DefaultNullValue;

        public SkillModel Shooting => GetSkillModel(SkillDefOf.Shooting);
        public SkillModel Melee => GetSkillModel(SkillDefOf.Melee);
        public SkillModel Construction => GetSkillModel(SkillDefOf.Construction);
        public SkillModel Mining => GetSkillModel(SkillDefOf.Mining);
        public SkillModel Cooking => GetSkillModel(SkillDefOf.Cooking);
        public SkillModel Plants => GetSkillModel(SkillDefOf.Plants);
        public SkillModel Animals => GetSkillModel(SkillDefOf.Animals);
        public SkillModel Crafting => GetSkillModel(SkillDefOf.Crafting);
        public SkillModel Artistic => GetSkillModel(SkillDefOf.Artistic);
        public SkillModel Medicine => GetSkillModel(SkillDefOf.Medicine);
        public SkillModel Social => GetSkillModel(SkillDefOf.Social);
        public SkillModel Intellectual => GetSkillModel(SkillDefOf.Intellectual);

        public string Activity => GetActivity();
        public string Equipped => GetEquipped();

        private PawnModel(Pawn pawn) => Base = pawn;

        private static PawnModel GetSelected()
        {
            if ((Current.Game == null) || !(Find.Selector.SingleSelectedThing is Pawn pawn)) { return null; }
            return pawn == null ? null : new PawnModel(pawn);
        }

        private SkillModel GetSkillModel(SkillDef def) => new SkillModel(Base, def);

        private string GetGender() => Base.gender == Gender.None ? null : Base.GetGenderLabel().CapitalizeFirst() + " ";
        private string GetGenderAndAge()
        {
            var gender = GetGender();
            var race = gender == null ? Base.kindDef.race.LabelCap : Base.kindDef.race.label;

            var ageYears = Base.ageTracker.AgeBiologicalYears;
            var ageDays = (Base.ageTracker.BirthDayOfYear - GenDate.DaysPassed).WrapTo(GenDate.DaysPerYear);

            var age = ageYears.ToString().Bold();
            if ((ageDays == 0) || (ageDays == GenDate.DaysPerYear)) { age += Lang.Get("Age.Birthday"); }
            else if (ageDays == 1) { age += Lang.Get("Age.Day"); }
            else { age += Lang.Get("Age.Days", ageDays); }

            return Lang.Get("GenderAndAge", gender, race, age);
        }

        private string GetKind()
        {
            if (!IsAnimal) { return Base.Faction == Faction.OfPlayer ? Base.story?.Title ?? Base.KindLabel : Base.TraderKind?.label ?? Base.KindLabel; }

            if (Base.Faction == null)
            {
                if (Base.RaceProps.petness > 0.5f) { return Lang.Get("Creature.Stray"); }
                if (Base.RaceProps.predator) { return Lang.Get("Creature.Predator"); }
                if (Base.RaceProps.packAnimal || (Base.kindDef.race.tradeTags?.Contains("AnimalFarm") ?? false) || Base.RaceProps.herdAnimal) { return Lang.Get("Creature.Beast"); }
                if (Base.kindDef.race.tradeTags?.Contains("AnimalInsect") ?? false) { return Lang.Get("Creature.Insect"); }
                return Lang.Get("Creature.Wild");
            }

            if (Base.RaceProps.petness > 0.5f) { return Lang.Get("Creature.Pet"); }
            if (Base.RaceProps.petness > 0f) { return Lang.Get("Creature.ExoticPet"); }
            if (Base.RaceProps.predator) { return Lang.Get("Creature.Hunt"); }
            if (Base.RaceProps.packAnimal) { return Lang.Get("Creature.Pack"); }
            if ((Base.kindDef.race.tradeTags?.Contains("AnimalFarm") ?? false)) { return Lang.Get("Creature.Farm"); }
            if (Base.RaceProps.herdAnimal) { return Lang.Get("Creature.Herd"); }
            if (Base.kindDef.race.tradeTags?.Contains("AnimalInsect") ?? false) { return Lang.Get("Creature.Insect"); }

            return Lang.Get("Creature.Tame");
        }

        private Color GetFactionRelationColor()
        {
            if (Base.Faction == null) { return IsAnimal ? Theme.FactionWildColor : Theme.FactionIndependentColor; }
            if (Base.Faction.IsPlayer) { return Theme.FactionOwnColor; }

            if (Base.Faction.PlayerRelationKind == FactionRelationKind.Hostile) { return Theme.FactionHostileColor; }
            return Base.Faction.PlayerRelationKind == FactionRelationKind.Ally ? Theme.FactionAlliedColor : Theme.FactionIndependentColor;
        }

        private string GetFactionRelation()
        {
            if (Base.Faction == null) { return IsAnimal ? (Base.kindDef == PawnKindDefOf.WildMan ? null : Lang.Get("Faction.Wild")) : Lang.Get("Faction.Independent"); }
            if (Base.Faction.IsPlayer) { return null; }

            var relation = Base.Faction.PlayerRelationKind;
            if (relation == FactionRelationKind.Hostile) { return Base.RaceProps.IsMechanoid ? Lang.Get("Faction.Hostile") : Lang.Get("Faction.Enemy"); }
            return relation == FactionRelationKind.Ally ? Lang.Get("Faction.Allied") : null;
        }

        private string GetRelationKindAndFaction()
        {
            var faction = (Base.Faction == null) || !Base.Faction.HasName ? null : Lang.Get("OfFaction", Base.Faction.Name);
            var relation = GetFactionRelation();
            var kind = GetKind();

            return Lang.Get("RelationKindAndFaction", relation, relation == null ? kind.CapitalizeFirst() : kind, faction);
        }

        private StringPlus GetBleedWarning()
        {
            var bloodLossTicksRemaining = HealthUtility.TicksUntilDeathDueToBloodLoss(Base);
            var text = bloodLossTicksRemaining < GenDate.TicksPerDay ? Lang.Get("Health.Bleed", bloodLossTicksRemaining.ToStringTicksToPeriod()) : null;

            return StringPlus.Create(text, Theme.CriticalColor);
        }

        private StringPlus GetTendWarning()
        {
            var count = Base.health.hediffSet.hediffs.Count(hediff => hediff.TendableNow());
            if (count == 0) { return null; }

            var text = count == 1 ? Lang.Get("Health.Tend", count) : Lang.Get("Health.TendPlural", count);
            var hasLifeThreateningCondition = GetLifeThreateningWarning();

            return StringPlus.Create(text, hasLifeThreateningCondition?.Color ?? Theme.WarningColor);
        }

        private StringPlus GetLifeThreateningWarning()
        {
            var threats = Base.health.hediffSet.hediffs.Where(hediff => hediff.CurStage?.lifeThreatening ?? false);
            var count = threats.Count();
            if (count == 0) { return null; }

            var worst = threats.MinBy(hediff => hediff.CurStage.deathMtbDays);
            var text = count == 1 ? Lang.Get("Health.Threat", worst.LabelCap) : Lang.Get("Health.ThreatPlural", worst.LabelCap, count);

            return StringPlus.Create(text, Theme.CriticalColor);
        }

        private StringPlus GetSicknessWarning()
        {
            var sicknesses = Base.health.hediffSet.hediffs.Where(hediff => hediff.def.makesSickThought);
            var count = sicknesses.Count();
            if (count == 0) { return null; }

            var worst = sicknesses.MaxBy(hediff => hediff.PainFactor);

            var text = count == 1 ? Lang.Get("Health.Sick", worst.LabelCap) : Lang.Get("Health.SickPlural", worst.LabelCap, count);
            return StringPlus.Create(text, Theme.WarningColor);
        }

        private StringPlus GetIncapacitatedWarning() => !Base.health.Downed ? null : StringPlus.Create(Lang.Get("Health.Incapacitated"), Theme.WarningColor);

        private StringPlus GetHealthCondition()
        {
            if (Base.Dead) { return StringPlus.Create(Lang.Get("Health.Dead"), Theme.InfoColor); }
            if (Base.health?.hediffSet?.hediffs == null) { return null; }

            return GetBleedWarning() ?? GetTendWarning() ?? GetLifeThreateningWarning() ?? GetSicknessWarning() ?? GetIncapacitatedWarning() ?? StringPlus.Create(Lang.Get("Health.Stable"), Theme.GoodColor);
        }

        private StringPlus GetMindState()
        {
            if (Base.mindState.mentalStateHandler.InMentalState) { return StringPlus.Create(Base.mindState.mentalStateHandler.CurState.InspectLine, Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor : Theme.WarningColor); }

            if (Base.mindState.mentalBreaker.BreakExtremeIsImminent) { return StringPlus.Create(Lang.Get("Mood.ExtremeBreakImminent"), Theme.CriticalColor); }
            if (Base.mindState.mentalBreaker.BreakMajorIsImminent) { return StringPlus.Create(Lang.Get("Mood.MajorBreakImminent"), Theme.WarningColor); }
            if (Base.mindState.mentalBreaker.BreakMinorIsImminent) { return StringPlus.Create(Lang.Get("Mood.MinorBreakImminent"), Theme.WarningColor); }
            if (Base.needs.mood.CurLevel > 0.9f) { return StringPlus.Create(Lang.Get("Mood.Happy"), Theme.ExcellentColor); }
            return Base.needs.mood.CurLevel > 0.65f ? StringPlus.Create(Lang.Get("Mood.Content"), Theme.GoodColor) : StringPlus.Create(Lang.Get("Mood.Indifferent"), Theme.InfoColor);
        }

        private StringPlus GetInspiration()
        {
            if (!Base.Inspired) { return null; }

            var inspiration = Base.Inspiration.InspectLine;
            return StringPlus.Create(inspiration, Theme.ExcellentColor);
        }

        private StringPlus GetMentalCondition() => Base.needs.mood == null ? null : (GetInspiration() ?? GetMindState());

        private string GetEquipped()
        {
            var equipped = Base.equipment?.Primary?.LabelCap;
            return equipped == null ? null : Lang.Get("Info.Equipped", equipped.Bold());
        }
        private string GetActivity()
        {
            var activity = Base.jobs?.curDriver?.GetReport()?.TrimEnd('.').CapitalizeFirst().Bold();
            return activity == null ? null : Lang.Get("Info.Activity", activity);
        }
    }
}
