using RimHUD.Data;
using RimHUD.Data.Extensions;
using RimHUD.Data.Integration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigIntegration : Tab
    {
        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Integration");
        public override TipSignal? Tooltip { get; } = null;

        public override void Reset() { }

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);

            var bubbles = new ListingPlus();

            bubbles.Begin(hGrid[1]);

            var hasBubbles = Bubbles.Instance.IsActive;
            bubbles.Label(Lang.Get("Integration.Bubbles").Bold(), Bubbles.Description);
            if (!hasBubbles) { bubbles.LinkLabel(Lang.Get("Integration.GetMod"), Bubbles.Url, Bubbles.Url); }

            var bubblesActive = hasBubbles && Bubbles.Activated.Value;
            bubbles.BoolToggle(Bubbles.Activated, hasBubbles);
            bubbles.BoolToggle(Bubbles.DoNonPlayer, bubblesActive);
            bubbles.BoolToggle(Bubbles.DoAnimals, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Bubbles.ScaleStart, bubblesActive);
            bubbles.RangeSlider(Bubbles.MinScale, bubblesActive);
            bubbles.RangeSlider(Bubbles.MaxWidth, bubblesActive);
            bubbles.RangeSlider(Bubbles.Spacing, bubblesActive);
            bubbles.RangeSlider(Bubbles.StartOffset, bubblesActive);
            bubbles.RangeSlider(Bubbles.OffsetDirection, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Bubbles.StartOpacity, bubblesActive);
            bubbles.RangeSlider(Bubbles.MouseOverOpacity, bubblesActive);
            bubbles.RangeSlider(Bubbles.MinTime, bubblesActive);
            bubbles.RangeSlider(Bubbles.FadeStart, bubblesActive);
            bubbles.RangeSlider(Bubbles.FadeLength, bubblesActive);
            bubbles.RangeSlider(Bubbles.MaxPerPawn, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Bubbles.FontSize, bubblesActive);
            bubbles.RangeSlider(Bubbles.PaddingX, bubblesActive);
            bubbles.RangeSlider(Bubbles.PaddingY, bubblesActive);

            bubbles.End();

            var pawnRules = new ListingPlus();

            pawnRules.Begin(hGrid[2]);
            pawnRules.Label(Lang.Get("Integration.PawnRules").Bold(), PawnRules.Description);
            var hasPawnRules = PawnRules.Instance.IsActive;
            if (!hasPawnRules) { pawnRules.LinkLabel(Lang.Get("Integration.GetMod"), PawnRules.Url, PawnRules.Url); }
            pawnRules.BoolToggle(PawnRules.ReplaceFoodSelector, hasPawnRules);
            pawnRules.BoolToggle(PawnRules.HideGizmo, hasPawnRules);

            pawnRules.End();
        }
    }
}
