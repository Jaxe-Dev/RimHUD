using RimHUD.Interface.Hud.Widgets;

namespace RimHUD.Interface.Hud.Models
{
  public abstract class BarModel : BaseModel
  {
    protected abstract string? Value { get; }

    protected abstract float Fill { get; }
    protected virtual float[]? Thresholds => null;
    protected virtual BarColorStyle? ColorStyle => null;

    public override IWidget? Build(HudArgs args) => Fill < 0f ? null : new BarWidget(Label, Value, Fill, Thresholds, Tooltip, OnHover, OnClick, TextStyle, args.BarColorStyle ?? ColorStyle ?? default);
  }
}
