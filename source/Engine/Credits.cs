using System.IO;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace RimHUD.Engine;

public static class Credits
{
  private static Vector2 _scrollPosition = Vector2.zero;
  private static Rect _scrollRect;

  private static Group[]? _groups;
  public static bool Exists => _groups is not null;

  public static void Load()
  {
    var file = new FileInfo(Path.Combine(Mod.ContentPack.RootDir, "About", "Credits.xml"));

    try
    {
      var xml = XDocument.Load(file.FullName).Root;
      if (xml?.IsEmpty ?? true) { throw new Report.Exception("Unable to load credits."); }

      _groups = xml.Elements("Group").Select(Group.FromXml).WhereNotNull().ToArray();
    }
    catch
    {
      _groups = null;
      if (SteamManager.Initialized) { Report.Warning("Unable to load credits. This may be an unofficial version of the mod."); }
    }
  }

  public static void Draw(Rect rect)
  {
    if (_groups is null) { return; }

    var vGrid = rect.GetVGrid(GUIPlus.SmallPadding, Text.LineHeight, -1f, 70f);

    Widgets.Label(vGrid[1], "This mod is kindly supported by:");

    WidgetsPlus.DrawContainer(vGrid[2]);

    var l = new ListingPlus();

    l.BeginScrollView(vGrid[2], ref _scrollPosition, ref _scrollRect);

    l.Gap();

    var hasEntry = false;
    foreach (var group in _groups.Where(static group => group.Entries.Length > 0 || group.Special))
    {
      if (hasEntry) { l.Gap(); }
      hasEntry = true;

      l.Label(group.Label);
      l.Gap(GenUI.GapTiny);

      if (group.Entries.Length > 0)
      {
        foreach (var entry in group.Entries)
        {
          l.LinkLabel(entry.Label, entry.Link, Color.gray);
          l.Gap(GenUI.GapTiny);
        }
      }
      else { l.Label("(None)".ColorizeHex("888888")); }
    }

    l.Gap();

    l.EndScrollView(ref _scrollRect);

    l.Begin(vGrid[3]);

    GUIPlus.SetAnchor(TextAnchor.LowerRight);

    l.Label("A special thanks to all above and to countless others who have helped along the way.");
    l.Gap(GenUI.GapTiny);
    l.Label($"{"❤️".ColorizeHex("FF0011")} {" Jaxe".Italic()}".Bold());

    l.LinkLabel("For more information, check out the mod description".SmallSize().Italic(), Mod.WorkshopLink, Color.gray);

    GUIPlus.ResetAnchor();

    l.End();
  }

  private sealed class Entry
  {
    public string Label { get; }
    public string? Link { get; }

    private Entry(string label, string? link)
    {
      Label = label;
      Link = link;
    }

    public static Entry? FromXml(XElement xml, string? format)
    {
      var label = xml.GetAttribute("Label");
      if (label.NullOrWhitespace()) { return null; }

      var labelText = format.NullOrWhitespace() ? label : string.Format(format, label);

      var quote = xml.GetAttribute("Quote")?.Italic();
      var quoteText = quote is null ? null : " - ".ColorizeHex("888888") + $"\"{quote}\"".ColorizeHex("AAAAAA");

      var link = xml.GetAttribute("Link");
      var linkText = link is null ? null : " (Click to see link)".SmallSize().Italic();

      return new Entry($"{labelText.Colorize(Color.white)}{quoteText}{linkText}", link);
    }
  }

  private sealed class Group
  {
    public string Label { get; }
    public bool Special { get; }
    public Entry[] Entries { get; }

    private Group(string label, bool special, Entry[] entries)
    {
      Label = label;
      Special = special;
      Entries = entries;
    }

    public static Group? FromXml(XElement xml)
    {
      var label = xml.GetAttribute("Label");
      if (label.NullOrWhitespace()) { return null; }

      var special = xml.GetAttribute("Special") is "1";
      var color = xml.GetAttribute("Color") ?? "FFFFFF";
      var format = xml.GetAttribute("Format");

      var entries = xml.Elements("Entry").Select(entry => Entry.FromXml(entry, format)).WhereNotNull();

      return new Group($"[ {label.ColorizeHex(color)} ]".Bold(), special, entries.ToArray());
    }
  }
}
