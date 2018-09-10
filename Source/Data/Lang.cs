using Verse;

namespace RimHUD.Data
{
    internal static class Lang
    {
        public static string Get(string key, params object[] args) => string.Format((Mod.Id + "." + key).Translate(), args);
    }
}
