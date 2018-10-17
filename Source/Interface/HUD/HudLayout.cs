using System.Xml;
using System.Xml.Linq;
using RimHUD.Data.Models;
using UnityEngine;

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

        private HudLayout(XElement xe) : base(xe, true)
        { }

        private static HudLayout FromEmbedded(string name)
        {
            using (var stream = Mod.Assembly.GetManifestResourceStream("RimHUD.Layouts." + name))
            {
                if (stream == null) { throw new Mod.Exception($"Cannot find embedded default HUD layout '{name}'"); }
                using (var reader = XmlReader.Create(stream)) { return FromXml(XDocument.Load(reader)) ?? throw new Mod.Exception($"Error reading embedded default HUD layout '{name}'"); }
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

        public void Draw(Rect rect, PawnModel model)
        {
            Prepare(model);
            Draw(rect);
        }
    }
}
