using System.Collections.Generic;
using RimHUD.Data;
using RimHUD.Integration;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigContent : Tab
    {
        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Content");
        public override TipSignal? Tooltip { get; } = null;

        public override void Reset()
        { }

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(hGrid[1]);

            l.Label(Lang.Get("Dialog_Config.Tab.Content.Layout").Bold());

            if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset")))
            {
                var presets = new List<FloatMenuOption>
                {
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.Default"), LoadDefaultPreset),
                            new FloatMenuOption("Dubs Bad Hygiene", () => TryLoadPreset(LayoutPresets.DubsBadHygiene, "DBH")),
                            new FloatMenuOption("A RimWorld of Magic", () => TryLoadPreset(LayoutPresets.RimWorldOfMagic, "RoM"))
                };

                Find.WindowStack.Add(new FloatMenu(presets));
            }

            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Alert.MoreComingSoon").Italic());
            l.Gap();
            l.Gap();
            var wrap = Text.WordWrap;
            Text.WordWrap = true;
            l.Label("A GUI for editing the layout and content is being developed but in the meanwhile advanced users can edit the layout in Docked/Floating.xml files found in the config folder.\n\nFor custom needs use <Need DefName=\"someNeed\">\nFor custom skills use <Skill DefName=\"someSkill\">");
            Text.WordWrap = wrap;

            l.End();
        }

        private static void LoadDefaultPreset()
        {
            HudLayout.LoadDefault();
            Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.DefaultLoaded"));
        }

        private static void TryLoadPreset(ExternalMod mod, string id)
        {
            if (!mod.IsActive)
            {
                Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.Invalid", mod.Name));
                return;
            }

            HudLayout.LoadPreset(id);
            Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.Loaded", mod.Name));
        }
    }
}
