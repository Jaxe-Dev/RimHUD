using System.Linq;
using RimHUD.Data.Extensions;
using RimHUD.Data.Storage;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
  internal class Tab_ConfigCredits : Tab
  {
    private const float LineSpacing = -4f;

    public override string Label { get; } = "Credits";
    public override TipSignal? Tooltip { get; } = null;

    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _viewRect;

    public override void Reset()
    { }

    public override void Draw(Rect rect)
    {
      var vGrid = rect.GetVGrid(GUIPlus.LargePadding, 10f, -1f, 70f);

      GUIPlus.DrawContainer(vGrid[2]);

      var l = new ListingPlus();
      l.BeginScrollView(vGrid[2], ref _scrollPosition, ref _viewRect);

      foreach (var group in Persistent.Credits.Elements("Group"))
      {
        var label = group.Attribute("label")?.Value;
        if (string.IsNullOrWhiteSpace(label)) { continue; }

        var format = group.Attribute("format")?.Value;
        var entries = group.Elements("Entry").ToArray();

        var special = group.Attribute("special")?.Value == "1";
        var color = group.Attribute("color")?.Value ?? "FFFFFF";

        if (entries.Length > 0 || special)
        {
          l.Label($"[ {label.ColorizeHex(color)} ]".Bold());
          l.Gap(LineSpacing);
        }

        if (entries.Length > 0)
        {
          foreach (var entry in entries)
          {
            var name = entry.Attribute("name")?.Value;
            if (name == null) { continue; }

            if (format != null) { name = string.Format(format, name); }

            var quote = entry.Attribute("quote")?.Value.Italic();
            var quoteText = quote == null ? null : " - ".ColorizeHex("888888") + $"\"{quote}\"".ColorizeHex("AAAAAA");

            var link = entry.Attribute("link")?.Value;
            var linkText = link == null ? null : " (Click to see link)".SmallSize().Italic();

            var text = $"{name.Colorize(Color.white)}{quoteText}{linkText}";
            l.LinkLabel(text, link, Color.gray);

            l.Gap(LineSpacing);
          }
        }
        else if (special) { l.Label("(None)".ColorizeHex("888888")); }

        if (entries.Length > 0 || special) { l.Gap(); }
      }

      l.Gap();

      l.EndScrollView(ref _viewRect);

      l.Begin(vGrid[3]);

      var anchor = Text.Anchor;
      Text.Anchor = TextAnchor.LowerRight;

      l.Label("A special thanks to supporters and countless others who have helped along the way.");
      l.Gap(LineSpacing);
      l.Label(("❤️".ColorizeHex("FF0011") + " Jaxe".Italic()).Bold());

      l.LinkLabel("For more information, check out the mod description".SmallSize().Italic(), Mod.WorkshopLink, Color.gray);

      Text.Anchor = anchor;

      l.End();
    }
  }
}
