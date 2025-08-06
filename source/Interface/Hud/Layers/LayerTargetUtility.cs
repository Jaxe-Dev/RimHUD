using System.Text;
using RimHUD.Engine;
using RimHUD.Extensions;

namespace RimHUD.Interface.Hud.Layers;

public static class LayerTargetUtility
{
  private const string PlayerHumanlike = "H";
  private const string PlayerCreature = "C";
  private const string OtherHumanlike = "h";
  private const string OtherCreature = "c";

  public static LayerTarget FromId(string? value)
  {
    if (value.NullOrWhitespace()) { return LayerTarget.All; }

    var targets = LayerTarget.All;
    if (!value.Contains(LayerTarget.PlayerHumanlike.GetId())) { targets &= ~LayerTarget.PlayerHumanlike; }
    if (!value.Contains(LayerTarget.PlayerCreature.GetId())) { targets &= ~LayerTarget.PlayerCreature; }
    if (!value.Contains(LayerTarget.OtherHumanlike.GetId())) { targets &= ~LayerTarget.OtherHumanlike; }
    if (!value.Contains(LayerTarget.OtherCreature.GetId())) { targets &= ~LayerTarget.OtherCreature; }

    return targets;
  }

  public static string? ToId(this LayerTarget self)
  {
    if (self is LayerTarget.All) { return null; }

    var value = new StringBuilder();
    if (self.HasTarget(LayerTarget.PlayerHumanlike)) { value.Append(LayerTarget.PlayerHumanlike.GetId()); }
    if (self.HasTarget(LayerTarget.PlayerCreature)) { value.Append(LayerTarget.PlayerCreature.GetId()); }
    if (self.HasTarget(LayerTarget.OtherHumanlike)) { value.Append(LayerTarget.OtherHumanlike.GetId()); }
    if (self.HasTarget(LayerTarget.OtherCreature)) { value.Append(LayerTarget.OtherCreature.GetId()); }

    return value.Length > 0 ? value.ToString() : null;
  }

  public static bool HasTarget(this LayerTarget self, LayerTarget target) => ((int)self & (int)target) == (int)target;

  private static string? GetId(this LayerTarget self)
  {
    return self switch
    {
      LayerTarget.PlayerHumanlike => PlayerHumanlike,
      LayerTarget.PlayerCreature => PlayerCreature,
      LayerTarget.OtherHumanlike => OtherHumanlike,
      LayerTarget.OtherCreature => OtherCreature,
      LayerTarget.All => null,
      _ => throw new Report.Exception("Invalid layer target type.")
    };
  }
}
