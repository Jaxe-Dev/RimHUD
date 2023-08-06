using System.Collections.Generic;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  public class Tab_ConfigContent : Tab
  {
    private const float EditorWidth = 280f;
    private const float TargetsHeight = 120f;

    private static LayoutEditor _editor = new LayoutEditor(Theme.HudDocked.Value);

    public override string Label { get; } = Lang.Get("Interface.Dialog_Config.Tab_Content");
    public override TipSignal? Tooltip { get; } = null;

    public override void Reset() => RefreshEditor();

    private static string GetMode() => Lang.Get("Interface.Dialog_Config.Tab_Content.Mode." + (_editor.Docked ? "Docked" : "Floating"));

    private static void DrawModeSelector(ListingPlus l)
    {
      if (!l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Mode", GetMode()))) { return; }
      var presets = new List<FloatMenuOption>
      {
        new FloatMenuOption(Lang.Get("Interface.Dialog_Config.Tab_Content.Mode.Docked"), () => RefreshEditor(true)),
        new FloatMenuOption(Lang.Get("Interface.Dialog_Config.Tab_Content.Mode.Floating"), () => RefreshEditor(false))
      };

      Find.WindowStack.Add(new FloatMenu(presets));
    }

    public static void DrawPresetSelector(Rect rect)
    {
      if (!WidgetsPlus.DrawButton(rect, Lang.Get("Interface.Dialog_Config.Tab_Content.Preset"))) { return; }

      var presets = new List<FloatMenuOption> { new FloatMenuOption(Lang.Get("Interface.Dialog_Config.Tab_Content.Preset.Default"), LoadDefaultPreset) };
      presets.AddRange(LayoutPreset.UserList.OrderBy(preset => preset.Label).Select(preset => new FloatMenuOption(preset.Label, () => LoadPreset(preset))));
      presets.AddRange(LayoutPreset.FixedList.OrderBy(preset => preset.Label).Select(preset => new FloatMenuOption(preset.Label, () => LoadPreset(preset))));

      presets.ShowMenu();
    }

    public override void Draw(Rect rect)
    {
      var l = new ListingPlus();
      var hGrid = rect.GetHGrid(WidgetsPlus.LargePadding, -1f, EditorWidth);

      l.Begin(hGrid[1]);

      l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Editor", GetMode()).Bold());
      var editorRect = l.GetRemaining();

      _editor.Draw(editorRect);

      l.End();

      l.Begin(hGrid[2]);

      l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Layout").Bold());
      DrawModeSelector(l);
      l.Gap();

      var presetButtonRect = l.GetRect(WidgetsPlus.ButtonHeight);
      l.Gap(l.verticalSpacing);
      Tutorial.SetDialogConfigPresetButton(presetButtonRect);

      DrawPresetSelector(presetButtonRect);

      var importExportGrid = l.GetButtonGrid(-1f, -1f);
      if (WidgetsPlus.DrawButton(importExportGrid[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layout.SavePreset"))) { Dialog_SavePreset.Open(); }
      if (WidgetsPlus.DrawButton(importExportGrid[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layout.ManagePresets"))) { Dialog_Presets.Open(); }

      l.Gap();

      var canAddContainer = _editor.CanAddContainer;
      var canAddRow = _editor.CanAddRow;
      var canAddWidget = _editor.CanAddWidget;
      var hasSelected = _editor.HasSelected;

      l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer").Bold());
      var moveButtonsGrid = l.GetButtonGrid(-1f, -1f);
      if (WidgetsPlus.DrawButton(moveButtonsGrid[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.MoveUp"), enabled: hasSelected && _editor.Selected.CanMoveUp)) { _editor.Selected.MoveUp(); }
      if (WidgetsPlus.DrawButton(moveButtonsGrid[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.MoveDown"), enabled: hasSelected && _editor.Selected.CanMoveDown)) { _editor.Selected.MoveDown(); }

      if (l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.Remove"), enabled: hasSelected && _editor.Selected.CanRemove))
      {
        var index = _editor.Selected.Index;
        var parent = _editor.Selected.Parent;
        _editor.Selected.Remove();
        if (parent != null) { _editor.Selected = index == 0 ? parent : parent.Contents[index - 1]; }
        return;
      }

      if (canAddContainer && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStack"), Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStackDesc"))) { ShowLayoutElementFloatMenu(StackLayer.LayoutElements); }
      if (canAddContainer && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddPanel"), Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddPanelDesc"))) { _editor.Add(PanelLayer.LayoutElement); }
      if (canAddRow && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRow"), Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRowDesc")))
      {
        Mod.Warning(_editor.Selected.Type.ToString());
        if (_editor.Selected.Type == LayoutElementType.Row) { _editor.AddSibling(RowLayer.LayoutElement); }
        else { _editor.Add(RowLayer.LayoutElement, selectNew: true); }
      }

      l.Gap();

      var buttonRow1 = l.GetButtonGrid(-1f, -1f);
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow1[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddWidget"))) { ShowLayoutElementFloatMenu(HudContent.BaseElements); }
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow1[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddThirdParty"), enabled: HudContent.ThirdPartyElements.Length > 0)) { ShowLayoutElementFloatMenu(HudContent.ThirdPartyElements); }

      var buttonRow2 = l.GetButtonGrid(-1f, -1f);
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow2[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddNeed"), enabled: HudContent.NeedElements.Length > 0)) { ShowNeedBarFloatMenu(); }
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow2[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddSkillOrTraining"), enabled: HudContent.SkillAndTrainingElements.Length > 0)) { ShowLayoutElementFloatMenu(HudContent.SkillAndTrainingElements); }

      var buttonRow3 = l.GetButtonGrid(-1f, -1f);
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow3[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStat"), enabled: HudContent.StatElements.Length > 0)) { ShowLayoutElementFloatMenu(HudContent.StatElements); }
      if (canAddWidget && WidgetsPlus.DrawButton(buttonRow3[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRecord"), enabled: HudContent.RecordElements.Length > 0)) { ShowLayoutElementFloatMenu(HudContent.RecordElements); }

      l.End();

      if (!hasSelected || _editor.Selected.IsRoot) { return; }

      var selectedRect = hGrid[2].GetVGrid(WidgetsPlus.MediumPadding, -1f, TargetsHeight)[2];
      l.Begin(selectedRect);
      l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Selected").Bold() + _editor.Selected.Label.Bold().Italic());

      var targets = HudTarget.None;
      if (l.CheckboxLabeled(Lang.Get("Layout.Target.PlayerHumanlike"), _editor.Selected.Targets.HasTarget(HudTarget.PlayerHumanlike), enabled: _editor.Selected.Targets != HudTarget.PlayerHumanlike)) { targets |= HudTarget.PlayerHumanlike; }
      if (l.CheckboxLabeled(Lang.Get("Layout.Target.PlayerCreature"), _editor.Selected.Targets.HasTarget(HudTarget.PlayerCreature), enabled: _editor.Selected.Targets != HudTarget.PlayerCreature)) { targets |= HudTarget.PlayerCreature; }
      if (l.CheckboxLabeled(Lang.Get("Layout.Target.OtherHumanlike"), _editor.Selected.Targets.HasTarget(HudTarget.OtherHumanlike), enabled: _editor.Selected.Targets != HudTarget.OtherHumanlike)) { targets |= HudTarget.OtherHumanlike; }
      if (l.CheckboxLabeled(Lang.Get("Layout.Target.OtherCreature"), _editor.Selected.Targets.HasTarget(HudTarget.OtherCreature), enabled: _editor.Selected.Targets != HudTarget.OtherCreature)) { targets |= HudTarget.OtherCreature; }

      _editor.Selected.Targets = targets;

      if (_editor.Selected.Type == LayoutElementType.Stack || _editor.Selected.Type == LayoutElementType.Panel) { _editor.Selected.FillHeight = l.CheckboxLabeled(Lang.Get("Interface.Dialog_Config.Tab_Content.Selected.Filled"), _editor.Selected.FillHeight, Lang.Get("Interface.Dialog_Config.Tab_Content.Selected.FilledDesc")); }

      l.End();
    }

    private static void ShowLayoutElementFloatMenu(IEnumerable<LayoutElement> items, string variant = null) => items.Select(item => new FloatMenuOption(item.Label, () => _editor.Add(item, variant), mouseoverGuiAction: itemRect =>
    {
      if (item.Def == null) { return; }
      WidgetsPlus.DrawTooltip(itemRect, () => item.LabelAndId, false);
    })).ShowMenu();

    private static void ShowNeedBarFloatMenu() => HudContent.NeedElements.Select(item => new FloatMenuOption(item.Label, () => Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>
    {
      GetNeedBarColorStyleMenu(item, BarWidget.ColorStyle.LowToMain),
      GetNeedBarColorStyleMenu(item, BarWidget.ColorStyle.LowOnly),
      GetNeedBarColorStyleMenu(item, BarWidget.ColorStyle.MainToLow),
      GetNeedBarColorStyleMenu(item, BarWidget.ColorStyle.MainOnly)
    })), mouseoverGuiAction: itemRect =>
    {
      if (item.Def == null) { return; }
      WidgetsPlus.DrawTooltip(itemRect, () => item.LabelAndId, false);
    })).ShowMenu();

    private static FloatMenuOption GetNeedBarColorStyleMenu(LayoutElement element, BarWidget.ColorStyle colorStyle) => new FloatMenuOption($"{element.Label} {BarWidget.GetColorStyleLabel(colorStyle).Italic().SmallSize()}", () => _editor.Add(element, colorStyle == BarWidget.ColorStyle.LowToMain ? null : colorStyle.ToString()));

    private static void RefreshEditor(bool? docked = null) => _editor = new LayoutEditor(docked ?? _editor.Docked);

    private static void LoadDefaultPreset()
    {
      LayoutLayer.LoadDefaultAndSave();
      Dialog_Alert.Open(Lang.Get("Interface.Dialog_Config.Tab_Content.Preset.DefaultLoaded"));
      RefreshEditor();
    }

    private static void LoadPreset(LayoutPreset preset)
    {
      if (!preset.Load())
      {
        Dialog_Alert.Open(Lang.Get("Interface.Dialog_Config.Tab_Content.Preset.Invalid", preset.Name));
        return;
      }
      Dialog_Alert.Open(Lang.Get("Interface.Dialog_Config.Tab_Content.Preset.Loaded", preset.Name));
      RefreshEditor();
    }
  }
}
