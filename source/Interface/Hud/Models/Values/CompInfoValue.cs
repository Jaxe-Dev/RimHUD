using System.Linq;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class CompInfoValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    try
    {
      var info = (from comp in Active.Pawn.AllComps select comp.CompInspectStringExtra() into text where !text.NullOrEmpty() select text.Replace('\n', ' ')).ToArray();
      return info.Length > 0 ? info.ToCommaList() : null;
    }
    catch { return null; }
  }
}
