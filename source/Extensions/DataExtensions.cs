using Verse;

namespace RimHUD.Extensions;

public static class DataExtensions
{
  public static string GetDefNameOrLabel(this Def self) => self.LabelCap.NullOrWhitespace() ? self.defName : self.LabelCap.ToString();

  public static bool IsPlayerControlled(this Pawn self) => self is { Dead: false, playerSettings: not null } && ((self.Faction?.IsPlayer ?? false) || (self.HostFaction?.IsPlayer ?? false));
  public static bool IsPlayerFaction(this Pawn self) => self.Faction?.IsPlayer ?? false;
  public static bool IsPlayerManaged(this Pawn self) => self.IsPlayerFaction() || (self.HostFaction?.IsPlayer ?? false);
  public static bool IsHumanlike(this Pawn self) => self.RaceProps?.Humanlike ?? false;
  public static bool IsAnimal(this Pawn self) => self.RaceProps?.Animal ?? false;
}
