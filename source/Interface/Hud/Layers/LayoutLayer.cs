using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layers;

public sealed class LayoutLayer : VStackLayer
{
  public const float Padding = GUIPlus.TinyPadding;

  public const string RootName = "Layout";
  public const string DockedFileName = "Docked.xml";
  public const string FloatingFileName = "Floating.xml";

  public const string DockedElementName = "Docked";
  public const string FloatingElementName = "Floating";

  private const string ManifestResourcePrefix = "RimHUD.Resources.Layouts.";

  private const string WidthAttributeName = "Width";
  private const string HeightAttributeName = "Height";
  private const string TabsAttributeName = "Tabs";

  private static readonly LayoutLayer DefaultDocked = FromEmbedded("Defaults.Docked.xml");
  private static readonly LayoutLayer DefaultFloating = FromEmbedded("Defaults.Floating.xml");

  public static LayoutLayer Docked { get; set; } = DefaultDocked;
  public static LayoutLayer Floating { get; set; } = DefaultFloating;

  private readonly Stopwatch _stopwatch = new();

  private Pawn? _lastPawn;

  private LayoutLayer(XElement xml) : base(xml)
  {
    Args.FillHeight = true;
    HudTimings.Add(this);

    bool docked;
    if (xml.Name == DockedElementName) { docked = true; }
    else if (xml.Name == FloatingElementName) { docked = false; }
    else { return; }

    var width = xml.GetAttribute(WidthAttributeName)?.ToInt() ?? -1;
    var height = xml.GetAttribute(HeightAttributeName)?.ToInt() ?? -1;
    var tabs = xml.GetAttribute(TabsAttributeName)?.ToInt() ?? -1;

    if (height > 0)
    {
      if (docked) { Theme.InspectPaneHeight.Value = height; }
      else { Theme.FloatingHeight.Value = height; }
    }
    if (width > 0)
    {
      if (docked) { Theme.InspectPaneTabWidth.Value = width; }
      else { Theme.FloatingWidth.Value = width; }
    }
    if (docked && tabs > 0) { Theme.InspectPaneMinTabs.Value = tabs; }
  }

  public static LayoutLayer FromXml(XElement xml) => new(xml);

  public static LayoutLayer FromLayoutView(LayoutEditor editor) => new(editor.Root.ToXml());

  public static void LoadDefaultAndSave(bool compact = false)
  {
    LoadDefault(compact);
    Presets.Save();
  }

  private static LayoutLayer FromEmbedded(string id)
  {
    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ManifestResourcePrefix + id);
    if (stream is null) { throw new Report.Exception($"Cannot find embedded layout '{id}'."); }

    using var reader = XmlReader.Create(stream);
    var layout = FromXml(XDocument.Load(reader).Root);
    if (layout is null) { throw new Report.Exception($"Error reading embedded layout '{id}'."); }

    LayoutPreset.Active = LayoutPreset.DefaultName;

    return layout;
  }

  private static void LoadDefault(bool compact)
  {
    Docked = DefaultDocked;
    Floating = DefaultFloating;

    Theme.SetDefaultHud(compact);

    LayoutPreset.Active = LayoutPreset.DefaultName;

    Persistent.Save();
  }

  public XElement ToXml(string? name, int width = -1, int height = -1, int tabs = -1)
  {
    var xml = new XElement(name ?? RootName);

    if (width > 0) { xml.AddAttribute(WidthAttributeName, width); }
    if (height > 0) { xml.AddAttribute(HeightAttributeName, height); }
    if (tabs > 0) { xml.AddAttribute(TabsAttributeName, tabs); }

    foreach (var child in Children) { xml.Add(child.ToXml()); }
    return xml;
  }

  public new void Draw(Rect rect)
  {
    HudTimings.Update(this)?.Start();

    try
    {
      _stopwatch.Start();

      if (Active.Pawn != _lastPawn || _stopwatch.ElapsedMilliseconds > Theme.RefreshRate.Value * 100)
      {
        Prepare();
        _lastPawn = Active.Pawn;
        _stopwatch.Restart();
      }

      base.Draw(rect);
    }
    finally { HudTimings.Update(this)?.Finish(rect, true); }
  }

  protected override XElement StartXml() => ToXml(null);

  public void RefreshNow() => _lastPawn = null;
}
