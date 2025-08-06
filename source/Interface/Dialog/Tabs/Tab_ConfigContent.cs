using System.Collections.Generic;
using System.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog.Tabs;

public sealed class Tab_ConfigContent : Tab
{
  private const float EditorWidth = 280f;

  private static LayoutEditor _editor = new();

  public override string Label { get; } = Lang.Get("Interface.Dialog_Config.Tab_Content");

  public static void DrawLayoutSelector(Rect rect)
  {
    if (!WidgetsPlus.DrawButton(rect, Lang.Get("Interface.Dialog_Config.Tab_Content.Presets.Select"))) { return; }

    var presets = new List<FloatMenuOption> { new(Lang.Get("Interface.Dialog_Config.Tab_Content.Presets.Default").Colorize(Theme.CorePresetColor), static () => LoadDefaultLayout()) };
    presets.AddRange(Presets.CoreList.OrderBy(static preset => preset.FullLabel).Select(static preset => new FloatMenuOption(preset.FullLabel, () => LoadLayout(preset))));
    presets.AddRange(Presets.UserList.OrderBy(static preset => preset.FullLabel).Select(static preset => new FloatMenuOption(preset.FullLabel, () => LoadLayout(preset))));
    presets.AddRange(Presets.PackagedList.OrderBy(static preset => preset.FullLabel).Select(static preset => new FloatMenuOption(preset.FullLabel, () => LoadLayout(preset))));

    presets.ShowMenu();
  }

  public static void RefreshEditor() => _editor = new LayoutEditor();

  private static void ShowLayoutElementFloatMenu(IEnumerable<LayoutElement> items) => items.Select(static item => new FloatMenuOption(item.Label, () => _editor.Add(item), mouseoverGuiAction: itemRect =>
  {
    if (item.Def is null) { return; }
    TooltipsPlus.DrawSimple(itemRect, item.LabelAndId);
  })).ShowMenu();

  private static void ShowNeedBarFloatMenu() => HudContent.BarElements.Select(static item => new FloatMenuOption(item.Label, () => Find.WindowStack!.Add(new FloatMenu([
    GetBarColorStyleMenu(item, BarColorStyle.LowToMain),
    GetBarColorStyleMenu(item, BarColorStyle.LowOnly),
    GetBarColorStyleMenu(item, BarColorStyle.MainToLow),
    GetBarColorStyleMenu(item, BarColorStyle.MainOnly)
  ])), mouseoverGuiAction: itemRect =>
  {
    if (item.Def is null) { return; }
    TooltipsPlus.DrawSimple(itemRect, item.LabelAndId);
  })).ShowMenu();

  private static FloatMenuOption GetBarColorStyleMenu(LayoutElement element, BarColorStyle colorStyle) => new($"{element.Label} {colorStyle.GetLabel().Italic().SmallSize()}", () =>
  {
    if (colorStyle != BarColorStyle.LowToMain) { element.Args.BarColorStyle = colorStyle; }
    _editor.Add(element);
  });

  private static void LoadDefaultLayout()
  {
    LayoutLayer.ResetToDefault();
    Dialog_Alert.Open(Lang.Get("Interface.Alert.PresetDefaultLoaded"));
    RefreshEditor();
  }

  private static void LoadLayout(LayoutPreset preset)
  {
    if (!preset.Load())
    {
      Dialog_Alert.Open(Lang.Get("Interface.Alert.InvalidPreset", preset.Name));
      return;
    }
    Dialog_Alert.Open(Lang.Get("Interface.Alert.PresetLoaded", preset.Name));
    RefreshEditor();
  }

  public override void Reset() => RefreshEditor();

  public override void Draw(Rect rect)
  {
    var l = new ListingPlus();
    var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, EditorWidth);

    l.Begin(hGrid[1]);

    l.Label(Lang.Get($"Interface.Dialog_Config.Tab_Content.Layers.{(Theme.DockedMode.Value ? "Docked" : "Floating")}").Bold());
    var editorRect = l.GetRemaining();

    _editor.Draw(editorRect);

    l.End();

    l.Begin(hGrid[2]);

    l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Presets").Bold());

    var layoutButtonRect = l.GetRect(WidgetsPlus.ButtonHeight);
    Tutorial.Presentation.Stages.SetDialogConfigLayoutButton(layoutButtonRect);

    l.Gap(l.verticalSpacing);

    DrawLayoutSelector(layoutButtonRect);

