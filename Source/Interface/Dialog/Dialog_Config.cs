using RimHUD.Data;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Dialog_Config : WindowPlus
    {
        private const float Padding = 8f;
        private const float TabWidth = 160f;
        private const float TabHeight = 30f;
        private const float ButtonWidth = 120f;
        private const float ButtonHeight = 40f;

        private readonly TabManager _tabs = new TabManager(TabWidth, TabHeight, new Tab_ConfigDesign(), new Tab_ConfigColors(), new Tab_ConfigContent(), new Tab_ConfigIntegration());

        private Dialog_Config() : base(Lang.Get("Dialog_Config.Title").Bold(), new Vector2(800f, 650f))
        {
            onlyOneOfTypeAllowed = true;
            absorbInputAroundWindow = false;
            preventCameraMotion = false;
            doCloseButton = false;
        }

        public static void Open() => Find.WindowStack.Add(new Dialog_Config());

        public override void PostClose() => Persistent.Save();

        protected override void DrawContent(Rect rect)
        {
            var grid = rect.GetVGrid(Padding, -1f, ButtonHeight);

            _tabs.Draw(grid[1]);

            var button = GUIPlus.DrawButtonRow(grid[2], ButtonWidth, Padding, Lang.Get("Dialog_Config.SetToDefault"), Lang.Get("Button.Close"));
            if (button == 1) { SetToDefault(); }
            else if (button == 2) { Close(); }
        }

        private static void SetToDefault() => Dialog_Alert.Open(Lang.Get("Alert.SetToDefault"), Dialog_Alert.Buttons.YesNo, Persistent.AllToDefault);
    }
}
