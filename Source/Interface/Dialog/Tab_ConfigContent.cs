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

        private bool _dockedMode = true;
        private LayoutDesign _design = new LayoutDesign(HudLayout.Docked);

        public override void Reset() => RefreshDesign();

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, 200f);
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

            l.Label(Lang.Get("Dialog_Config.Tab.Content.Layout.Mode").Bold());
            if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Layout.Mode." + (_dockedMode ? "Docked" : "Floating"))))
            {
                var presets = new List<FloatMenuOption>
                {
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Layout.Mode.Docked"), () => SetDockedMode(true)),
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Layout.Mode.Floating"), () => SetDockedMode(false))
                };

                Find.WindowStack.Add(new FloatMenu(presets));
            }

            l.GapLine();
            l.Gap();

            _design.Draw(l.GetRect(200f));

            l.GapLine();
            l.Gap();
            l.Gap();

            l.Label("This tab is incomplete but should be available in the next update.".Italic(), color: Color.yellow);

            l.End();
        }

        private void SetDockedMode(bool value)
        {
            _dockedMode = value;
            RefreshDesign();
        }

        private void RefreshDesign() => _design = new LayoutDesign(_dockedMode ? HudLayout.Docked : HudLayout.Floating);

        private void LoadDefaultPreset()
        {
            HudLayout.LoadDefault();
            Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.DefaultLoaded"));
            RefreshDesign();
        }

        private void TryLoadPreset(ExternalMod mod, string id)
        {
            if (!mod.IsActive)
            {
                Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.Invalid", mod.Name));
                return;
            }

            HudLayout.LoadPreset(id);
            Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Layout.Preset.Loaded", mod.Name));
            RefreshDesign();
        }
    }
}
