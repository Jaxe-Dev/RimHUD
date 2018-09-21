using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
    [StaticConstructorOnStartup]
    internal static class Access
    {
        public static readonly MethodInfo Method_RimWorld_InspectPaneUtility_InterfaceToggleTab = AccessTools.Method(typeof(InspectPaneUtility), "InterfaceToggleTab", new[] { typeof(InspectTabBase), typeof(IInspectPane) });

        public static readonly FieldInfo Field_RimWorld_AlertsReadout_ActiveAlerts = AccessTools.Field(typeof(AlertsReadout), "activeAlerts");
        public static readonly FieldInfo Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex = AccessTools.Field(typeof(InspectPaneUtility), "InspectTabButtonFillTex");
        public static readonly FieldInfo Field_Verse_LetterStack_Letters = AccessTools.Field(typeof(LetterStack), "letters");
        public static readonly FieldInfo Field_Verse_LetterStack_LastTopYInt = AccessTools.Field(typeof(LetterStack), "lastTopYInt");
    }
}
