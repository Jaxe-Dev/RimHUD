using RimHUD.Data;
using RimHUD.Integration;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface
{
    internal class Tab_ConfigIntegration : Tab
    {
        public override string Label => Lang.Get("Dialog_Config.Tab.Integration");
        public override string Tooltip => null;

        public override void Draw(Rect rect)
        {
            var hGrid = rect.GetHGrid(GUIPlus.LargePadding, -1f, -1f);
            var l = new ListingPlus();
            l.Begin(hGrid[1]);

            l.Label(Lang.Get("Integration.PawnRules").Bold(), PawnRules.Description);
            if (!PawnRules.IsLoaded) { l.LinkLabel(Lang.Get("Integration.GetMod"), PawnRules.Url, PawnRules.Url); }
            l.BoolToggle(Theme.IntegrationPawnRulesHideGizmo, Union.PawnRules);
            l.GapLine();
            l.Gap();

            l.Label(Lang.Get("Integration.Bubbles").Bold(), Bubbles.Description);
            if (!Bubbles.IsLoaded) { l.LinkLabel(Lang.Get("Integration.GetMod"), Bubbles.Url, Bubbles.Url); }
            l.RangeSlider(Theme.IntegrationBubblesFadeStart, Union.Bubbles);
            l.RangeSlider(Theme.IntegrationBubblesFadeTime, Union.Bubbles);
            l.RangeSlider(Theme.IntegrationBubblesWidth, Union.Bubbles);
            l.RangeSlider(Theme.IntegrationBubblesPadding, Union.Bubbles);
            l.RangeSlider(Theme.IntegrationBubblesMaxPerPawn, Union.Bubbles);
            l.GapLine();
            l.Gap();
            l.Gap();
            l.Label(Lang.Get("Alert.MoreComingSoon").Italic());

            l.End();
        }
    }
}
