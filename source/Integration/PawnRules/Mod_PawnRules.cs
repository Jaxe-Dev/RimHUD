using System;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Models;

namespace RimHUD.Integration.PawnRules
{
  public sealed class Mod_PawnRules : IntegratedMod
  {
    private static Mod_PawnRules? _instance;

    public static bool IsAvailable => (_instance?.IsActive ?? false) && CanHaveRules();

    public Mod_PawnRules() : base("Pawn Rules", "PawnRules", "PawnRules.Integration.RimHUD")
    {
      try
      {
        Traverse.Property("ReplaceFoodSelector")!.SetValue(true);
        Traverse.Property("HideGizmo")!.SetValue(true);

        _instance = this;
      }
      catch (Exception exception) { DisableFrom(exception); }
    }

    private static string GetRules() => _instance!.Traverse.Method("GetRules", Active.Pawn)!.GetValue<string>();

    private static void OpenRules() => _instance!.Traverse.Method("OpenRules", Active.Pawn)!.GetValue();

    private static bool CanHaveRules() => _instance!.Traverse.Method("CanHaveRules", Active.Pawn)!.GetValue<bool>();

    public sealed class RulesSelector : SelectorModel
    {
      protected override string? Label { get; }

      protected override Action? OnClick { get; }

      public RulesSelector()
      {
        try
        {
          if (!_instance!.IsActive || !CanHaveRules()) { return; }

          Label = Lang.Get("Integration.PawnRules.RuleNameFormat", GetRules());

          OnClick = static () => OpenRules();
        }
        catch (Exception exception)
        {
          Label = null;
          _instance!.DisableFrom(exception);
          Report.HandleWarning(exception);
        }
      }
    }
  }
}
