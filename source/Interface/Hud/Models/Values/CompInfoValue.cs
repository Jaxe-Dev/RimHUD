using System.Linq;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class CompInfoValue : ValueModel
{
  protected override string? Value { get; } = GetValue();

  protected override TextStyle TextStyle => Theme.SmallTextStyle;

  private static string? GetValue()
  {
    if (ModsConfig.AnomalyActive && Active.Pawn.health?.hediffSet?.GetFirstHediff<Hediff_MetalhorrorImplant>() is { Emerging: true }) { return "Emerging".Translate(); }
    if (ModsConfig.BiotechActive && Active.Pawn.needs?.energy?.IsLowEnergySelfShutdown is not null) { return "MustBeCarriedToRecharger".Translate(); }

    var quests = new StringBuilder();
    var hasQuest = false;
    QuestUtility.AppendInspectStringsFromQuestParts((inspectString, _) =>
    {
      quests.AppendLine(hasQuest ? " | " + inspectString : inspectString);
      hasQuest = true;
    }, Active.Pawn, out _);

    if (quests.Length > 0) { return quests.ToString(); }

    var info = (from comp in Active.Pawn.AllComps select comp.CompInspectStringExtra() into text where !text.NullOrEmpty() select text.Replace('\n', ' ')).ToArray();
    return info.Length > 0 ? info.ToCommaList() : null;
  }
}
