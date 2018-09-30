using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimHUD.Interface;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimHUD.Data
{
    internal class PawnModel
    {
        public static PawnModel Selected => GetSelected();

        public Pawn Base { get; }

        public string Name => Base.Name?.ToStringFull.CapitalizeFirst() ?? Base.LabelCap;
        public string GenderAndAge => GetGenderAndAge();
        public Color FactionRelationColor => GetFactionRelationColor();
        public string RelationKindAndFaction => GetRelationKindAndFaction();

        public StringPlus HealthCondition => GetHealthCondition();
        public StringPlus MentalCondition => GetMindCondition();

        public bool IsAnimal => Base.RaceProps.Animal;

        public bool HasNeeds => Base.needs != null;
        public float Health => Base.health?.summaryHealth?.SummaryHealthPercent ?? Layout.NullValue;
        public float Rest => Base.needs?.rest?.CurLevelPercentage ?? Layout.NullValue;
        public float Food => Base.needs?.food?.CurLevelPercentage ?? Layout.NullValue;
        public float Recreation => Base.needs?.joy?.CurLevelPercentage ?? Layout.NullValue;
        public float Mood => Base.needs?.mood?.CurLevelPercentage ?? Layout.NullValue;
        public float MoodThresholdMinor => Base.mindState?.mentalBreaker?.BreakThresholdMinor ?? Layout.NullValue;
        public float MoodThresholdMajor => Base.mindState?.mentalBreaker?.BreakThresholdMajor ?? Layout.NullValue;
        public float MoodThresholdExtreme => Base.mindState?.mentalBreaker?.BreakThresholdExtreme ?? Layout.NullValue;

        public bool HasSkills => Base.skills != null;
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

        public bool HasTraining => Base.training != null;
        public TrainingModel Tameness => GetTrainingModel(TrainableDefOf.Tameness);
        public TrainingModel Obedience => GetTrainingModel(TrainableDefOf.Obedience);
        public TrainingModel Release => GetTrainingModel(TrainableDefOf.Release);
        public TrainingModel Rescue => GetTrainingModel(Access.TrainableDefOfRescue);
        public TrainingModel Haul => GetTrainingModel(Access.TrainableDefOfHaul);

        public string Activity => GetActivity();
        public string Queued => GetQueued();

        public string Equipped => GetEquipped();
        public string Carrying => GetCarrying();

        public string CompInfo => GetCompInfo();

        public Func<string> HealthTooltip => GetHealthTooltip;
        public Func<string> MoodTooltip => GetMoodTooltip;
        public Func<string> BioTooltip => GetBioTooltip;

        private PawnModel(Pawn pawn) => Base = pawn;

        private static PawnModel GetSelected()
        {
            var selected = State.SelectedPawn;
            return selected == null ? null : new PawnModel(selected);
        }

        private SkillModel GetSkillModel(SkillDef def) => new SkillModel(Base, def);
        private TrainingModel GetTrainingModel(TrainableDef def) => new TrainingModel(Base, def);

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
            if (Base.kindDef.race.tradeTags?.Contains("AnimalFarm") ?? false) { return Lang.Get("Creature.Farm"); }
            if (Base.RaceProps.herdAnimal) { return Lang.Get("Creature.Herd"); }
            if (Base.kindDef.race.tradeTags?.Contains("AnimalInsect") ?? false) { return Lang.Get("Creature.Insect"); }

            return Lang.Get("Creature.Tame");
        }

        private Color GetFactionRelationColor()
        {
            if (Base.Faction == null) { return IsAnimal ? Theme.FactionWildColor.Value : Theme.FactionIndependentColor.Value; }
            if (Base.Faction.IsPlayer) { return Theme.FactionOwnColor.Value; }

            if (Base.Faction.PlayerRelationKind == FactionRelationKind.Hostile) { return Theme.FactionHostileColor.Value; }
            return Base.Faction.PlayerRelationKind == FactionRelationKind.Ally ? Theme.FactionAlliedColor.Value : Theme.FactionIndependentColor.Value;
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

            return StringPlus.Create(text, Theme.CriticalColor.Value);
        }

        private StringPlus GetTendWarning()
        {
            var count = Base.health.hediffSet.hediffs.Count(hediff => hediff.TendableNow());
            if (count == 0) { return null; }

            var text = count == 1 ? Lang.Get("Health.Tend", count) : Lang.Get("Health.TendPlural", count);
            var hasLifeThreateningCondition = GetLifeThreateningWarning();

            return StringPlus.Create(text, hasLifeThreateningCondition?.Color ?? Theme.WarningColor.Value);
        }

        private StringPlus GetLifeThreateningWarning()
        {
            var threats = Base.health.hediffSet.hediffs.Where(hediff => hediff.CurStage?.lifeThreatening ?? false).ToArray();
            var count = threats.Count();
            if (count == 0) { return null; }

            var worst = threats.MinBy(hediff => hediff.CurStage.deathMtbDays);
            var text = count == 1 ? Lang.Get("Health.Threat", worst.LabelCap) : Lang.Get("Health.ThreatPlural", worst.LabelCap, count);

            return StringPlus.Create(text, Theme.CriticalColor.Value);
        }

        private StringPlus GetSicknessWarning()
        {
            var sicknesses = Base.health.hediffSet.hediffs.Where(hediff => hediff.def.makesSickThought).ToArray();
            var count = sicknesses.Count();
            if (count == 0) { return null; }

            var worst = sicknesses.MaxBy(hediff => hediff.PainFactor);

            var text = count == 1 ? Lang.Get("Health.Sick", worst.LabelCap) : Lang.Get("Health.SickPlural", worst.LabelCap, count);
            return StringPlus.Create(text, Theme.WarningColor.Value);
        }

        private StringPlus GetIncapacitatedWarning() => !Base.health.Downed ? null : StringPlus.Create(Lang.Get("Health.Incapacitated"), Theme.WarningColor.Value);

        private StringPlus GetHealthCondition()
        {
            if (Base.Dead) { return StringPlus.Create(Lang.Get("Health.Dead"), Theme.InfoColor.Value); }
            if (Base.health?.hediffSet?.hediffs == null) { return null; }

            return GetBleedWarning() ?? GetTendWarning() ?? GetLifeThreateningWarning() ?? GetSicknessWarning() ?? GetIncapacitatedWarning() ?? StringPlus.Create(Lang.Get("Health.Stable"), Theme.GoodColor.Value);
        }

        private StringPlus GetMindState()
        {
            if (Base.mindState?.mentalStateHandler == null) { return null; }
            if (Base.mindState.mentalStateHandler.InMentalState) { return StringPlus.Create(Base.mindState.mentalStateHandler.CurState.InspectLine, Base.mindState.mentalStateHandler.CurState.def.IsAggro || Base.mindState.mentalStateHandler.CurState.def.IsExtreme ? Theme.CriticalColor.Value : Theme.WarningColor.Value); }

            if ((Base.needs?.mood == null) || (Base.mindState?.mentalBreaker == null)) { return null; }

            if (Base.mindState.mentalBreaker.BreakExtremeIsImminent) { return StringPlus.Create(Lang.Get("Mood.ExtremeBreakImminent"), Theme.CriticalColor.Value); }
            if (Base.mindState.mentalBreaker.BreakMajorIsImminent) { return StringPlus.Create(Lang.Get("Mood.MajorBreakImminent"), Theme.WarningColor.Value); }
            if (Base.mindState.mentalBreaker.BreakMinorIsImminent) { return StringPlus.Create(Lang.Get("Mood.MinorBreakImminent"), Theme.WarningColor.Value); }

            var inspiration = GetInspiration();
            if (inspiration != null) { return inspiration; }

            if (Base.needs.mood.CurLevel > 0.9f) { return StringPlus.Create(Lang.Get("Mood.Happy"), Theme.ExcellentColor.Value); }
            return Base.needs.mood.CurLevel > 0.65f ? StringPlus.Create(Lang.Get("Mood.Content"), Theme.GoodColor.Value) : StringPlus.Create(Lang.Get("Mood.Indifferent"), Theme.InfoColor.Value);
        }

        private StringPlus GetInspiration()
        {
            if (!Base.Inspired) { return null; }

            var inspiration = Base.Inspiration.InspectLine;
            return StringPlus.Create(inspiration, Theme.ExcellentColor.Value);
        }

        private StringPlus GetMindCondition() => GetMindState();

        private string GetActivity()
        {
            var lord = Base.GetLord()?.LordJob?.GetReport()?.CapitalizeFirst();
            var jobText = Base.jobs?.curDriver?.GetReport()?.TrimEnd('.').CapitalizeFirst();

            var target = (Base.jobs?.curJob?.def == JobDefOf.AttackStatic) || (Base.jobs?.curJob?.def == JobDefOf.Wait_Combat) ? Base.TargetCurrentlyAimingAt.Thing?.LabelCap : null;
            var activity = target == null ? lord.NullOrEmpty() ? jobText : $"{lord} ({jobText})" : Lang.Get("Info.Attacking", target);

            return activity == null ? null : Lang.Get("Info.Activity", activity.Bold());
        }

        private string GetQueued()
        {
            if ((Base.jobs?.curJob == null) || (Base.jobs.jobQueue.Count == 0)) { return null; }

            var queued = Base.jobs.jobQueue[0].job.GetReport(Base)?.TrimEnd('.').CapitalizeFirst().Bold();
            var remaining = Base.jobs.jobQueue.Count - 1;
            if (remaining > 0) { queued += $" (+{remaining})"; }

            return queued == null ? null : Lang.Get("Info.Queued", queued);
        }

        private string GetCarrying()
        {
            var carried = Base.carryTracker?.CarriedThing?.LabelCap;
            return carried == null ? null : Lang.Get("Info.Carrying", carried.Bold());
        }

        private string GetEquipped()
        {
            var equipped = Base.equipment?.Primary?.LabelCap;

            return RestraintsUtility.ShouldShowRestraintsInfo(Base) ? "InRestraints".Translate() : equipped == null ? null : Lang.Get("Info.Equipped", equipped.Bold());
        }

        private string GetCompInfo()
        {
            var list = new List<string>();
            foreach (var comp in Base.AllComps)
            {
                var text = comp.CompInspectStringExtra();
                if (!text.NullOrEmpty()) { list.Add(text.Replace('\n', ' ')); }
            }
            var comps = list.ToArray();
            return comps.Length > 0 ? comps.ToCommaList() : null;
        }

        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss) => (IEnumerable<IGrouping<BodyPartRecord, Hediff>>) Access.Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder.Invoke(null, new object[] { pawn, showBloodLoss });

        private string GetHealthTooltip()
        {
            if (Base.health?.hediffSet?.hediffs == null) { return null; }

            var builder = new StringBuilder();

            foreach (var hediffs in VisibleHediffGroupsInOrder(Base, true))
            {
                foreach (var hediff in hediffs) { builder.AppendLine(GetHealthTooltipLine(hediff)); }
            }

            return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
        }

        private static string GetHealthTooltipLine(Hediff hediff)
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

        private string GetMoodTooltip()
        {
            if (Base.needs?.mood?.thoughts == null) { return null; }

            var thoughts = new List<Thought>();
            PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(Base.needs.mood, thoughts);

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
            if (Base.Inspired) { builder.AppendLine(Base.Inspiration.InspectLine.Color(Theme.ExcellentColor.Value)); }

            return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
        }

        private string GetBioTooltip()
        {
            if (Base.story == null) { return null; }

            var builder = new StringBuilder();

            var title = Base.story?.TitleCap;
            if (title != null) { builder.TryAppendLine(Lang.Get("Bio.Title", title)); }
            var faction = Base.Faction?.Name;
            if (faction != null) { builder.TryAppendLine(Lang.Get("Bio.Faction", faction)); }

            builder.AppendLine();

            var childhood = Base.story.GetBackstory(BackstorySlot.Childhood);
            var childhoodText = childhood == null ? null : $"{"Childhood".Translate()}: {childhood.TitleCapFor(Base.gender)}";
            var adulthood = Base.story.GetBackstory(BackstorySlot.Adulthood);
            var adulthoodText = adulthood == null ? null : $"{"Adulthood".Translate()}: {adulthood.TitleCapFor(Base.gender)}";

            builder.TryAppendLine(childhoodText);
            builder.TryAppendLine(adulthoodText);

            builder.AppendLine();

            var traits = Base.story.traits.allTraits.Count > 0 ? "Traits".Translate() + ": " + Base.story.traits.allTraits.Select(trait => trait.LabelCap).ToCommaList(true) : null;
            builder.TryAppendLine(traits);

            builder.AppendLine();

            var disabledWork = Base.story.CombinedDisabledWorkTags;
            var incapable = disabledWork == WorkTags.None ? null : "IncapableOf".Translate() + ": " + disabledWork.GetAllSelectedItems<WorkTags>().Where(tag => tag != WorkTags.None).Select(tag => tag.LabelTranslated().CapitalizeFirst()).ToCommaList(true);

            builder.TryAppendLine(incapable?.Color(Theme.CriticalColor.Value));

            return builder.Length > 0 ? builder.ToStringTrimmed().Size(Theme.RegularTextStyle.ActualSize) : null;
        }
    }
}
