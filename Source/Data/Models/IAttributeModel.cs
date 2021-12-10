using System;
using UnityEngine;

namespace RimHUD.Data.Models
{
    internal interface IAttributeModel
    {
        PawnModel Model { get; }

        bool Hidden { get; }

        string Label { get; }
        Color? Color { get; }
        Func<string> Tooltip { get; }

        Action OnHover { get; }
        Action OnClick { get; }
    }
}
