using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  public sealed class Dialog_ManagePresets : WindowPlus
  {
    private Rect _scrollView;
    private Vector2 _scrollPosition;

    private LayoutPreset? _selected;

    private Dialog_ManagePresets() : base(new Vector2(500f, 450f), Lang.Get("Interface.Dialog_ManagePresets.Title"))
    {
      onlyOneOfTypeAllowed = true;
      absorbInputAroundWindow = true;
      preventCameraMotion = false;
      doCloseButton = false;
    }

    public static void Open()
    {
      LayoutPreset.RefreshList();
      Find.WindowStack!.Add(new Dialog_ManagePresets());
    }

    protected override void DrawContent(Rect rect)
    {
      var vGrid = rect.GetVGrid(GUIPlus.LargePadding, -1f, WidgetsPlus.ButtonHeight);

      var l = new ListingPlus();
      l.BeginScrollView(vGrid[1], ref _scrollPosition, ref _scrollView);

      foreach (var preset in LayoutPreset.UserList)
      {
        _selected ??= preset;
        if (l.RadioButton(preset.Name, _selected == preset)) { _selected = preset; }
      }

      l.EndScrollView(ref _scrollView);

      l.Begin(vGrid[2]);

      var buttonGrid = l.GetButtonGrid(-1f, -1f);

      if (WidgetsPlus.DrawButton(buttonGrid[1], Lang.Get("Interface.Dialog_ManagePresets.Delete"), enabled: _selected is not null))
      {
        Dialog_Alert.Open(Lang.Get("Interface.Alert.ConfirmDelete", _selected), Dialog_Alert.Buttons.YesNo, () =>
        {
          Presets.Delete(_selected!);
          _selected = null;
        });
      }
      if (WidgetsPlus.DrawButton(buttonGrid[2], Lang.Get("Interface.Button.Close"))) { Close(); }

      l.End();
    }
  }
}
