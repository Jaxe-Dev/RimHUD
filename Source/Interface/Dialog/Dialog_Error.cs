using System;
using RimHUD.Data;
using RimHUD.Data.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Dialog_Error : Window
    {
        private const float ButtonWidth = 120f;
        private Vector2 _scrollPosition = Vector2.zero;
        private Rect _scrollView;

        private readonly string _message;
        private readonly string _stacktrace;

        public override Vector2 InitialSize { get; } = new Vector2(800f, 300f);

        private Dialog_Error(Exception exception)
        {
            doCloseButton = false;
            closeOnAccept = true;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = true;
            draggable = true;

            _message = exception.Message;
            _stacktrace = BuildStacktrace(exception);

            if (exception.InnerException == null) { return; }

            var innerException = exception.InnerException;
            var level = 1;
            do
            {
                _message += $"\n{new string('+', level)} [{innerException.Source}] {innerException.Message}";
                _stacktrace += "\n\n" + BuildStacktrace(innerException);
                level++;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }

        public static void Open(Exception exception) => Find.WindowStack.Add(new Dialog_Error(exception));

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new ListingPlus();
            listing.Begin(inRect);
            listing.Label(Lang.Get("Dialog_Error.AutoDeactivate").Bold());
            listing.Label(_message);
            listing.Gap();
            listing.Label(Lang.Get("Dialog_Error.Stacktrace").Bold(), font: GameFont.Tiny);
            listing.End();

            var grid = inRect.GetVGrid(0f, listing.CurHeight, -1f, GUIPlus.SmallButtonHeight + GUIPlus.MediumPadding);

            Widgets.DrawMenuSection(grid[2]);

            var stacktraceRect = grid[2].ContractedBy(GUIPlus.SmallPadding);
            var stacktraceList = new ListingPlus();

            stacktraceList.BeginScrollView(stacktraceRect, ref _scrollPosition, ref _scrollView);
            stacktraceList.Label(_stacktrace, font: GameFont.Tiny);
            stacktraceList.EndScrollView(ref _scrollView);

            grid[3].yMin += GUIPlus.MediumPadding;
            var buttonGrid = grid[3].GetHGrid(GUIPlus.MediumPadding, ButtonWidth, -1f, ButtonWidth, ButtonWidth);

            if (GUIPlus.DrawButton(buttonGrid[1], Lang.Get("Dialog_Error.CopyToClipboard"), font: GameFont.Tiny))
            {
                GUIUtility.systemCopyBuffer = $"[[RimHUD Auto-deactivation report]]\n{_message}\n\n{_stacktrace}";
                Mod.Message("RimHUD Auto-deactivation details copied to clipboard");
            }
            if (GUIPlus.DrawButton(buttonGrid[3], Lang.Get("Dialog_Error.Reactivate"), font: GameFont.Tiny))
            {
                Close();
                State.Activated = true;
            }
            if (GUIPlus.DrawButton(buttonGrid[4], Lang.Get("Button.Close"), font: GameFont.Tiny)) { Close(); }
        }

        private static string BuildStacktrace(Exception exception) => $"[{exception.Source}: {exception.Message}]\n{exception.StackTrace}";
    }
}
