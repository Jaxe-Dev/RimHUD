using System;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Interface.Hud.Widgets;

namespace RimHUD.Interface.Hud.Models
{
  public abstract class BaseModel : IModel
  {
    public abstract IWidget? Build(HudArgs? args);

    protected virtual string? Label => null;
    protected virtual Func<string?>? Tooltip => null;

    protected virtual Action? OnHover => null;
    protected virtual Action? OnClick => null;

    protected virtual TextStyle TextStyle => Theme.RegularTextStyle;
  }
}
