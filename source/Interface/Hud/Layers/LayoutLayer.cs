using System;
using System.Xml;
using System.Xml.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layers
{
  public class LayoutLayer : VStackLayer
  {
    public const string RootName = "Layout";
    public const string DockedFileName = "Docked.xml";
    public const string FloatingFileName = "Floating.xml";
    public const float Padding = WidgetsPlus.TinyPadding;

    public const string DockedElementName = "Docked";
    public const string FloatingElementName = "Floating";

    private const string HeightAttributeName = "Height";
    private const string TabsAttributeName = "Tabs";
    private const string WidthAttributeName = "Width";

    private static readonly LayoutLayer DefaultDocked = FromEmbedded("Defaults.Docked.xml");
    private static readonly LayoutLayer DefaultFloating = FromEmbedded("Defaults.Floating.xml");

    public static LayoutLayer Docked { get; set; } = DefaultDocked;
    public static LayoutLayer Floating { get; set; } = DefaultFloating;

    private DateTime _lastRefresh;
    private Pawn _lastPawn;

    private LayoutLayer(XElement xe) : base(xe, true)
    {
      HudTimings.Add(this);

      bool docked;
      if (xe.Name == DockedElementName) { docked = true; }
      else if (xe.Name == FloatingElementName) { docked = false; }
      else { return; }

      var width = xe.Attribute(WidthAttributeName)?.Value.ToInt() ?? -1;
      var height = xe.Attribute(HeightAttributeName)?.Value.ToInt() ?? -1;
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

    private static LayoutLayer FromEmbedded(string id)
    {
      using (var stream = Mod.Assembly.GetManifestResourceStream("RimHUD.Resources.Presets." + id))
      {
        if (stream == null) { throw new Mod.Exception($"Cannot find embedded HUD layout '{id}'"); }
        using (var reader = XmlReader.Create(stream)) { return FromXml(XDocument.Load(reader).Root) ?? throw new Mod.Exception($"Error reading embedded HUD layout '{id}'"); }
      }
    }

    public static LayoutLayer FromXml(XElement xe) => new LayoutLayer(xe);
    public static LayoutLayer FromLayoutView(LayoutEditor editor) => new LayoutLayer(editor.Root.ToXml());

    public override XElement ToXml() => ToXml(null);

    public XElement ToXml(string name, int width = -1, int height = -1, int tabs = -1)
    {
      var xml = new XElement(name ?? RootName);

      if (width > 0) { xml.Add(new XAttribute(WidthAttributeName, width)); }
      if (height > 0) { xml.Add(new XAttribute(HeightAttributeName, height)); }
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

    public void Draw(Rect rect, PawnModel owner)
    {
      HudTimings.Update(this)?.Start();

      try
      {
        if (owner == null) { return; }

        if (owner.Base != _lastPawn || _lastRefresh == default || (DateTime.Now - _lastRefresh).TotalMilliseconds > Theme.RefreshRate.Value * 100)
        {
          Prepare(owner);
          _lastPawn = owner.Base;
          _lastRefresh = DateTime.Now;
        }

        Draw(rect);
      }
      catch (Exception exception) { Troubleshooter.HandleError(exception); }

      HudTimings.Update(this)?.Finish(rect, true);
    }
  }
}
