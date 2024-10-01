using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using Verse;

namespace RimHUD.Interface.Hud.Models.Values
{
  public sealed class StunInfoValue : ValueModel
  {
    protected override string Value { get; }

    protected override TextStyle TextStyle => Theme.SmallTextStyle;

    public StunInfoValue()
    {
      Value = GetValue();
    }

    private static string GetValue()
    {
      string str = "";
      
      var stunner = Active.Pawn.stances?.stunner;
      if (stunner is not null && stunner.Stunned)
      {
        if (stunner.Hypnotized)
        {
          str += "InTrance".Translate();
        }
        else if (stunner.StunFromEMP)
        {
          str += "StunnedByEMP".Translate() + ": " + stunner.StunTicksLeft.ToStringSecondsFromTicks();
        }
        else
        {
          str += "StunLower".Translate().CapitalizeFirst() + ": " + stunner.StunTicksLeft.ToStringSecondsFromTicks();
        }
      }

      var stagger = Active.Pawn.stances?.stagger;
      if (stagger is not null && stagger.Staggered)
      {
        str += (str.NullOrEmpty() ? "" : "\n") + "SlowedByDamage".Translate() + ": " + stagger.StaggerTicksLeft.ToStringSecondsFromTicks();
      }

      return Lang.Get("Model.Info.StunInfo", str);
    }
  }
}
