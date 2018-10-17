using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Data.Models;
using RimHUD.Patch;
using UnityEngine;

namespace RimHUD.Interface.HUD
{
    internal class HudVStack : HudStack
    {
        public const string Name = "VStack";
        protected override string ElementName { get; } = Name;

        private float[] _heights;

        public HudVStack(XElement xe, bool? fillHeight) : base(xe, fillHeight)
        { }

        public override float Prepare(PawnModel model)
        {
            if (Containers.Length == 0) { return 0f; }

            var list = new List<float>();
            var totalFixedHeight = 0f;
            var totalVisible = 0;
            foreach (var container in Containers)
            {
                var height = container.Prepare(model);
                list.Add(height);
                if (height != -1f) { totalFixedHeight += height; }
                if (height != 0f) { totalVisible++; }
            }

            _heights = list.ToArray();

            return FillHeight ? -1f : totalFixedHeight + (HudLayout.Padding * (totalVisible - 1));
        }

        public override bool Draw(Rect rect)
        {
            if (Containers.Length == 0) { return false; }

            var grid = rect.GetVGrid(HudLayout.Padding, _heights);
            var index = 1;
            foreach (var container in Containers)
            {
                if (_heights[index - 1] != 0f) { container.Draw(grid[index]); }
                index++;
            }

            return true;
        }
    }
}
