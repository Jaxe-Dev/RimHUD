using System;
using RimHUD.Interface;
using RimHUD.Interface.HUD;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
  internal class HealthBarModel : IBarModel
  {
    public PawnModel Model { get; }
    public bool Hidden { get; }

    public string Label { get; }
    public Color? Color { get; }
    public Func<string> Tooltip { get; }

    public Action OnHover { get; }
    public Action OnClick { get; }

    public float Max { get; }
    public float Value { get; }
    public HudBar.ValueStyle ValueStyle { get; }
    public float[] Thresholds { get; }

    public HealthBarModel(PawnModel model)
    {
      Model = model;

      if (model.Base.health == null)
      {
        Hidden = true;
        return;
      }

      Label = "Health".Translate();

      Max = 1f;
      Value = model.Health.Percent;
      ValueStyle = HudBar.ValueStyle.Percentage;

      Tooltip = model.Health.Tooltip;

      OnClick = InspectPanePlus.ToggleHealthTab;
    }
  }
}
