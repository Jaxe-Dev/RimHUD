using System;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudSelector : HudFeature
    {
        private readonly Color? _color;
        private readonly Action _onClick;
        private readonly Action _onHover;
        public override float Height { get; }

        public HudSelector(string label, TipSignal? tooltip, TextStyle textStyle, Color? color, Action onClick, Action onHover) : base(label, tooltip, textStyle)
        {
            _color = color;
            _onClick = onClick;
            _onHover = onHover;
            Height = textStyle.LineHeight;
        }

        private HudSelector(ISelectorModel model, TextStyle textStyle) : this(model.Label, model.Tooltip, textStyle, model.Color, model.OnClick, model.OnHover) { }

        public static HudSelector FromSelectorModel(ISelectorModel model, TextStyle textStyle) => model == null ? null : new HudSelector(model, textStyle);

        public override bool Draw(Rect rect)
        {
            if (Label.NullOrEmpty()) { return true; }

            Widgets.DrawBoxSolid(rect, _color ?? Theme.SelectorBackgroundColor.Value);
            DrawText(rect.HUDContractedBy(GUIPlus.SmallPadding, 0f), Label, Theme.SelectorTextColor.Value);

            if (Mouse.IsOver(rect))
            {
                var border = rect.ContractedBy(-1f);
                Widgets.DrawBox(border);
                _onHover?.Invoke();
            }

            if (Widgets.ButtonInvisible(rect)) { _onClick?.Invoke(); }
            return true;
        }
    }
}
