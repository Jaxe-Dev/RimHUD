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

            var hasBubbles = Mod_Bubbles.Instance.IsActive;
            bubbles.Label(Lang.Get("Integration.Bubbles").Bold(), Mod_Bubbles.Description);
            if (!hasBubbles) { bubbles.LinkLabel(Lang.Get("Integration.GetMod"), Mod_Bubbles.Url, Mod_Bubbles.Url); }

            var bubblesActive = hasBubbles && Mod_Bubbles.Activated.Value;
            bubbles.BoolToggle(Mod_Bubbles.Activated, hasBubbles);
            bubbles.BoolToggle(Mod_Bubbles.DoNonPlayer, bubblesActive);
            bubbles.BoolToggle(Mod_Bubbles.DoAnimals, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Mod_Bubbles.ScaleStart, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.MinScale, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.MaxWidth, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.Spacing, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.StartOffset, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.OffsetDirection, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Mod_Bubbles.StartOpacity, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.MouseOverOpacity, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.MinTime, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.FadeStart, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.FadeLength, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.MaxPerPawn, bubblesActive);
            bubbles.GapLine(5f);
            bubbles.RangeSlider(Mod_Bubbles.FontSize, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.PaddingX, bubblesActive);
            bubbles.RangeSlider(Mod_Bubbles.PaddingY, bubblesActive);

            bubbles.End();

            var pawnRules = new ListingPlus();

            pawnRules.Begin(hGrid[2]);
            pawnRules.Label(Lang.Get("Integration.PawnRules").Bold(), Mod_PawnRules.Description);
            var hasPawnRules = Mod_PawnRules.Instance.IsActive;
            if (!hasPawnRules) { pawnRules.LinkLabel(Lang.Get("Integration.GetMod"), Mod_PawnRules.Url, Mod_PawnRules.Url); }
            pawnRules.BoolToggle(Mod_PawnRules.ReplaceFoodSelector, hasPawnRules);
            pawnRules.BoolToggle(Mod_PawnRules.HideGizmo, hasPawnRules);

            pawnRules.End();
        }
    }
}
