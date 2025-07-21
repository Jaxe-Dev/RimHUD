using System;
using RimHUD.Access;
using RimHUD.Configuration;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using RimHUD.Interface.Screen;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values;

public sealed class TrainableValue : ValueModel
{
  protected override string? Label { get; }

  protected override string? Value { get; }

  protected override Func<string?>? Tooltip { get; }

  protected override Action? OnClick { get; }

  public TrainableValue(TrainableDef def)
  {
    if (Active.Pawn.RaceProps?.trainability is null || Active.Pawn.training is null || !Active.Pawn.training.CanAssignToTrain(def, out var visible).Accepted || !visible) { return; }

    var disabled = !Active.Pawn.training.GetWanted(def);
    var hasLearned = Active.Pawn.training.HasLearned(def);

    var color = disabled switch
    {
      true => Theme.DisabledColor.Value,
      _ => hasLearned ? Theme.SkillMinorPassionColor.Value : Theme.MainTextColor.Value
    };

    Label = def.GetDefNameOrLabel().Colorize(color);

    var steps = GetSteps(Active.Pawn, def);
    var value = $"{steps} / {def.steps}".Colorize(color);

    Value = hasLearned ? value.Bold() : value;

    Tooltip = () => AnimalTooltip.Get(def);

    OnClick = InspectPaneTabs.ToggleTraining;
  }

  private static int GetSteps(Pawn pawn, TrainableDef def) => Reflection.RimWorld_Pawn_TrainingTracker_GetSteps.Invoke<int>(pawn.training, def);
}
