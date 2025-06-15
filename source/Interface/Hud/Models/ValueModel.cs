using RimHUD.Interface.Hud.Widgets;

namespace RimHUD.Interface.Hud.Models;

public abstract class ValueModel : BaseModel
{
  protected abstract string? Value { get; }

  public override IWidget? Build(HudArgs? args) => Value is null ? null : new ValueWidget(Label, Value, Tooltip, OnHover, OnClick, TextStyle);
}
