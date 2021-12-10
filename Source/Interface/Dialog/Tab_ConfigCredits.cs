using RimHUD.Data.Extensions;
using RimHUD.Data.Storage;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigCredits : Tab
    {
        public override string Label { get; } = "About";
        public override TipSignal? Tooltip { get; } = null;

        private Vector2 _scrollPosition = Vector2.zero;
        private Rect _viewRect;

        public override void Reset()
        { }

        public override void Draw(Rect rect)
        {
            var vGrid = rect.GetVGrid(GUIPlus.LargePadding, 40f, -1f, 10f);

            var l = new ListingPlus();
            l.Begin(vGrid[1]);

            l.Label("This mod was brought to you by:".Bold());
            l.GapLine();
            l.End();

            l.BeginScrollView(vGrid[2], ref _scrollPosition, ref _viewRect);

            l.Label(Persistent.GetCredits());

            l.Gap();

            l.Label("A special thanks to everyone above and countless others who have helped along the way.");
            l.Label(("❤️".Color("FF0011") + " Jaxe".Italic()).Bold());

            l.Gap();

            l.EndScrollView(ref _viewRect);
        }
    }
}
