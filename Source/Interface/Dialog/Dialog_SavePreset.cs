using System.Xml.Linq;
using RimHUD.Data;
using RimHUD.Extensions;
using RimHUD.Integration;
using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Dialog_SavePreset : WindowPlus
    {
        private const string NameControl = "PresetName";
        private string _name;
        private bool _includeDocked = true;
        private bool _includeFloating = true;
        private bool _includeHeight = true;
        private bool _includeWidth = true;
        private bool _includeTabs;

        private Dialog_SavePreset() : base(Lang.Get("Dialog_SavePreset.Title").Bold(), new Vector2(400f, 320f))
        {
            onlyOneOfTypeAllowed = true;
            absorbInputAroundWindow = true;
            preventCameraMotion = false;
            doCloseButton = false;
        }

        public static void Open() => Find.WindowStack.Add(new Dialog_SavePreset());
        protected override void DrawContent(Rect rect)
        {
            var l = new ListingPlus();
            l.Begin(rect);
            _includeDocked = l.CheckboxLabeled(Lang.Get("Dialog_SavePreset.IncludeDocked"), _includeDocked, enabled: !_includeDocked || _includeFloating);
            _includeFloating = l.CheckboxLabeled(Lang.Get("Dialog_SavePreset.IncludeFloating"), _includeFloating, enabled: !_includeFloating || _includeDocked);
            l.GapLine();
            _includeHeight = l.CheckboxLabeled(Lang.Get("Dialog_SavePreset.IncludeHeight"), _includeHeight);
            _includeWidth = l.CheckboxLabeled(Lang.Get("Dialog_SavePreset.IncludeWidth"), _includeWidth);
            _includeTabs = l.CheckboxLabeled(Lang.Get("Dialog_SavePreset.IncludeTabs"), _includeTabs, enabled: _includeDocked);

            l.GapLine();
            l.Label(Lang.Get("Dialog_SavePreset.Name"));
            GUI.SetNextControlName(NameControl);
            _name = l.TextEntry(_name);
            GUI.FocusControl(NameControl);
            l.Gap();

            var buttonGrid = l.GetButtonGrid(-1f, -1f);
            if (GUIPlus.DrawButton(buttonGrid[1], Lang.Get("Button.Save"), enabled: Persistent.IsValidFilename(_name))) { Save(); }
            if (GUIPlus.DrawButton(buttonGrid[2], Lang.Get("Button.Cancel"))) { Close(); }
            l.End();
        }

        public override void OnAcceptKeyPressed()
        {
            if (!Persistent.IsValidFilename(_name)) { return; }
            Save();
        }

        private void Save()
        {
            var xe = new XElement(LayoutPreset.RootElementName);

            xe.Add(new XAttribute(LayoutPreset.VersionAttributeName, Mod.Version));

            if (_includeDocked) { xe.Add(HudLayout.Docked.ToXml(HudLayout.DockedElementName, _includeHeight, _includeWidth, _includeTabs)); }
            if (_includeFloating) { xe.Add(HudLayout.Floating.ToXml(HudLayout.FloatingElementName, _includeHeight, _includeWidth, _includeTabs)); }

            Persistent.SaveLayoutPreset(_name, xe);

            Dialog_Alert.Open(Lang.Get("Alert.Saved", _name));
            Close();
        }
    }
}
