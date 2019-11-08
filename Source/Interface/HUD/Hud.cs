using RimHUD.Data.Models;
using RimHUD.Data.Theme;
using RimHUD.Interface.Dialog;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
    internal static class Hud
    {
        private static readonly float ConfigButtonSize = Mathf.Max(Textures.ConfigIcon.width, Textures.ConfigIcon.height);

        public static void DrawDocked(Rect bounds, PawnModel model)
        {
            HudLayout.Docked.Draw(bounds, model ?? PawnModel.Selected);
            DrawConfigButton(bounds, false);
        }

        public static void DrawFloating()
        {
            var bounds = Theme.GetHudBounds();

            Widgets.DrawWindowBackground(bounds);
            var inner = bounds.ContractedBy(GUIPlus.MediumPadding);
            HudLayout.Floating.Draw(inner, PawnModel.Selected);
            DrawConfigButton(inner, true);
        }

        private static void DrawConfigButton(Rect bounds, bool top)
        {
            var y = top ? bounds.y : bounds.yMax - ConfigButtonSize;
            if (Mouse.IsOver(bounds) && Widgets.ButtonImage(new Rect(bounds.xMax - ConfigButtonSize, y, ConfigButtonSize, ConfigButtonSize), Textures.ConfigIcon)) { Dialog_Config.Open(); }
        }
    }
}
