using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace RimHUD.Patch
{
    [StaticConstructorOnStartup]
    internal static class Access
    {
        private static readonly FieldInfo Field_RimWorld_AlertsReadout_ActiveAlerts = AccessTools.Field(typeof(AlertsReadout), "activeAlerts");
        private static readonly FieldInfo Field_Verse_LetterStack_Letters = AccessTools.Field(typeof(LetterStack), "letters");
        private static readonly FieldInfo Field_Verse_LetterStack_LastTopYInt = AccessTools.Field(typeof(LetterStack), "lastTopYInt");

        public static List<Alert> Field_RimWorld_AlertsReadout_ActiveAlerts_Get() => (List<Alert>) Field_RimWorld_AlertsReadout_ActiveAlerts.GetValue(((UIRoot_Play) Find.UIRoot).alerts);
        public static List<Letter> Field_Verse_LetterStack_Letters_Get() => (List<Letter>) Field_Verse_LetterStack_Letters.GetValue(Find.LetterStack);
        public static void Field_Verse_LetterStack_LastTopYInt_Set(float value) => Field_Verse_LetterStack_LastTopYInt.SetValue(Find.LetterStack, value);
    }
}
