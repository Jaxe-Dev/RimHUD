using Verse;

namespace RimHUD.Data
{
    internal static class Lang
    {
        public static string Get(string key, params object[] args) => string.Format((Mod.Id + "." + key).Translate(), args);
        public static string GetIndexed(string key, int index)
        {
            var strings = Get(key).Split('|');

            if ((index >= strings.Length) || (index < 0)) { return "(INDEX?'" + key + "')"; }
            return strings[index];
        }
    }
}
