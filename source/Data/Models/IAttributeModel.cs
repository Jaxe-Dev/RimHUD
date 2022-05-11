using System;

namespace RimHUD.Data.Models
{
  internal interface IAttributeModel
  {
    PawnModel Model { get; }

    bool Hidden { get; }

    string Label { get; }
    Func<string> Tooltip { get; }

    Action OnHover { get; }
    Action OnClick { get; }
  }
}
