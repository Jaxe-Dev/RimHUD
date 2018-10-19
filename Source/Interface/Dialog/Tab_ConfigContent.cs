using RimHUD.Data;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class Tab_ConfigContent : Tab
    {
        public override string Label { get; } = Lang.Get("Dialog_Config.Tab.Content");
        public override TipSignal? Tooltip { get; } = Lang.Get("Alert.NotImplemented");

        public Tab_ConfigContent() => Enabled = false;

        public override void Reset()
        { }

        public override void Draw(Rect rect)
        { }
    }
}
