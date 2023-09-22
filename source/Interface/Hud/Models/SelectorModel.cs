using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;

namespace RimHUD.Interface.Hud.Models
{
  public abstract class SelectorModel : BaseModel
  {
    protected override TextStyle TextStyle => Theme.SmallTextStyle;
    protected virtual Color? Color => null;

    public override IWidget? Build(HudArgs? args) => OnClick is null ? null : new SelectorWidget(Label, Tooltip, OnClick, OnHover, TextStyle, Color);
  }
}
