using System.Linq;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips
{
  public static class BioTooltip
  {
    public static string? Get()
    {
      if (Active.Pawn.IsAnimal()) { return AnimalTooltip.Get(); }

      if (Active.Pawn.story is null) { return null; }

      var builder = new StringBuilder();

      var title = Active.Pawn.story.TitleCap;
      if (title is not null) { builder.AppendLineIfNotEmpty(Lang.Get("Model.Bio.Title", title)); }
      var faction = Active.Pawn.Faction?.Name;
      if (faction is not null) { builder.AppendLineIfNotEmpty(Lang.Get("Model.Bio.Faction", faction)); }
      var ideoligion = !ModsConfig.IdeologyActive || Active.Pawn.Ideo is null ? null : Active.Pawn.Ideo.name;
      if (ideoligion is not null) { builder.AppendLineIfNotEmpty(Lang.Get("Model.Bio.Ideoligion", ideoligion)); }

      builder.AppendLine();

      builder.AppendLineIfNotEmpty("Childhood".Translate().WithValue(Active.Pawn.story.GetBackstory(BackstorySlot.Childhood)?.TitleCapFor(Active.Pawn.gender)));
      builder.AppendLineIfNotEmpty("Adulthood".Translate().WithValue(Active.Pawn.story.GetBackstory(BackstorySlot.Adulthood)?.TitleCapFor(Active.Pawn.gender)));

      builder.AppendLine();

      builder.AppendLineIfNotEmpty(Active.Pawn.story.traits?.allTraits?.Count > 0 ? "Traits".Translate().WithValue(Active.Pawn.story.traits.allTraits.Select(static trait => trait.LabelCap).ToCommaList(true)) : null);

      var disabledWork = Active.Pawn.story.DisabledWorkTagsBackstoryAndTraits;
      var incapable = disabledWork is WorkTags.None ? null : "IncapableOf".Translate().WithValue(disabledWork.GetAllSelectedItems<WorkTags>().Where(static tag => tag != WorkTags.None).Select(static tag => tag.LabelTranslated().CapitalizeFirst()).ToCommaList(true));
      builder.AppendLineIfNotEmpty(incapable.NullOrWhitespace() ? null : incapable.Colorize(Theme.CriticalColor.Value));

      builder.AppendLine();

      builder.AppendStatLine(StatDefOf.PsychicSensitivity);

      if (ModsConfig.RoyaltyActive)
      {
        builder.AppendLine("MeditationFocuses".Translate().CapitalizeFirst().WithValue(MeditationUtility.FocusTypesAvailableForPawnString(Active.Pawn).CapitalizeFirst()));
        builder.AppendStatLine(StatDefOf.MeditationFocusGain);
        builder.AppendStatLine(StatDefOf.MeditationFocusStrength);
      }

      if (!ModsConfig.IdeologyActive) { return builder.ToTooltip(); }

      builder.AppendStatLine(StatDefOf.SocialIdeoSpreadFrequencyFactor);
      builder.AppendStatLine(StatDefOf.CertaintyLossFactor);

      return builder.ToTooltip();
    }
  }
}
