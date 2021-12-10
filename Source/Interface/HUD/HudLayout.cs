using System;
using System.Xml;
using System.Xml.Linq;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using RimHUD.Data.Storage;
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

        private static readonly HudLayout DefaultDocked = FromEmbedded("Defaults.Docked.xml");
        private static readonly HudLayout DefaultFloating = FromEmbedded("Defaults.Floating.xml");

        public static HudLayout Docked { get; set; } = DefaultDocked;
        public static HudLayout Floating { get; set; } = DefaultFloating;

        private DateTime _lastRefresh;
        private Pawn _lastPawn;

        private HudLayout(XElement xe) : base(xe, true)
        {
            HudTimings.Add(this);

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
            if (docked && tabs > 0) { Theme.InspectPaneMinTabs.Value = tabs; }
        }

        private static HudLayout FromEmbedded(string id)
        {
            using (var stream = Mod.Assembly.GetManifestResourceStream("RimHUD.Interface.Layout.Presets." + id))
            {
                if (stream == null) { throw new Mod.Exception($"Cannot find embedded HUD layout '{id}'"); }
                using (var reader = XmlReader.Create(stream)) { return FromXml(XDocument.Load(reader).Root) ?? throw new Mod.Exception($"Error reading embedded HUD layout '{id}'"); }
            }
        }

        public static HudLayout FromXml(XElement xe) => new HudLayout(xe);
        public static HudLayout FromLayoutView(LayoutEditor editor) => new HudLayout(editor.Root.ToXml());

        public override XElement ToXml() => ToXml(null);

        public XElement ToXml(string name, int height = -1, int width = -1, int tabs = -1)
        {
            var xml = new XElement(name ?? RootName);

            if (height > 0) { xml.Add(new XAttribute(HeightAttributeName, height)); }
            if (width > 0) { xml.Add(new XAttribute(WidthAttributeName, width)); }
            if (tabs > 0) { xml.Add(new XAttribute(TabsAttributeName, tabs)); }

            foreach (var container in Containers) { xml.Add(container.ToXml()); }
            return xml;
        }

        private static void LoadDefault()
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

        public void Draw(Rect rect, PawnModel model)
        {
            HudTimings.Update(this)?.Start();

            try
            {
                if (model == null) { return; }

                if (model.Base != _lastPawn || _lastRefresh == default || (DateTime.Now - _lastRefresh).TotalMilliseconds > Theme.RefreshRate.Value * 100)
                {
                    Prepare(model);
                    _lastPawn = model.Base;
                    _lastRefresh = DateTime.Now;
                }

                Draw(rect);
            }
            catch (Exception exception) { Mod.HandleError(exception); }

            HudTimings.Update(this)?.Finish(rect, true);
        }
    }
}
