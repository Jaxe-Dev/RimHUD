using System;
using System.Xml;
using System.Xml.Linq;
using RimHUD.Data;
using RimHUD.Data.Models;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudLayout : HudVStack
    {
        public const string RootName = "Layout";
        public const string DockedFileName = "Docked.xml";
        public const string FloatingFileName = "Floating.xml";
        public const float Padding = GUIPlus.TinyPadding;

        public const string DockedElementName = "Docked";
        public const string FloatingElementName = "Floating";

        private const string HeightAttributeName = "Height";
        private const string TabsAttributeName = "Tabs";
        private const string WidthAttributeName = "Width";

        private static readonly HudLayout DefaultDocked = FromEmbedded("DefaultDocked.xml");
        private static readonly HudLayout DefaultFloating = FromEmbedded("DefaultFloating.xml");

        public static HudLayout Docked { get; set; } = DefaultDocked;
        public static HudLayout Floating { get; set; } = DefaultFloating;

        private DateTime _lastDraw;
        private Pawn _lastPawn;

        private HudLayout(XElement xe) : base(xe, true)
        {
            bool docked;
            if (xe.Name == DockedElementName) { docked = true; }
            else if (xe.Name == FloatingElementName) { docked = false; }
            else { return; }

            var height = xe.Attribute(HeightAttributeName)?.Value.ToInt() ?? -1;
            var width = xe.Attribute(WidthAttributeName)?.Value.ToInt() ?? -1;
            var tabs = xe.Attribute(TabsAttributeName)?.Value.ToInt() ?? -1;

            if (height > 0)
            {
                if (docked) { Theme.InspectPaneHeight.Value = height; }
                else { Theme.HudHeight.Value = height; }
            }
            if (width > 0)
            {
                if (docked) { Theme.InspectPaneTabWidth.Value = width; }
                else { Theme.HudWidth.Value = width; }
            }
            if (docked && (tabs > 0)) { Theme.InspectPaneMinTabs.Value = tabs; }
        }

        private static HudLayout FromEmbedded(string id)
        {
            using (var stream = Mod.Assembly.GetManifestResourceStream("RimHUD.Layouts." + id))
            {
                if (stream == null) { throw new Mod.Exception($"Cannot find embedded HUD layout '{id}'"); }
                using (var reader = XmlReader.Create(stream)) { return FromXml(XDocument.Load(reader).Root) ?? throw new Mod.Exception($"Error reading embedded HUD layout '{id}'"); }
            }
        }

        public static HudLayout FromXml(XElement xe) => new HudLayout(xe);
        public static HudLayout FromLayoutView(LayoutEditor editor) => new HudLayout(editor.Root.ToXml());

        public override XElement ToXml()
        {
            var xml = new XElement(RootName);
            foreach (var container in Containers) { xml.Add(container.ToXml()); }
            return xml;
        }

        public static void LoadDefault()
        {
            Docked = DefaultDocked;
            Floating = DefaultFloating;

            Theme.InspectPaneHeight.ToDefault();
            Theme.InspectPaneTabWidth.ToDefault();
            Theme.InspectPaneMinTabs.ToDefault();
            Theme.HudHeight.ToDefault();
            Theme.HudWidth.ToDefault();

            Persistent.Save();
        }

        public static void LoadDefaultAndSave()
        {
            LoadDefault();
            Persistent.SaveLayouts();
        }

        public static void LoadPreset(string id)
        {
            Docked = FromEmbedded($"Presets.{id}.Docked.xml");
            Floating = FromEmbedded($"Presets.{id}.Floating.xml");

            Persistent.SaveLayouts();
        }

        public void Draw(Rect rect, PawnModel model)
        {
            try
            {
                if ((model.Base != _lastPawn) || (_lastDraw == default(DateTime)) || ((DateTime.Now - _lastDraw).TotalMilliseconds > Theme.RefreshRate.Value * 100))
                {
                    Prepare(model);
                    _lastPawn = model.Base;
                    _lastDraw = DateTime.Now;
                }

                Draw(rect);
            }
            catch (Exception ex)
            {
                State.Activated = false;
                Dialog_Alert.Open("RimHUD has automatically deactivated due to the following error:\n\n" + ex.Message.Italic());
            }
        }
    }
}
