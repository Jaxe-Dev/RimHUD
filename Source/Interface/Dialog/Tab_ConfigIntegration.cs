using System;
using RimHUD.Data;
using RimHUD.Integration;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigIntegration : Tab
    {
        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Integration");
        public override Func<string> Tooltip { get; } = null;

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(hGrid[1]);

            l.Label(Lang.Get("Integration.General").Bold());
            l.BoolToggle(Theme.IntegrationGeneralShowCustomNeeds);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Integration.PawnRules").Bold(), () => PawnRules.Description);
            var hasPawnRules = PawnRules.Instance.IsActive;
            if (!hasPawnRules) { l.LinkLabel(Lang.Get("Integration.GetMod"), PawnRules.Url, () => PawnRules.Url); }
            l.BoolToggle(PawnRules.HideGizmo, hasPawnRules);

            l.End();

            l.Begin(hGrid[2]);

            var hasBubbles = Bubbles.Instance.IsActive;
            l.Label(Lang.Get("Integration.Bubbles").Bold(), () => Bubbles.Description);
            if (!hasBubbles) { l.LinkLabel(Lang.Get("Integration.GetMod"), Bubbles.Url, () => Bubbles.Url); }

            var bubblesActive = hasBubbles && Bubbles.Activated.Value;
            l.BoolToggle(Bubbles.Activated, hasBubbles);
            l.BoolToggle(Bubbles.DoNonPlayer, bubblesActive);
            l.BoolToggle(Bubbles.DoAnimals, bubblesActive);
            l.GapLine();
            l.RangeSlider(Bubbles.ScaleStart, bubblesActive);
            l.RangeSlider(Bubbles.MinScale, bubblesActive);
            l.RangeSlider(Bubbles.MaxWidth, bubblesActive);
            l.RangeSlider(Bubbles.Spacing, bubblesActive);
            l.RangeSlider(Bubbles.StartOffset, bubblesActive);
            l.RangeSlider(Bubbles.OffsetDirection, bubblesActive);
            l.GapLine();
            l.RangeSlider(Bubbles.StartOpacity, bubblesActive);
            l.RangeSlider(Bubbles.MouseOverOpacity, bubblesActive);
            l.RangeSlider(Bubbles.MinTime, bubblesActive);
            l.RangeSlider(Bubbles.FadeStart, bubblesActive);
            l.RangeSlider(Bubbles.FadeLength, bubblesActive);
            l.RangeSlider(Bubbles.MaxPerPawn, bubblesActive);
            l.GapLine();
            l.RangeSlider(Bubbles.FontSize, bubblesActive);
            l.RangeSlider(Bubbles.PaddingX, bubblesActive);
            l.RangeSlider(Bubbles.PaddingY, bubblesActive);
            l.GapLine();

            l.End();
        }
    }
}
