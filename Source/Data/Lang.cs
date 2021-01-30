using Verse;

namespace RimHUD.Data
{
    internal static class Lang
    {
        public static bool HasKey(string key) => LanguageDatabase.activeLanguage.HaveTextForKey(Mod.Id + "." + key);

        public static string Get(string key, params object[] args)
        {
            try { return string.Format((Mod.Id + "." + key).Translate(), args); }
            catch { return $"<TranslationError:{key}>"; }
        }

        public static string GetIndexed(string key, int index)
        {
            var strings = Get(key).Split('|');

            if (index >= strings.Length || index < 0) { return "(INDEX?'" + key + "')"; }
            return strings[index];
        }

        public static string CombineWords(string first, string second) => string.Concat(first, " ", second).Trim();

        public static string AdjectiveNoun(string adjective, string noun) => Get("Language.AdjectiveNounOrder", adjective, noun).Trim();
    }
}
