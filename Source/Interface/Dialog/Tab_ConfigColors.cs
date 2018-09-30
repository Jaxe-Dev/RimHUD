using System;
using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;
using ColorOption = RimHUD.Data.ColorOption;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigColors : Tab
    {
        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Colors");
        public override Func<string> Tooltip { get; } = null;

        private Vector2 _scrollPosition = Vector2.zero;

        private ColorOption _selected;

        private readonly RangeOption _hue = new RangeOption(0, 0, 100, Lang.Get("Dialog_Config.Tab.Colors.Hue"));
        private string _hueText;
        private readonly RangeOption _saturation = new RangeOption(0, 0, 100, Lang.Get("Dialog_Config.Tab.Colors.Saturation"));
        private string _saturationText;
        private readonly RangeOption _lightness = new RangeOption(0, 0, 100, Lang.Get("Dialog_Config.Tab.Colors.Lightness"));
        private string _lightnessText;
        private readonly RangeOption _alpha = new RangeOption(0, 0, 100, Lang.Get("Dialog_Config.Tab.Colors.Alpha"));
        private string _alphaText;

        private Rect _viewRect = default(Rect);

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();

            var selected = _selected;

            l.BeginScrollView(hGrid[1], ref _scrollPosition, ref _viewRect);

            l.Label(Lang.Get("Theme.HudColors").Bold());
            l.GapLine();
            l.ColorOptionSelect(Theme.MainTextColor, ref _selected);
            l.ColorOptionSelect(Theme.CriticalColor, ref _selected);
            l.ColorOptionSelect(Theme.WarningColor, ref _selected);
            l.ColorOptionSelect(Theme.InfoColor, ref _selected);
            l.ColorOptionSelect(Theme.GoodColor, ref _selected);
            l.ColorOptionSelect(Theme.ExcellentColor, ref _selected);
            l.ColorOptionSelect(Theme.BarBackgroundColor, ref _selected);
            l.ColorOptionSelect(Theme.BarMainColor, ref _selected);
            l.ColorOptionSelect(Theme.BarLowColor, ref _selected);
            l.ColorOptionSelect(Theme.BarThresholdMinorColor, ref _selected);
            l.ColorOptionSelect(Theme.BarThresholdMajorColor, ref _selected);
            l.ColorOptionSelect(Theme.BarThresholdExtremeColor, ref _selected);
            l.ColorOptionSelect(Theme.LineColor, ref _selected);
            l.Gap();

            l.Label(Lang.Get("Theme.FactionColors").Bold());
            l.GapLine();
            l.ColorOptionSelect(Theme.FactionOwnColor, ref _selected);
            l.ColorOptionSelect(Theme.FactionAlliedColor, ref _selected);
            l.ColorOptionSelect(Theme.FactionIndependentColor, ref _selected);
            l.ColorOptionSelect(Theme.FactionHostileColor, ref _selected);
            l.ColorOptionSelect(Theme.FactionWildColor, ref _selected);
            l.Gap();

            l.Label(Lang.Get("Theme.SkillColors").Bold());
            l.GapLine();
            l.ColorOptionSelect(Theme.SkillDisabledColor, ref _selected);
            l.ColorOptionSelect(Theme.SkillMinorPassionColor, ref _selected);
            l.ColorOptionSelect(Theme.SkillMajorPassionColor, ref _selected);

            l.EndScrollView(ref _viewRect);

            l.Begin(hGrid[2]);

            if (_selected != null)
            {
                if (_selected != selected)
                {
                    GUI.FocusControl(null);
                    Color.RGBToHSV(_selected.Value, out var hue, out var saturation, out var lightness);
                    _hue.Value = hue.ToPercentageInt();
                    _saturation.Value = saturation.ToPercentageInt();
                    _lightness.Value = lightness.ToPercentageInt();
                    _alpha.Value = _selected.Value.a.ToPercentageInt();
                }

                l.Label(Lang.Get("Dialog_Config.Tab.Colors.Editor", _selected.Label).Bold());
                l.GapLine();
                l.RangeSliderEntry(_hue, ref _hueText, 1);
                l.RangeSliderEntry(_saturation, ref _saturationText, 2);
                l.RangeSliderEntry(_lightness, ref _lightnessText, 3);
                l.RangeSliderEntry(_alpha, ref _alphaText, 4);
                l.GapLine();
                var newColor = Color.HSVToRGB(_hue.Value.ToPercentageFloat(), _saturation.Value.ToPercentageFloat(), _lightness.Value.ToPercentageFloat());
                newColor.a = _alpha.Value.ToPercentageFloat();
                _selected.Value = newColor;

                var sampleRect = l.GetRect(30f);
                Widgets.DrawBoxSolid(sampleRect, _selected.Value);
            }

            l.End();
        }
    }
}
