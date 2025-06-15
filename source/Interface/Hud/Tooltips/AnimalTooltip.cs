using System.Text;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Models;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Tooltips;

public static class AnimalTooltip
{
  public static string Get(TrainableDef? def)
  {
    var builder = new StringBuilder();

    if (Active.Pawn.RaceProps is not null)
    {
      var trainability = Active.Pawn.RaceProps.trainability?.LabelCap;
      if (trainability is not null) { builder.AppendLine(Lang.Get("Model.Bio.Trainability", trainability)); }

      builder.AppendLine($"{"TrainingDecayInterval".Translate()}: {TrainableUtility.DegradationPeriodTicks(Active.Pawn.def).ToStringTicksToDays()}");
      if (!TrainableUtility.TamenessCanDecay(Active.Pawn.def)) { builder.AppendLine("TamenessWillNotDecay".Translate()); }

      builder.AppendLine(Lang.Get("Model.Bio.Petness", Active.Pawn.RaceProps.petness.ToStringPercent()));
      builder.AppendLine(Lang.Get("Model.Bio.Diet", Active.Pawn.RaceProps.ResolvedDietCategory.ToStringHuman()));
    }

    var master = Active.Pawn.playerSettings?.Master?.LabelShort;
    if (!master.NullOrWhitespace())
    {
      builder.AppendLine();
      builder.AppendLine(Lang.Get("Model.Bio.Master", master));
    }

    if (def is null) { return builder.ToStringTrimmed(); }

    builder.AppendLine();
    builder.AppendLine(def.description);

    return builder.ToStringTrimmed();
  }

  public static string Get() => Get(null);
}
