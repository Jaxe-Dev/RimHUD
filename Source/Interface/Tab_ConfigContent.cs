using RimHUD.Data;
using UnityEngine;

namespace RimHUD.Interface
{
    internal class Tab_ConfigContent : Tab
    {
        public override string Label => Lang.Get("Dialog_Config.Tab.Content");
        public override string Tooltip => Lang.Get("Alert.NotImplemented");

        public Tab_ConfigContent() => Enabled = false;

        public override void Draw(Rect rect)
        { }
    }
}
