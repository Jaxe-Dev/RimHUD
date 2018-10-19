using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    [StaticConstructorOnStartup]
    internal static class Textures
    {
        public static readonly Texture2D ToggleIcon = LoadTexture("ToggleIcon");
        public static readonly Texture2D ConfigIcon = LoadTexture("ConfigIcon");
        public static readonly Texture2D SelfTendOnIcon = LoadTexture("SelfTendOnIcon");
        public static readonly Texture2D SelfTendOffIcon = LoadTexture("SelfTendOffIcon");

        public static readonly Texture2D InspectTabButtonFillTex = (Texture2D) Access.Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex.GetValue(null);
        public static readonly Texture2D SelectOverlappingNextTex = ContentFinder<Texture2D>.Get("UI/Buttons/SelectNextOverlapping");

        private static Texture2D LoadTexture(string key) => ContentFinder<Texture2D>.Get(Mod.Id + "/" + key);
    }
}
