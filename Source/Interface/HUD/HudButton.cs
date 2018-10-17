using System;
using RimHUD.Data.Models;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal class HudButton : HudFeature
    {
        private readonly Texture2D _texture;
        private readonly Action _onClick;
        private readonly Action _onHover;
        public override float Height { get; }
        public HudButton(string label, TipSignal? tooltip, TextStyle textStyle, Texture2D texture, Action onClick, Action onHover) : base(label, tooltip, textStyle)
        {
            _texture = texture;
            _onClick = onClick;
            _onHover = onHover;
            Height = textStyle.LineHeight;
        }

        private HudButton(ButtonModel model, TextStyle textStyle) : this(model.Label, model.Tooltip, textStyle, model.Texture, model.OnClick, model.OnHover)
        { }

        public static HudButton FromModel(ButtonModel model, TextStyle textStyle) => model == null ? null : new HudButton(model, textStyle);

        public override bool Draw(Rect rect)
        {
            GUI.DrawTexture(rect, _texture);
            DrawText(rect.ContractedBy(GUIPlus.SmallPadding, 0f), Label);

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
