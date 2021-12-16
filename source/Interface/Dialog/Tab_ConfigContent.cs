using System.Collections.Generic;
using System.Linq;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using RimHUD.Data.Models;
using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal class Tab_ConfigContent : Tab
  {
    private const float EditorWidth = 280f;
    private const float SelectedHeight = 145f;

    public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Content");
    public override TipSignal? Tooltip { get; } = null;

    private LayoutEditor _editor = new LayoutEditor(Theme.HudDocked.Value);

    public override void Reset() => RefreshEditor();

    private string GetMode() => Lang.Get("Dialog_Config.Tab.Content.Mode." + (_editor.Docked ? "Docked" : "Floating"));

    private void DrawModeSelector(ListingPlus l)
    {
      if (!l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Mode", GetMode()))) { return; }
      var presets = new List<FloatMenuOption>
      {
        new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Mode.Docked"), () => RefreshEditor(true)),
        new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Mode.Floating"), () => RefreshEditor(false))
      };

      Find.WindowStack.Add(new FloatMenu(presets));
    }

    private void DrawPresetSelector(ListingPlus l)
    {
      if (!l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Preset"))) { return; }

      var presets = new List<FloatMenuOption> { new FloatMenuOption(Lang.Get("Dialog_Config.Tab.Content.Preset.Default"), LoadDefaultPreset) };
      presets.AddRange(LayoutPreset.UserList.OrderBy(preset => preset.Label).Select(preset => new FloatMenuOption(preset.Label, () => LoadPreset(preset))));
      presets.AddRange(LayoutPreset.FixedList.OrderBy(preset => preset.Label).Select(preset => new FloatMenuOption(preset.Label, () => LoadPreset(preset))));

      presets.ShowMenu();
    }

    public override void Draw(Rect rect)
    {
      var l = new ListingPlus();
      var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, EditorWidth);

      l.Begin(hGrid[1]);
      l.Label(Lang.Get("Dialog_Config.Tab.Content.Editor", GetMode()).Bold());
      var editorRect = l.GetRemaining();
      _editor.Draw(editorRect);
      l.End();

      l.Begin(hGrid[2]);

      l.Label(Lang.Get("Dialog_Config.Tab.Content.Layout").Bold());
      DrawModeSelector(l);
      l.Gap();
      DrawPresetSelector(l);

      var importExportGrid = l.GetButtonGrid(-1f, -1f);
      if (GUIPlus.DrawButton(importExportGrid[1], Lang.Get("Dialog_Config.Tab.Content.Layout.SavePreset"))) { Dialog_SavePreset.Open(); }
      if (GUIPlus.DrawButton(importExportGrid[2], Lang.Get("Dialog_Config.Tab.Content.Layout.ManagePresets"))) { Dialog_Presets.Open(); }

      l.Gap();

      var canAddContainer = _editor.CanAddContainer;
      var canAddRow = _editor.CanAddRow;
      var canAddElement = _editor.CanAddElement;
      var hasSelected = _editor.HasSelected;

      l.Label(Lang.Get("Dialog_Config.Tab.Content.Component").Bold());
      var moveButtonsGrid = l.GetButtonGrid(-1f, -1f);
      if (GUIPlus.DrawButton(moveButtonsGrid[1], Lang.Get("Dialog_Config.Tab.Content.Component.MoveUp"), enabled: hasSelected && _editor.Selected.CanMoveUp)) { _editor.Selected.MoveUp(); }
      if (GUIPlus.DrawButton(moveButtonsGrid[2], Lang.Get("Dialog_Config.Tab.Content.Component.MoveDown"), enabled: hasSelected && _editor.Selected.CanMoveDown)) { _editor.Selected.MoveDown(); }

      if (l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Component.Remove"), enabled: hasSelected && _editor.Selected.CanRemove))
      {
        _editor.Selected.Remove();
        return;
      }

      if (canAddContainer && l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Component.Stack"), Lang.Get("Dialog_Config.Tab.Content.Component.StackDesc"))) { HudModel.StackComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }
      if (canAddContainer && l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Component.Panel"), Lang.Get("Dialog_Config.Tab.Content.Component.PanelDesc"))) { _editor.Add(HudModel.PanelComponent); }
      if (canAddRow && l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Component.Row"), Lang.Get("Dialog_Config.Tab.Content.Component.RowDesc"))) { _editor.Add(HudModel.RowComponent); }
      if (canAddElement && l.ButtonText(Lang.Get("Dialog_Config.Tab.Content.Component.Element"), Lang.Get("Dialog_Config.Tab.Content.Component.ElementDesc"))) { HudModel.ElementComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }

      var statRecordGrid = l.GetButtonGrid(-1f, -1f);
      if (canAddElement && GUIPlus.DrawButton(statRecordGrid[1], Lang.Get("Dialog_Config.Tab.Content.Component.Stat"), enabled: HudModel.StatComponents.Length > 0)) { HudModel.StatComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }
      if (canAddElement && GUIPlus.DrawButton(statRecordGrid[2], Lang.Get("Dialog_Config.Tab.Content.Component.Record"), enabled: HudModel.RecordComponents.Length > 0)) { HudModel.RecordComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }

      var customButtonsGrid = l.GetButtonGrid(-1f, -1f);
      if (canAddElement && GUIPlus.DrawButton(customButtonsGrid[1], Lang.Get("Dialog_Config.Tab.Content.Component.Need"), enabled: HudModel.NeedComponents.Length > 0)) { HudModel.NeedComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }
      if (canAddElement && GUIPlus.DrawButton(customButtonsGrid[2], Lang.Get("Dialog_Config.Tab.Content.Component.SkillOrTraining"), enabled: HudModel.SkillAndTrainingComponents.Length > 0)) { HudModel.SkillAndTrainingComponents.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item))).ShowMenu(); }

      l.End();

      if (!hasSelected || _editor.Selected.IsRoot) { return; }

      var selectedRect = hGrid[2].GetVGrid(GUIPlus.MediumPadding, -1f, SelectedHeight)[2];
      l.Begin(selectedRect);
      l.Label(Lang.Get("Dialog_Config.Tab.Content.Selected").Bold() + _editor.Selected.Label.Bold().Italic());

      var targets = HudTarget.None;
      if (l.CheckboxLabeled(Lang.Get("Model.Target.PlayerHumanlike"), _editor.Selected.Targets.HasTarget(HudTarget.PlayerHumanlike), enabled: _editor.Selected.Targets != HudTarget.PlayerHumanlike)) { targets |= HudTarget.PlayerHumanlike; }
      if (l.CheckboxLabeled(Lang.Get("Model.Target.PlayerCreature"), _editor.Selected.Targets.HasTarget(HudTarget.PlayerCreature), enabled: _editor.Selected.Targets != HudTarget.PlayerCreature)) { targets |= HudTarget.PlayerCreature; }
      if (l.CheckboxLabeled(Lang.Get("Model.Target.OtherHumanlike"), _editor.Selected.Targets.HasTarget(HudTarget.OtherHumanlike), enabled: _editor.Selected.Targets != HudTarget.OtherHumanlike)) { targets |= HudTarget.OtherHumanlike; }
      if (l.CheckboxLabeled(Lang.Get("Model.Target.OtherCreature"), _editor.Selected.Targets.HasTarget(HudTarget.OtherCreature), enabled: _editor.Selected.Targets != HudTarget.OtherCreature)) { targets |= HudTarget.OtherCreature; }

      _editor.Selected.Targets = targets;

      if (_editor.Selected.Type == LayoutItemType.Stack || _editor.Selected.Type == LayoutItemType.Panel) { _editor.Selected.FillHeight = l.CheckboxLabeled(Lang.Get("Dialog_Config.Tab.Content.Selected.Filled"), _editor.Selected.FillHeight, Lang.Get("Dialog_Config.Tab.Content.Selected.FilledDesc")); }

      l.End();
    }

    private void RefreshEditor(bool? docked = null) => _editor = new LayoutEditor(docked ?? _editor.Docked);

    private void LoadDefaultPreset()
    {
      HudLayout.LoadDefaultAndSave();
      Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Preset.DefaultLoaded"));
      RefreshEditor();
    }

    private void LoadPreset(LayoutPreset preset)
    {
      if (!preset.Load())
      {
        Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Preset.Invalid", preset.Name));
        return;
      }
      Dialog_Alert.Open(Lang.Get("Dialog_Config.Tab.Content.Preset.Loaded", preset.Name));
      RefreshEditor();
    }
  }
}
