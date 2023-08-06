using System;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Compatibility.PawnRules
{
  [Attributes.IntegratedOptions]
  public static class Mod_PawnRules
  {
    public static IntegratedMod Instance { get; } = new IntegratedMod("Pawn Rules", "PawnRules", "PawnRules.Integration.RimHUD");

    public static void TryRegister()
    {
      if (!Instance.IsActive) { return; }

      try
      {
        Instance.RegisterMethod("GetRules");
        Instance.RegisterMethod("OpenRules");
        Instance.RegisterMethod("CanHaveRules");
        Instance.SetProperty("ReplaceFoodSelector", true);
        Instance.SetProperty("HideGizmo", true);
      }
      catch (Exception exception) { Instance.FailInitialization(exception); }
    }

    private static string GetRules(Pawn pawn) => Instance.InvokeMethod<string>("GetRules", pawn);

    private static void OpenRules(Pawn pawn) => Instance.InvokeMethod("OpenRules", pawn);

    private static bool CanHaveRules(Pawn pawn) => Instance.InvokeMethod<bool>("CanHaveRules", pawn);

    public class ModelRules : IModelSelector
    {
      public PawnModel Owner { get; }

      public bool Hidden { get; }

      public string Label { get; }
      public Color? Color => null;
      public Func<string> Tooltip => null;

      public Action OnHover => null;
      public Action OnClick { get; }

      public ModelRules(PawnModel owner)
      {
        try
        {
          Owner = owner;

          if (!Instance.IsActive || !CanHaveRules(owner.Base))
          {
            Hidden = true;
            return;
          }

          Label = Lang.Get("Integration.PawnRules.RuleNameFormat", GetRules(owner.Base));

          OnClick = () => OpenRules(owner.Base);
        }
        catch (Exception exception)
        {
          Troubleshooter.HandleWarning(exception);
          Hidden = true;
        }
      }
    }
  }
}
