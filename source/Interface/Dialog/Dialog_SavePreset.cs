using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layout;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog;

public sealed class Dialog_SavePreset : WindowPlus
{
  private const string NameControl = "PresetName";
  private string? _name;

  private bool _includeDocked = true;
  private bool _includeFloating = true;
  private bool _includeWidth = true;
  private bool _includeHeight = true;
  private bool _includeTabs;
  private bool _includeTextSizes = true;

  private Dialog_SavePreset() : base(new Vector2(400f, 350f), Lang.Get("Interface.Dialog_SavePreset.Title"))
  {
    onlyOneOfTypeAllowed = true;
    absorbInputAroundWindow = true;
    preventCameraMotion = false;
    doCloseButton = false;
  }

  public static void Open() => Find.WindowStack!.Add(new Dialog_SavePreset());

  public override void OnAcceptKeyPressed()
  {
    if (!Presets.IsValidFilename(_name)) { return; }
    Save();
  }

  protected override void DrawContent(Rect rect)
  {
    var l = new ListingPlus();
    l.Begin(rect);

    _includeDocked = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeDocked"), _includeDocked, enabled: !_includeDocked || _includeFloating);
    _includeFloating = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeFloating"), _includeFloating, enabled: !_includeFloating || _includeDocked);
    l.GapLine();

    _includeWidth = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeWidth"), _includeWidth);
    _includeHeight = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeHeight"), _includeHeight);
    _includeTabs = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeTabs"), _includeTabs, enabled: _includeDocked);
    l.GapLine();

    _includeTextSizes = l.CheckboxLabeled(Lang.Get("Interface.Dialog_SavePreset.IncludeTextSizes"), _includeTextSizes);
    l.GapLine();

    l.Label(Lang.Get("Interface.Dialog_SavePreset.Name"));
    GUI.SetNextControlName(NameControl);
    _name = l.TextEntry(_name);
    GUI.FocusControl(NameControl);
    l.Gap();

    var buttonGrid = l.GetButtonGrid(-1f, -1f);
    if (WidgetsPlus.DrawButton(buttonGrid[1], Lang.Get("Interface.Button.Save"), enabled: Presets.IsValidFilename(_name))) { Save(); }
    if (WidgetsPlus.DrawButton(buttonGrid[2], Lang.Get("Interface.Button.Cancel"))) { Close(); }
    l.End();
  }

  private void Save()
  {
    if (_name is null) { return; }

    LayoutPreset.SaveCurrent(_name, _includeDocked, _includeFloating, _includeWidth, _includeHeight, _includeTabs, _includeTextSizes);
    Presets.RefreshList();

    Dialog_Alert.Open(Lang.Get("Interface.Alert.PresetSaved", _name));
    Close();
  }
}
