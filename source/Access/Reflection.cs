using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimHUD.Access;

public static class Reflection
{
  public static readonly FastInvokeHandler RimWorld_HealthCardUtility_VisibleHediffGroupsInOrder = MethodInvoker.GetHandler(AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffGroupsInOrder", [typeof(Pawn), typeof(bool)]));
  public static readonly FastInvokeHandler RimWorld_InspectPaneUtility_InterfaceToggleTab = MethodInvoker.GetHandler(AccessTools.Method(typeof(InspectPaneUtility), "InterfaceToggleTab", [typeof(InspectTabBase), typeof(IInspectPane)]));
  public static readonly FastInvokeHandler RimWorld_InspectPaneUtility_ToggleTab = MethodInvoker.GetHandler(AccessTools.Method(typeof(InspectPaneUtility), "ToggleTab", [typeof(InspectTabBase), typeof(IInspectPane)]));
  public static readonly FastInvokeHandler RimWorld_Pawn_TrainingTracker_GetSteps = MethodInvoker.GetHandler(AccessTools.Method(typeof(Pawn_TrainingTracker), "GetSteps", [typeof(TrainableDef)]));
  public static readonly FastInvokeHandler RimWorld_SkillUI_GetSkillDescription = MethodInvoker.GetHandler(AccessTools.Method(typeof(SkillUI), "GetSkillDescription", [typeof(SkillRecord)]));
  public static readonly FastInvokeHandler Verse_Widgets_CheckPlayDragSliderSound = MethodInvoker.GetHandler(AccessTools.Method(typeof(Widgets), "CheckPlayDragSliderSound"));

  public static readonly FieldInfo RimWorld_InspectPaneUtility_InspectTabButtonFillTex = AccessTools.Field(typeof(InspectPaneUtility), "InspectTabButtonFillTex");
  public static readonly FieldInfo Verse_LetterStack_LastTopYInt = AccessTools.Field(typeof(LetterStack), "lastTopYInt");
  public static readonly FieldInfo Verse_LetterStack_Letters = AccessTools.Field(typeof(LetterStack), "letters");
  public static readonly FieldInfo Verse_LetterStack_TmpBundledLetters = AccessTools.Field(typeof(LetterStack), "tmpBundledLetters");
  public static readonly FieldInfo Verse_Widgets_RangeControlTextColor = AccessTools.Field(typeof(Widgets), "RangeControlTextColor");
  public static readonly FieldInfo Verse_Widgets_SliderDraggingId = AccessTools.Field(typeof(Widgets), "sliderDraggingID");
  public static readonly FieldInfo Verse_Widgets_SliderHandle = AccessTools.Field(typeof(Widgets), "SliderHandle");
  public static readonly FieldInfo Verse_Widgets_SliderRailAtlas = AccessTools.Field(typeof(Widgets), "SliderRailAtlas");

  public static ModContentPack? GetModFromAssemblyName(string name)
  {
    var possible = LoadedModManager.RunningMods.FirstOrDefault(mod => mod.assemblies!.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == name) is not null);
    if (possible is null || Equals(possible, Mod.ContentPack)) { return null; }

    return possible;
  }

  public static T GetValue<T>(this FieldInfo self, object instance) => (T)self.GetValue(instance);
  public static T GetValueStatic<T>(this FieldInfo self) => (T)self.GetValue(null);
  public static void SetValueStatic<T>(this FieldInfo self, T value) => self.SetValue(null, value);
  public static T Invoke<T>(this FastInvokeHandler self, object target, params object[] parameters) => (T)self.Invoke(target, parameters);
  public static T InvokeStatic<T>(this FastInvokeHandler self, params object[] parameters) => (T)self.Invoke(null, parameters);
  public static void InvokeStatic(this FastInvokeHandler self, params object[] parameters) => self.Invoke(null, parameters);
}
