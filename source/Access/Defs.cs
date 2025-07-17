using RimWorld;

namespace RimHUD.Access;

[DefOf]
public static class Defs
{
  [DefAlias("Schedule")] public static readonly MainButtonDef MainButtonRestrict = null!;
  [DefAlias("Work")] public static readonly MainButtonDef MainButtonWork = null!;

  [DefAlias("Mood")] public static readonly NeedDef NeedMood = null!;
  [DefAlias("Joy")] public static readonly NeedDef NeedRecreation = null!;
  [DefAlias("Beauty")] public static readonly NeedDef NeedBeauty = null!;
  [DefAlias("Comfort")] public static readonly NeedDef NeedComfort = null!;
  [DefAlias("Outdoors")] public static readonly NeedDef NeedOutdoors = null!;
  [DefAlias("MechEnergy"), MayRequireBiotech] public static readonly NeedDef NeedEnergy = null!;
  [DefAlias("Suppression"), MayRequireIdeology] public static readonly NeedDef NeedSuppression = null!;

  [DefAlias("Haul")] public static readonly TrainableDef TrainableHaul = null!;
  [DefAlias("Rescue")] public static readonly TrainableDef TrainableRescue = null!;

  [DefAlias("AttackTarget"), MayRequireOdyssey] public static readonly TrainableDef TrainableAttackTarget = null!;
  [DefAlias("Comfort"), MayRequireOdyssey] public static readonly TrainableDef TrainableComfort = null!;
  [DefAlias("Dig"), MayRequireOdyssey] public static readonly TrainableDef TrainableDig = null!;
  [DefAlias("EggSpew"), MayRequireOdyssey] public static readonly TrainableDef TrainableEggSpew = null!;
  [DefAlias("Forage"), MayRequireOdyssey] public static readonly TrainableDef TrainableForage = null!;
  [DefAlias("SludgeSpew"), MayRequireOdyssey] public static readonly TrainableDef TrainableSludgeSpew = null!;
  [DefAlias("TerrorRoar"), MayRequireOdyssey] public static readonly TrainableDef TrainableTerrorRoar = null!;
  [DefAlias("ThrumboRoar"), MayRequireOdyssey] public static readonly TrainableDef TrainableThrumboRoar = null!;
  [DefAlias("WarTrumpet"), MayRequireOdyssey] public static readonly TrainableDef TrainableWarTrumpet = null!;

  static Defs() => DefOfHelper.EnsureInitializedInCtor(typeof(Defs));
}
