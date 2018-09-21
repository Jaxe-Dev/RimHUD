using RimHUD.Data;
using UnityEngine;

namespace RimHUD.Interface
{
    internal class Tab_ConfigColors : Tab
    {
        public override string Label => Lang.Get("Dialog_Config.Tab.Colors");
        public override string Tooltip => Lang.Get("Alert.NotImplemented");

        public Tab_ConfigColors() => Enabled = false;

        public override void Draw(Rect rect)
        { }
    }
}
