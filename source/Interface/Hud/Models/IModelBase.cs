using System;

namespace RimHUD.Interface.Hud.Models
{
  public interface IModelBase
  {
    string Label { get; }
    Func<string> Tooltip { get; }
    bool Hidden { get; }

    Action OnHover { get; }
    Action OnClick { get; }
  }
}
