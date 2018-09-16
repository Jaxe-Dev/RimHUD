using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal class Dialog_Config : WindowPlus
    {
        private const float GridPadding = 8f;
        private const float ButtonWidth = 120f;
        private const float ButtonHeight = 40f;
        private string _hudWidthText;
        private string _hudHeightText;
        private string _hudOffsetXText;
        private string _hudOffsetYText;

        private Dialog_Config() : base(Lang.Get("Dialog_Config.Title").Bold(), new Vector2(800f, 600f))
        {
            onlyOneOfTypeAllowed = true;
            absorbInputAroundWindow = false;
            preventCameraMotion = false;
            doCloseButton = false;
        }

        public static void Open() => Find.WindowStack.Add(new Dialog_Config());

        public override void PostClose() => Persistent.Save();

        protected override void DrawContent(Rect rect)
        {
            var vGrid = rect.GetVGrid(GridPadding, -1f, ButtonHeight);
            var hGrid = vGrid[1].GetHGrid(GridPadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(hGrid[1]);

            l.Label(Lang.Get("Theme.HudDimensions").Bold());
            l.RangeSliderEntry(Theme.HudWidth, ref _hudWidthText, 1);
            l.RangeSliderEntry(Theme.HudHeight, ref _hudHeightText, 2);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.HudOffset").Bold());
            l.RangeSlider(Theme.HudAnchor);
            l.RangeSliderEntry(Theme.HudOffsetX, ref _hudOffsetXText, 3);
            l.RangeSliderEntry(Theme.HudOffsetY, ref _hudOffsetYText, 4);
            l.GapLine();
            l.Gap();

            // Temporary
            l.Gap();
            var previousWrap = Text.WordWrap;
            Text.WordWrap = true;
            l.Label("The next customization update will focus on custom colors and textures then followed by allowing custom content.\n\n\nAs this mod is still in its early days please bear with the gradual introduction of features. If you like it so far then tell others about it and rate it on the Steam Workshop and any feedback is welcome.".Italic());
            Text.WordWrap = previousWrap;

            l.End();

            l.Begin(hGrid[2]);

            l.TextStyleEditor(Theme.RegularTextStyle);
            l.GapLine();
            l.Gap();
            l.TextStyleEditor(Theme.LargeTextStyle);
            l.GapLine();
            l.Gap();
            l.TextStyleEditor(Theme.SmallTextStyle);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.OtherOptions").Bold());
            Theme.LetterCompress = l.CheckboxLabeled(Lang.Get("Theme.LetterCompress"), Theme.LetterCompress, Lang.Get("Theme.LetterCompressDesc"));
            l.RangeSlider(Theme.LetterPadding, Theme.LetterCompress);
            l.GapLine();

            l.End();

            var buttonGrid = vGrid[2].GetHGrid(GridPadding, -1f, ButtonWidth, ButtonWidth);
            if (Widgets.ButtonText(buttonGrid[2], Lang.Get("Dialog_Config.SetToDefault"))) { SetToDefault(); }
            if (Widgets.ButtonText(buttonGrid[3], Lang.Get("Button.Close"))) { Close(); }
        }

        private static void SetToDefault() => Dialog_Alert.Open(Lang.Get("Alert.SetToDefault"), Dialog_Alert.Buttons.YesNo, Theme.ToDefault);
    }
}
