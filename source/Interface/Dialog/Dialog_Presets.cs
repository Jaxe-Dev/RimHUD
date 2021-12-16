﻿using RimHUD.Data;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimHUD.Data.Storage;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal class Dialog_Presets : WindowPlus
  {
    private Rect _scrollView;
    private Vector2 _scrollPosition;

    private LayoutPreset _selected;

    private Dialog_Presets() : base(Lang.Get("Dialog_Presets.Title").Bold(), new Vector2(400f, 450f))
    {
      onlyOneOfTypeAllowed = true;
      absorbInputAroundWindow = true;
      preventCameraMotion = false;
      doCloseButton = false;
    }

    public static void Open()
    {
      LayoutPreset.RefreshUserPresets();
      Find.WindowStack.Add(new Dialog_Presets());
    }

    protected override void DrawContent(Rect rect)
    {
      var vGrid = rect.GetVGrid(GUIPlus.LargePadding, -1f, 70f);
      var l = new ListingPlus();
      l.BeginScrollView(vGrid[1], ref _scrollPosition, ref _scrollView);

      foreach (var preset in LayoutPreset.UserList)
      {
        if (_selected == null) { _selected = preset; }
        if (l.RadioButton(preset.Label, _selected == preset)) { _selected = preset; }
      }

      l.EndScrollView(ref _scrollView);

      l.Begin(vGrid[2]);

      var buttonGrid = l.GetButtonGrid(-1f, -1f);
      if (GUIPlus.DrawButton(buttonGrid[1], Lang.Get("Dialog_Presets.Delete"), enabled: _selected?.IsUserMade ?? false)) { ConfirmDelete(); }
      if (GUIPlus.DrawButton(buttonGrid[2], Lang.Get("Button.Close"))) { Close(); }

      l.End();
    }

    private void ConfirmDelete() => Dialog_Alert.Open(Lang.Get("Alert.ConfirmDelete", _selected.Name), Dialog_Alert.Buttons.YesNo, Delete);

    private void Delete()
    {
      Persistent.DeleteLayoutPreset(_selected);
      _selected = null;
      LayoutPreset.RefreshUserPresets();
    }
  }
}
