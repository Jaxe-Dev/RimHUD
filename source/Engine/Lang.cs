using System;
using Verse;

namespace RimHUD.Engine;

public static class Lang
{
  public static bool HasKey(string key) => LanguageDatabase.activeLanguage!.HaveTextForKey($"{Mod.Id}.{key}");

  public static string Get(string key) => $"{Mod.Id}.{key}".TranslateSimple();

  public static string Get(string key, params object?[] args)
  {
    try { return string.Format($"{Mod.Id}.{key}".TranslateSimple(), args); }
    catch (Exception exception)
    {
      Report.Error($"Translation key '{key}' threw the following exception: {exception.Message}");
      return Get(key);
    }
  }

  public static string GetIndexed(string key, int index) => Get(key).Split('|')?[index] ?? "";

  public static string AdjectiveNoun(string? adjective, string? noun) => Get("Language.AdjectiveNounOrder", adjective, noun).Trim();
}
