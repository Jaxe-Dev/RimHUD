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
        private LayoutView _layout = new LayoutView(HudLayout.Docked);

        public override void Reset() => RefreshDesign();

        public override void Draw(Rect rect)
        {
            var topGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(topGrid[1]);
            if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Preset")))
            {
                var presets = new List<FloatMenuOption>
                {
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Preset.Default"), LoadDefaultPreset),
                            new FloatMenuOption("Dubs Bad Hygiene", () => TryLoadPreset(LayoutPresets.DubsBadHygiene, "DBH")),
                            new FloatMenuOption("A RimWorld of Magic", () => TryLoadPreset(LayoutPresets.RimWorldOfMagic, "RoM"))
                };

                Find.WindowStack.Add(new FloatMenu(presets));
            }

            l.End();

            l.Begin(topGrid[2]);
            if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Mode")))
            {
                var presets = new List<FloatMenuOption>
                {
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Mode.Docked"), () => SetDockedMode(true)),
                            new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Mode.Floating"), () => SetDockedMode(false))
                };

                Find.WindowStack.Add(new FloatMenu(presets));
            }

            var topHeight = l.CurHeight;
            l.End();

            var hGrid = rect.GetVGrid(GUIPlus.LargePadding, topHeight, -1f)[2].GetHGrid(GUIPlus.LargePadding, -1f, 300f);

            l.Begin(hGrid[1]);
            l.Label(Lang.Get("Dialog_Config.Tab.Content.Layout", Lang.Get("Dialog_Config.Tab.Content.Mode." + (_dockedMode ? "Docked" : "Floating"))));
            _layout.Draw(l.GetRect(200f));

            l.Gap();
            l.Gap();

            l.Label("This tab is incomplete but should be available in the next update.".Italic(), color: Color.yellow);

            l.End();

            /*
            l.Begin(hGrid[2]);
            l.Label(Lang.Get("Dialog_Config.Tab.Content.Components"));
            if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.AddContainer")))
            {
                var containers = new List<FloatMenuOption>
                {
                            new FloatMenuOption()
                }
            }
            l.End();
        */
        }

        private void SetDockedMode(bool value)
        {
            _dockedMode = value;
            RefreshDesign();
        }

        private void RefreshDesign() => _layout = new LayoutView(_dockedMode ? HudLayout.Docked : HudLayout.Floating);

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
