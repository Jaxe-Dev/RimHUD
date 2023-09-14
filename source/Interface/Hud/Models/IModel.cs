using RimHUD.Interface.Hud.Widgets;

namespace RimHUD.Interface.Hud.Models
{
  public interface IModel
  {
    IWidget? Build(HudArgs args);
  }
}
