using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Models
{
  public class TrainingModel : IModelValue
  {
    public PawnModel Owner { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover => null;
    public Action OnClick { get; }

    public string Value { get; }

    public TrainingModel(PawnModel owner, TrainableDef def)
    {
      try
      {
        Owner = owner;

        bool canTrainNow;
        try { canTrainNow = owner.Base.RaceProps?.trainability != null && owner.Base.training != null && owner.Base.training.CanAssignToTrain(def, out var visible).Accepted && visible; }
        catch (Exception exception)
        {
          Troubleshooter.HandleWarning(exception);
          canTrainNow = false;
        }

        if (!canTrainNow)
        {
          Hidden = true;
          return;
        }

        var disabled = !owner.Base.training.GetWanted(def);
        var hasLearned = owner.Base.training.HasLearned(def);

        Color color;
        if (disabled) { color = Theme.DisabledColor.Value; }
        else if (hasLearned) { color = Theme.SkillMinorPassionColor.Value; }
        else { color = Theme.MainTextColor.Value; }

        Label = def.GetLabelCap().Colorize(color);

        var steps = GetSteps(owner.Base, def);
        var value = (steps + " / " + def.steps).Colorize(color);

        Value = hasLearned ? value.Bold() : value;

        Tooltip = () => owner.GetAnimalTooltip(def);

        OnClick = InspectPanePlus.ToggleTrainingTab;
      }
      catch (Exception exception)
      {
        Troubleshooter.HandleWarning(exception);
        Hidden = true;
      }
    }

    private static int GetSteps(Pawn pawn, TrainableDef def) => (int)Access.Method_RimWorld_Pawn_TrainingTracker_GetSteps.Invoke(pawn.training, def);
  }
}
