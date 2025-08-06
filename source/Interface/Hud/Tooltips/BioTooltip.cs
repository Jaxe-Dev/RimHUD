using System.Linq;
using System.Text;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips;

public static class BioTooltip
{
  public static string? Get()
  {
    if (Active.Pawn.IsAnimal()) { return AnimalTooltip.Get(); }

    if (Active.Pawn.story is null) { return null; }

    var builder = new StringBuilder();

    var title = Active.Pawn.story.TitleCap;
    if (title is not null) { builder.AppendValue("Title".TranslateSimple(), title); }
    var faction = Active.Pawn.Faction?.Name;
    if (faction is not null) { builder.AppendValue("Faction".TranslateSimple(), faction); }
    var ideoligion = !ModsConfig.IdeologyActive || Active.Pawn.Ideo is null ? null : Active.Pawn.Ideo.name;
    if (ideoligion is not null) { builder.AppendValue("Ideo".TranslateSimple().CapitalizeFirst(), ideoligion); }

    builder.AppendLine();

    builder.AppendValue("Childhood".TranslateSimple(), Active.Pawn.story.GetBackstory(BackstorySlot.Childhood)?.TitleCapFor(Active.Pawn.gender));
    builder.AppendValue("Adulthood".TranslateSimple(), Active.Pawn.story.GetBackstory(BackstorySlot.Adulthood)?.TitleCapFor(Active.Pawn.gender));

    builder.AppendLine();

    if (Active.Pawn.story.traits?.TraitsSorted?.Count > 0) { builder.AppendValue("Traits".TranslateSimple(), Active.Pawn.story.traits.TraitsSorted.Select(static trait => trait.LabelCap).ToCommaList()); }

    var disabledWork = Active.Pawn.story.DisabledWorkTagsBackstoryAndTraits;
    if (disabledWork is not WorkTags.None) { builder.AppendValue("IncapableOf".TranslateSimple(), disabledWork.GetAllSelectedItems<WorkTags>().Where(static tag => tag != WorkTags.None).Select(static tag => tag.LabelTranslated().CapitalizeFirst()).ToCommaList().Colorize(ColoredText.SubtleGrayColor)); }

    builder.AppendLine();

    builder.AppendStatLine(StatDefOf.PsychicSensitivity);

    if (ModsConfig.RoyaltyActive)
    {
      builder.AppendValue("MeditationFocuses".TranslateSimple().CapitalizeFirst(), MeditationUtility.FocusTypesAvailableForPawnString(Active.Pawn).CapitalizeFirst());
      builder.AppendStatLine(StatDefOf.MeditationFocusGain);
      builder.AppendStatLine(StatDefOf.MeditationFocusStrength);
    }

    if (!ModsConfig.IdeologyActive) { return builder.ToStringTrimmedOrNull(); }

    builder.AppendStatLine(StatDefOf.SocialIdeoSpreadFrequencyFactor);
    builder.AppendStatLine(StatDefOf.CertaintyLossFactor);

    return builder.ToStringTrimmedOrNull();
  }
}