    var layoutButtonsGrid = l.GetButtonGrid(-1f, -1f);
    if (WidgetsPlus.DrawButton(layoutButtonsGrid[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Presets.Save"))) { Dialog_SavePreset.Open(); }
    if (WidgetsPlus.DrawButton(layoutButtonsGrid[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Presets.Manage"))) { Dialog_ManagePresets.Open(); }

    l.Gap();

    var hasSelected = _editor.Selected is not null;
    var canAddContainer = hasSelected && _editor.Selected!.Type is LayoutElementType.Stack;
    var canAddRow = hasSelected && _editor.Selected!.Type is LayoutElementType.Panel or LayoutElementType.Row;
    var canAddWidget = hasSelected && _editor.Selected!.Type is LayoutElementType.Row;

    l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer").Bold());
    var moveButtonsGrid = l.GetButtonGrid(-1f, -1f);
    if (WidgetsPlus.DrawButton(moveButtonsGrid[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.MoveUp"), enabled: hasSelected && _editor.Selected!.CanMoveUp)) { _editor.Selected!.MoveUp(); }
    if (WidgetsPlus.DrawButton(moveButtonsGrid[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.MoveDown"), enabled: hasSelected && _editor.Selected!.CanMoveDown)) { _editor.Selected!.MoveDown(); }

    if (l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.Remove"), enabled: hasSelected && _editor.Selected!.CanRemove))
    {
      var index = _editor.Selected!.Index;
      var parent = _editor.Selected.Parent;
      _editor.Selected.Remove();
      if (parent is not null) { _editor.Selected = index is 0 ? parent : parent.Children[index - 1]; }
      return;
    }

    if (canAddContainer && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStack"), static () => Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStackDesc"))) { ShowLayoutElementFloatMenu(StackLayer.LayoutElements); }
    if (canAddContainer && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddPanel"), static () => Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddPanelDesc"))) { _editor.Add(PanelLayer.LayoutElement); }
    if (canAddRow && l.ButtonText(Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRow"), static () => Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRowDesc")))
    {
      if (_editor.Selected!.Type is LayoutElementType.Row) { _editor.AddSibling(RowLayer.LayoutElement); }
      else { _editor.Add(RowLayer.LayoutElement, true); }
    }

    l.Gap();

    var buttonRow1 = l.GetButtonGrid(-1f, -1f);
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow1[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddWidget"))) { ShowLayoutElementFloatMenu(HudContent.CommonElements); }
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow1[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddThirdParty"), enabled: HudContent.ExternalElements.Any())) { ShowLayoutElementFloatMenu(HudContent.ExternalElements); }

    var buttonRow2 = l.GetButtonGrid(-1f, -1f);
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow2[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddNeed"), enabled: HudContent.BarElements.Any())) { ShowNeedBarFloatMenu(); }
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow2[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddSkillOrTraining"), enabled: HudContent.SkillAndTrainableElements.Any())) { ShowLayoutElementFloatMenu(HudContent.SkillAndTrainableElements); }

    var buttonRow3 = l.GetButtonGrid(-1f, -1f);
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow3[1], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddStat"), enabled: HudContent.StatElements.Any())) { ShowLayoutElementFloatMenu(HudContent.StatElements); }
    if (canAddWidget && WidgetsPlus.DrawButton(buttonRow3[2], Lang.Get("Interface.Dialog_Config.Tab_Content.Layer.AddRecord"), enabled: HudContent.RecordElements.Any())) { ShowLayoutElementFloatMenu(HudContent.RecordElements); }

    l.End();

    if (!hasSelected || _editor.Selected!.IsRoot) { return; }

    var fillHeightToggle = _editor.Selected.Type is LayoutElementType.Stack or LayoutElementType.Panel;

    var linesRemaining = 5 + (fillHeightToggle ? 1 : 0);
    var selectedRect = hGrid[2].GetVGrid(GUIPlus.MediumPadding, -1f, (linesRemaining * (Text.LineHeight + l.verticalSpacing)) - l.verticalSpacing)[2];

    l.Begin(selectedRect);
    l.Label(Lang.Get("Interface.Dialog_Config.Tab_Content.Selected").Bold() + _editor.Selected.Label.Bold().Italic());

    if (fillHeightToggle) { _editor.Selected.FillHeight = l.CheckboxLabeled(Lang.Get("Interface.Dialog_Config.Tab_Content.Selected.Filled"), _editor.Selected.FillHeight, Lang.Get("Interface.Dialog_Config.Tab_Content.Selected.FilledDesc")); }

    var targets = LayerTarget.All;
    if (!l.CheckboxLabeled(Lang.Get("Layout.Target.PlayerHumanlike"), _editor.Selected.Targets.HasTarget(LayerTarget.PlayerHumanlike), enabled: _editor.Selected.Targets != LayerTarget.PlayerHumanlike)) { targets &= ~LayerTarget.PlayerHumanlike; }
    if (!l.CheckboxLabeled(Lang.Get("Layout.Target.PlayerCreature"), _editor.Selected.Targets.HasTarget(LayerTarget.PlayerCreature), enabled: _editor.Selected.Targets != LayerTarget.PlayerCreature)) { targets &= ~LayerTarget.PlayerCreature; }
    if (!l.CheckboxLabeled(Lang.Get("Layout.Target.OtherHumanlike"), _editor.Selected.Targets.HasTarget(LayerTarget.OtherHumanlike), enabled: _editor.Selected.Targets != LayerTarget.OtherHumanlike)) { targets &= ~LayerTarget.OtherHumanlike; }
    if (!l.CheckboxLabeled(Lang.Get("Layout.Target.OtherCreature"), _editor.Selected.Targets.HasTarget(LayerTarget.OtherCreature), enabled: _editor.Selected.Targets != LayerTarget.OtherCreature)) { targets &= ~LayerTarget.OtherCreature; }

    _editor.Selected.Targets = targets;

    l.End();
  }
}
