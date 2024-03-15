using RimWorld;

namespace RimHUD.Access
{
  [DefOf]
  public static class Defs
  {
    [DefAlias("Schedule")]
    public static readonly MainButtonDef MainButtonRestrict = null!;
    [DefAlias("Work")]
    public static readonly MainButtonDef MainButtonWork = null!;

    [DefAlias("Mood")]
    public static readonly NeedDef NeedMood = null!;
    [DefAlias("Joy")]
    public static readonly NeedDef NeedRecreation = null!;
    [DefAlias("Beauty")]
    public static readonly NeedDef NeedBeauty = null!;
    [DefAlias("Comfort")]
    public static readonly NeedDef NeedComfort = null!;
    [DefAlias("Outdoors")]
    public static readonly NeedDef NeedOutdoors = null!;
    [DefAlias("MechEnergy"), MayRequireBiotech]
    public static readonly NeedDef NeedEnergy = null!;
    [DefAlias("Suppression"), MayRequireIdeology]
    public static readonly NeedDef NeedSuppression = null!;

    [DefAlias("Haul")]
    public static readonly TrainableDef TrainableHaul = null!;
    [DefAlias("Rescue")]
    public static readonly TrainableDef TrainableRescue = null!;

    static Defs() => DefOfHelper.EnsureInitializedInCtor(typeof(Defs));
  }
}
