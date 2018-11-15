using RimHUD.Data;
using RimHUD.Extensions;
using RimHUD.Integration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Dialog_Presets : WindowPlus
    {
        private Rect _scrollView;
        private Vector2 _scrollPosition;

        private LayoutPreset _selected;

        private Dialog_Presets() : base(Lang.Get("Dialog_Preset.Title").Bold(), new Vector2(400f, 450f))
        {
            onlyOneOfTypeAllowed = true;
            absorbInputAroundWindow = false;
            preventCameraMotion = false;
            doCloseButton = false;
        }

        public static void Open() => Find.WindowStack.Add(new Dialog_Presets());

        protected override void DrawContent(Rect rect)
        {
            var vGrid = rect.GetVGrid(GUIPlus.LargePadding, -1f, 100f);
            var l = new ListingPlus();
            l.BeginScrollView(vGrid[1], ref _scrollPosition, ref _scrollView);

            foreach (var preset in LayoutPreset.List)
            {
                if (_selected == null) { _selected = preset; }
                if (l.RadioButton(preset.Label, _selected == preset)) { _selected = preset; }
            }

            l.EndScrollView(ref _scrollView);

            l.Begin(vGrid[2]);

            if (l.ButtonText(Lang.Get("Dialog_Preset.Overwrite"), enabled: _selected.IsUserMade)) { }
            if (l.ButtonText(Lang.Get("Dialog_Preset.Delete"), enabled: _selected.IsUserMade)) { }
            if (l.ButtonText(Lang.Get("Button.Close"))) { Close(); }

            l.End();
        }
    }
}
