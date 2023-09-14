using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.Dialog.Tabs
{
  public sealed class Tab_ConfigDesign : Tab
  {
    public override string Label { get; } = Lang.Get("Interface.Dialog_Config.Tab_Design");

    private string? _hudWidthText;
    private string? _hudHeightText;
    private string? _hudOffsetXText;
    private string? _hudOffsetYText;
    private string? _inspectPaneHeightText;

    public override void Reset()
    { }

    public override void Draw(Rect rect)
    {
      var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
      var l = new ListingPlus();
      l.Begin(hGrid[1]);

      l.RangeSlider(Theme.RefreshRate);
      l.BoolToggle(Theme.DockedMode);
      l.GapLine();

      l.Label(Lang.Get("Theme.InspectPane").Bold());
      l.BoolToggle(Theme.InspectPaneTabModify, !Theme.DockedMode.Value);
      l.BoolToggle(Theme.InspectPaneTabAddLog, Theme.InspectPaneTabModify.Value && !Theme.DockedMode.Value);
      l.RangeSliderEntry(Theme.InspectPaneHeight, ref _inspectPaneHeightText, 5, Theme.InspectPaneTabModify.Value);
      l.RangeSlider(Theme.InspectPaneTabWidth, Theme.InspectPaneTabModify.Value);
      l.RangeSlider(Theme.InspectPaneMinTabs, Theme.InspectPaneTabModify.Value);
      l.BoolToggle(Theme.ShowFactionIcon, Theme.InspectPaneTabModify.Value);
      l.BoolToggle(Theme.ShowIdeoligionIcon, Theme.InspectPaneTabModify.Value);
      l.GapLine();

      l.Label(Lang.Get("Theme.Floating").Bold());
      l.RangeSlider(Theme.FloatingAnchor, !Theme.DockedMode.Value);
      l.RangeSliderEntry(Theme.FloatingOffsetX, ref _hudOffsetXText, 1, !Theme.DockedMode.Value);
      l.RangeSliderEntry(Theme.FloatingOffsetY, ref _hudOffsetYText, 2, !Theme.DockedMode.Value);
      l.RangeSliderEntry(Theme.FloatingWidth, ref _hudWidthText, 3, !Theme.DockedMode.Value);
      l.RangeSliderEntry(Theme.FloatingHeight, ref _hudHeightText, 4, !Theme.DockedMode.Value);
      l.BoolToggle(Theme.LetterCompress, !Theme.DockedMode.Value);
      l.RangeSlider(Theme.LetterPadding, !Theme.DockedMode.Value && Theme.LetterCompress.Value);

      l.End();

      l.Begin(hGrid[2]);

      l.TextStyleEditor(Theme.RegularTextStyle);
      l.RangeSlider(Theme.LabelWidth);
      l.RangeSlider(Theme.ValueWidth);
      l.GapLine();

      l.TextStyleEditor(Theme.LargeTextStyle);
      l.GapLine();

      l.TextStyleEditor(Theme.SmallTextStyle);
      l.GapLine();

      l.Label(Lang.Get("Theme.Miscellaneous").Bold());
      l.BoolToggle(Theme.ShowDecimals);

      l.End();
    }
  }
}
