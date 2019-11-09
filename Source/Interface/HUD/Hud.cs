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
            var configRect = GetConfigButtonRect(bounds, false);
            IsMouseOverConfigButton = Mouse.IsOver(configRect);
            HudLayout.Docked.Draw(bounds, model ?? PawnModel.Selected);
            if (Mouse.IsOver(bounds)) { DrawConfigButton(configRect); }
        }

        public static void DrawFloating()
        {
            var bounds = Theme.GetHudBounds();

            Widgets.DrawWindowBackground(bounds);
            var inner = bounds.ContractedBy(GUIPlus.MediumPadding);
            var configRect = GetConfigButtonRect(inner, true);
            IsMouseOverConfigButton = Mouse.IsOver(configRect);
            HudLayout.Floating.Draw(inner, PawnModel.Selected);
            if(Mouse.IsOver(bounds)) { DrawConfigButton(configRect); }
        }

        private static Rect GetConfigButtonRect(Rect bounds, bool top)
        {
            var y = top ? bounds.y : bounds.yMax - ConfigButtonSize;
            return new Rect(bounds.xMax - ConfigButtonSize, y, ConfigButtonSize, ConfigButtonSize);
        }

        private static void DrawConfigButton(Rect rect)
        {
            if (Widgets.ButtonImage(rect, Textures.ConfigIcon)) { Dialog_Config.Open(); }
        }

        public static bool IsMouseOverConfigButton = true;
    }
}
