using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
    [StaticConstructorOnStartup]
    internal static class Access
    {
        public static readonly MethodInfo Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder = AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffGroupsInOrder", new[] { typeof(Pawn), typeof(bool) });
        public static readonly MethodInfo Method_RimWorld_HealthCardUtility_VisibleHediffs = AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffs", new[] { typeof(Pawn), typeof(bool) });
        public static readonly MethodInfo Method_RimWorld_InspectPaneUtility_InterfaceToggleTab = AccessTools.Method(typeof(InspectPaneUtility), "InterfaceToggleTab", new[] { typeof(InspectTabBase), typeof(IInspectPane) });
        public static readonly MethodInfo Method_RimWorld_Pawn_TrainingTracker_GetSteps = AccessTools.Method(typeof(Pawn_TrainingTracker), "GetSteps", new[] { typeof(TrainableDef) });
        public static readonly MethodInfo Method_RimWorld_SkillUI_GetSkillDescription = AccessTools.Method(typeof(SkillUI), "GetSkillDescription", new[] { typeof(SkillRecord) });

        public static readonly FieldInfo Field_RimWorld_AlertsReadout_ActiveAlerts = AccessTools.Field(typeof(AlertsReadout), "activeAlerts");
        public static readonly FieldInfo Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex = AccessTools.Field(typeof(InspectPaneUtility), "InspectTabButtonFillTex");
        public static readonly FieldInfo Field_Verse_LetterStack_LastTopYInt = AccessTools.Field(typeof(LetterStack), "lastTopYInt");
        public static readonly FieldInfo Field_Verse_LetterStack_Letters = AccessTools.Field(typeof(LetterStack), "letters");

        public static readonly MainButtonDef MainButtonDefOfRestrict = DefDatabase<MainButtonDef>.GetNamed("Restrict");
        public static readonly NeedDef NeedDefOfMood = DefDatabase<NeedDef>.GetNamed("Mood");
        public static readonly NeedDef NeedDefOfBeauty = DefDatabase<NeedDef>.GetNamed("Beauty");
        public static readonly NeedDef NeedDefOfComfort = DefDatabase<NeedDef>.GetNamed("Comfort");
        public static readonly NeedDef NeedDefOfOutdoors = DefDatabase<NeedDef>.GetNamed("Outdoors");
        public static readonly NeedDef NeedDefOfRoomSize = DefDatabase<NeedDef>.GetNamed("RoomSize");

        public static readonly TrainableDef TrainableDefOfHaul = DefDatabase<TrainableDef>.GetNamed("Haul");
        public static readonly TrainableDef TrainableDefOfRescue = DefDatabase<TrainableDef>.GetNamed("Rescue");
    }
}
