using System;
using UnityEngine;
using Verse;

namespace RimHUD.Data.Models
{
    internal interface IAttributeModel
    {
        PawnModel Model { get; }

        bool Hidden { get; }

        string Label { get; }
        Color? Color { get; }
        TipSignal? Tooltip { get; }

        Action OnHover { get; }
        Action OnClick { get; }
    }
}
