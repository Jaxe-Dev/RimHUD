using System.Linq;
using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal class HudHStack : HudStack
    {
        public const string Name = "HStack";
        public override string ElementName { get; } = Name;

        public HudHStack(XElement xe, bool? fillHeight) : base(xe, fillHeight)
        { }

        public override float Prepare(PawnModel model)
        {
            if ((Containers.Length == 0) || !IsTargetted(model)) { return 0f; }
            var maxHeight = Containers.Select(container => container.Prepare(model)).Max();
            return FillHeight ? -1f : maxHeight;
        }

        public override bool Draw(Rect rect)
        {
            if (Containers.Length == 0) { return false; }

            var grid = rect.GetHGrid(HudLayout.Padding, Enumerable.Repeat(-1f, Containers.Length).ToArray());
            var index = 1;
            foreach (var container in Containers)
            {
                container.Draw(grid[index]);
                index++;
            }

            return true;
        }
    }
}
