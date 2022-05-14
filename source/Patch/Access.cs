using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
  [StaticConstructorOnStartup]
  internal static class Access
  {
    public static FastInvokeHandler Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder { get; private set; }
    public static FastInvokeHandler Method_RimWorld_HealthCardUtility_VisibleHediffs { get; private set; }
    public static FastInvokeHandler Method_RimWorld_InspectPaneUtility_InterfaceToggleTab { get; private set; }
    public static FastInvokeHandler Method_RimWorld_InspectPaneUtility_ToggleTab { get; private set; }
    public static FastInvokeHandler Method_RimWorld_Pawn_TrainingTracker_GetSteps { get; private set; }
    public static FastInvokeHandler Method_RimWorld_SkillUI_GetSkillDescription { get; private set; }

    public static FieldInfo Field_RimWorld_AlertsReadout_ActiveAlerts { get; private set; }
    public static FieldInfo Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex { get; private set; }
    public static FieldInfo Field_Verse_LetterStack_LastTopYInt { get; private set; }
    public static FieldInfo Field_Verse_LetterStack_Letters { get; private set; }

    public static MainButtonDef MainButtonDefOfRestrict { get; private set; }
    public static MainButtonDef MainButtonDefOfWork { get; private set; }

    public static NeedDef NeedDefOfMood { get; private set; }
    public static NeedDef NeedDefOfSuppression { get; private set; }
    public static NeedDef NeedDefOfBeauty { get; private set; }
    public static NeedDef NeedDefOfComfort { get; private set; }
    public static NeedDef NeedDefOfOutdoors { get; private set; }
    public static NeedDef NeedDefOfRoomSize { get; private set; }

    public static TrainableDef TrainableDefOfHaul { get; private set; }
    public static TrainableDef TrainableDefOfRescue { get; private set; }

    public static void Initialize()
    {
      Method_RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder = MethodInvoker.GetHandler(AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffGroupsInOrder", new[] { typeof(Pawn), typeof(bool) }));
      Method_RimWorld_HealthCardUtility_VisibleHediffs = MethodInvoker.GetHandler(AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffs", new[] { typeof(Pawn), typeof(bool) }));
      Method_RimWorld_InspectPaneUtility_InterfaceToggleTab = MethodInvoker.GetHandler(AccessTools.Method(typeof(InspectPaneUtility), "InterfaceToggleTab", new[] { typeof(InspectTabBase), typeof(IInspectPane) }));
      Method_RimWorld_InspectPaneUtility_ToggleTab = MethodInvoker.GetHandler(AccessTools.Method(typeof(InspectPaneUtility), "ToggleTab", new[] { typeof(InspectTabBase), typeof(IInspectPane) }));
      Method_RimWorld_Pawn_TrainingTracker_GetSteps = MethodInvoker.GetHandler(AccessTools.Method(typeof(Pawn_TrainingTracker), "GetSteps", new[] { typeof(TrainableDef) }));
      Method_RimWorld_SkillUI_GetSkillDescription = MethodInvoker.GetHandler(AccessTools.Method(typeof(SkillUI), "GetSkillDescription", new[] { typeof(SkillRecord) }));

      Field_RimWorld_AlertsReadout_ActiveAlerts = AccessTools.Field(typeof(AlertsReadout), "activeAlerts");
      Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex = AccessTools.Field(typeof(InspectPaneUtility), "InspectTabButtonFillTex");
      Field_Verse_LetterStack_LastTopYInt = AccessTools.Field(typeof(LetterStack), "lastTopYInt");
      Field_Verse_LetterStack_Letters = AccessTools.Field(typeof(LetterStack), "letters");

      MainButtonDefOfRestrict = DefDatabase<MainButtonDef>.GetNamed("Schedule");
      MainButtonDefOfWork = DefDatabase<MainButtonDef>.GetNamed("Work");

      NeedDefOfMood = DefDatabase<NeedDef>.GetNamed("Mood");
      NeedDefOfBeauty = DefDatabase<NeedDef>.GetNamed("Beauty");
      NeedDefOfComfort = DefDatabase<NeedDef>.GetNamed("Comfort");
      NeedDefOfOutdoors = DefDatabase<NeedDef>.GetNamed("Outdoors");
      NeedDefOfRoomSize = DefDatabase<NeedDef>.GetNamed("RoomSize");

      TrainableDefOfHaul = DefDatabase<TrainableDef>.GetNamed("Haul");
      TrainableDefOfRescue = DefDatabase<TrainableDef>.GetNamed("Rescue");

      if (ModsConfig.IdeologyActive) { NeedDefOfSuppression = DefDatabase<NeedDef>.GetNamed("Suppression", false); }
    }

    public class StaticMethodHandler
    {
      private readonly Type _type;
      private readonly FastInvokeHandler _handler;

      public StaticMethodHandler(Type type, string name, Type[] parameters = null, Type[] generics = null)
      {
        _type = type;
        _handler = MethodInvoker.GetHandler(AccessTools.Method(type, name, parameters, generics));
      }

      public void Invoke(params object[] parameters) => Invoke<object>();
      public T Invoke<T>(params object[] parameters) => (T) _handler.Invoke(_type, parameters);
    }
  }
}
