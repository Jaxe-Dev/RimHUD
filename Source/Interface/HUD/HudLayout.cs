using System;
using System.Xml;
using System.Xml.Linq;
using RimHUD.Data;
using RimHUD.Data.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudLayout : HudVStack
    {
        public const string DockedFileName = "Docked.xml";
        public const string FloatingFileName = "Floating.xml";
        public const float Padding = GUIPlus.TinyPadding;

        private static readonly HudLayout DefaultDocked = FromEmbedded("DefaultDocked.xml");
        private static readonly HudLayout DefaultFloating = FromEmbedded("DefaultFloating.xml");

        public static HudLayout Docked { get; set; } = DefaultDocked;
        public static HudLayout Floating { get; set; } = DefaultFloating;

        private DateTime _lastDraw;
        private Pawn _lastPawn;

        private HudLayout(XElement xe) : base(xe, true)
        { }

        private static HudLayout FromEmbedded(string id)
        {
            using (var stream = Mod.Assembly.GetManifestResourceStream("RimHUD.Layouts." + id))
            {
                if (stream == null) { throw new Mod.Exception($"Cannot find embedded HUD layout '{id}'"); }
                using (var reader = XmlReader.Create(stream)) { return FromXml(XDocument.Load(reader)) ?? throw new Mod.Exception($"Error reading embedded HUD layout '{id}'"); }
            }
        }

        public static HudLayout FromXml(XDocument doc) => new HudLayout(doc.Root);

        public override XElement ToXml()
        {
            var xml = new XElement("Layout");
            foreach (var container in Containers) { xml.Add(container.ToXml()); }
            return xml;
        }

        public XDocument AsXDocument()
        {
            var doc = new XDocument();
            doc.Add(ToXml());
            return doc;
        }

        public static void LoadDefault()
        {
            Docked = DefaultDocked;
            Floating = DefaultFloating;
        }

        public static void LoadPreset(string id)
        {
            Docked = FromEmbedded($"Presets.{id}.Docked.xml");
            Floating = FromEmbedded($"Presets.{id}.Floating.xml");

            Persistent.SaveLayouts();
        }

        public void Draw(Rect rect, PawnModel model)
        {
            if ((model.Base != _lastPawn) || (_lastDraw == default(DateTime)) || ((DateTime.Now - _lastDraw).TotalMilliseconds > Theme.RefreshRate.Value * 100))
            {
                Prepare(model);
                _lastPawn = model.Base;
                _lastDraw = DateTime.Now;
            }

            Draw(rect);
       }
    }
}
