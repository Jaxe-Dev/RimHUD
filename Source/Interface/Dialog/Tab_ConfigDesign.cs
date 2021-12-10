using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigDesign : Tab
    {
        private string _hudWidthText;
        private string _hudHeightText;
        private string _hudOffsetXText;
        private string _hudOffsetYText;
        private string _inspectPaneHeightText;

        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Design");
        public override TipSignal? Tooltip { get; } = null;

        public override void Reset()
        { }

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(hGrid[1]);

            l.RangeSlider(Theme.RefreshRate);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.HudPosition").Bold());
            l.BoolToggle(Theme.HudDocked);
            l.RangeSlider(Theme.HudAnchor, !Theme.HudDocked.Value);
            l.RangeSliderEntry(Theme.HudOffsetX, ref _hudOffsetXText, 1, !Theme.HudDocked.Value);
            l.RangeSliderEntry(Theme.HudOffsetY, ref _hudOffsetYText, 2, !Theme.HudDocked.Value);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.HudDimensions").Bold());
            l.RangeSliderEntry(Theme.HudWidth, ref _hudWidthText, 3, !Theme.HudDocked.Value);
            l.RangeSliderEntry(Theme.HudHeight, ref _hudHeightText, 4, !Theme.HudDocked.Value);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.InspectPane").Bold());
            l.BoolToggle(Theme.InspectPaneTabModify);
            l.BoolToggle(Theme.InspectPaneTabAddLog, Theme.InspectPaneTabModify.Value && !Theme.HudDocked.Value);
            l.RangeSliderEntry(Theme.InspectPaneHeight, ref _inspectPaneHeightText, 5, Theme.InspectPaneTabModify.Value);
            l.RangeSlider(Theme.InspectPaneTabWidth, Theme.InspectPaneTabModify.Value);
            l.RangeSlider(Theme.InspectPaneMinTabs, Theme.InspectPaneTabModify.Value);

            l.End();

            l.Begin(hGrid[2]);

            l.TextStyleEditor(Theme.RegularTextStyle);
            l.RangeSlider(Theme.LabelWidth);
            l.RangeSlider(Theme.ValueWidth);
            l.GapLine();
            l.Gap();

            l.TextStyleEditor(Theme.LargeTextStyle, !Theme.HudDocked.Value);
            l.GapLine();
            l.Gap();

            l.TextStyleEditor(Theme.SmallTextStyle);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Theme.OtherOptions").Bold());
            l.BoolToggle(Theme.ShowDecimals);
            l.BoolToggle(Theme.LetterCompress);
            l.RangeSlider(Theme.LetterPadding, Theme.LetterCompress.Value);

            l.End();
        }
    }
}
