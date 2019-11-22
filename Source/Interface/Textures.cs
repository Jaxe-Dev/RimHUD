using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    [StaticConstructorOnStartup]
    internal static class Textures
    {
        public static Texture2D ToggleIcon { get; private set; }
        public static Texture2D ConfigIcon { get; private set; }
        public static Texture2D SelfTendOnIcon { get; private set; }
        public static Texture2D SelfTendOffIcon { get; private set; }

        public static Texture2D InspectTabButtonFill { get; private set; }
        public static Texture2D SelectOverlappingNext { get; private set; }

        public static Texture2D Reveal { get; private set; }
        public static Texture2D Collapse { get; private set; }

        public static void Initialize()
        {
            ToggleIcon = LoadTexture("ToggleIcon");
            ConfigIcon = LoadTexture("ConfigIcon");
            SelfTendOnIcon = LoadTexture("SelfTendOnIcon");
            SelfTendOffIcon = LoadTexture("SelfTendOffIcon");

            InspectTabButtonFill = (Texture2D) Access.Field_RimWorld_InspectPaneUtility_InspectTabButtonFillTex.GetValue(null);
            SelectOverlappingNext = ContentFinder<Texture2D>.Get("UI/Buttons/SelectNextOverlapping");

            Reveal = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal");
            Collapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse");
        }

        private static Texture2D LoadTexture(string key) => ContentFinder<Texture2D>.Get(Mod.Id + "/" + key);
    }
}
