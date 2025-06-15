using UnityEngine;
using Verse;

namespace RimHUD.Interface;

[StaticConstructorOnStartup]
public static class TexturesPlus
{
  public static Texture2D ToggleIcon { get; private set; } = LoadTexture("ToggleIcon");
  public static Texture2D ConfigIcon { get; private set; } = LoadTexture("ConfigIcon");
  public static Texture2D TutorialConfigIcon { get; private set; } = LoadTexture("TutorialConfigIcon");
  public static Texture2D SelfTendOnIcon { get; private set; } = LoadTexture("SelfTendOnIcon");
  public static Texture2D SelfTendOffIcon { get; private set; } = LoadTexture("SelfTendOffIcon");

  private static Texture2D LoadTexture(string key) => ContentFinder<Texture2D>.Get($"{Mod.Id}/{key}");
}
