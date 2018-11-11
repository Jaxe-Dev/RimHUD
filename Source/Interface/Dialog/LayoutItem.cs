using System.Collections.Generic;
using RimHUD.Data;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog
{
    internal class LayoutItem
    {
        private const float Indent = GUIPlus.LargePadding;
        private const float ButtonSize = 18f;

        private static readonly Color StackColor = new Color(1f, 1f, 0.5f);
        private static readonly Color PanelColor = new Color(1f, 0.75f, 0.5f);
        private static readonly Color RowColor = new Color(0.75f, 0.75f, 0.75f);
        private static readonly Color ElementColor = new Color(1f, 1f, 1f);

        private readonly LayoutView _view;
        private readonly LayoutItem _parent;
        public string Id { get; }
        private readonly LayoutItemType _type;
        public Color Color => GetColor();
        private readonly Def _def;
        private readonly HudTarget _targets;
        private readonly bool _fillHeight;

        public List<LayoutItem> Contents { get; } = new List<LayoutItem>();

        private bool _collapsed;
        private bool Selected => Equals(_view.Selected);

        public bool CanMoveUp => (_parent != null) && (_parent.Contents.IndexOf(this) > 0);
        public bool CanMoveDown => (_parent != null) && (_parent.Contents.IndexOf(this) < _parent.Contents.LastIndex());

        public LayoutItem(LayoutItemType type, string id, Def def = null)
        {
            _view = null;
            _parent = null;
            _type = type;
            Id = id;
            _def = def;
        }

        public LayoutItem(LayoutView view, LayoutItem parent, HudComponent component)
        {
            _view = view;
            _parent = parent;

            var type = component.GetType();
            if (type.IsSubclassOf(typeof(HudContainer)))
            {
                _fillHeight = ((HudContainer) component).FillHeight;
                if (type.IsSubclassOf(typeof(HudStack))) { _type = LayoutItemType.Stack; }
                else if (type == typeof(HudPanel)) { _type = LayoutItemType.Panel; }
            }
            else if (type == typeof(HudRow)) { _type = LayoutItemType.Row; }
            else if (component is HudElement element)
            {
                _type = element.DefName == null ? LayoutItemType.Element : LayoutItemType.CustomElement;
                if (element.DefName != null) { _def = DefDatabase<Def>.GetNamed(element.DefName); }
            }
            else { throw new Mod.Exception($"Invalid {nameof(LayoutItem)} type"); }

            Id = component.ElementName;
            _targets = component.Targets;
        }

        public void MoveUp()
        {
            if (_parent == null) { return; }

            var oldIndex = _parent.Contents.IndexOf(this);
            if (oldIndex == 0) { return; }

            _parent.Contents.RemoveAt(oldIndex);
            _parent.Contents.Insert(oldIndex - 1, this);
        }

        public void MoveDown()
        {
            if (_parent == null) { return; }

            var oldIndex = _parent.Contents.IndexOf(this);
            if (oldIndex == _parent.Contents.LastIndex()) { return; }

            _parent.Contents.Insert(oldIndex + 1, this);
            _parent.Contents.RemoveAt(oldIndex);
        }

        public void Remove() => _parent?.Contents.RemoveAt(_parent.Contents.IndexOf(this));

        public void Select() => _view.Selected = this;

        public float Draw(float x, float y, float width)
        {
            var labelRect = new Rect(x + ButtonSize, y, width, Text.LineHeight);
            if (Widgets.ButtonInvisible(labelRect)) { Select(); }
            if (Selected) { Widgets.DrawBoxSolid(labelRect, GUIPlus.ItemSelectedColor); }

            if (_type != LayoutItemType.Element)
            {
                var buttonRect = new Rect(x, y, ButtonSize, ButtonSize);
                if (Widgets.ButtonImage(buttonRect, _collapsed ? Textures.Reveal : Textures.Collapse)) { _collapsed = !_collapsed; }
            }

            var label = GetLabel() + GetAttributes().Size(Theme.SmallTextStyle.ActualSize);
            GUIPlus.DrawText(labelRect, _type == LayoutItemType.Element ? label.Bold() : label, Color);
            Widgets.DrawHighlightIfMouseover(labelRect);
            var curY = y + Text.LineHeight;

            if (_collapsed || (Contents.Count == 0)) { return curY; }

            foreach (var item in Contents) { curY = item.Draw(x + Indent, curY, width - Indent); }

            return curY;
        }

        private string GetLabel() => _def != null ? Lang.Get("Model.Component." + Id, _def.LabelCap) : Lang.Get("Model.Component." + Id);
        private string GetAttributes() => (_fillHeight ? $" {Lang.Get("Model.Component.Container.Filled")}" : null) + GetTargets();
        private string GetTargets()
        {
            if (_targets == HudTarget.All) { return null; }

            var targets = new List<string>();
            if (_targets.HasTarget(HudTarget.PlayerHumanlike)) { targets.Add(Lang.Get("Model.Target.PlayerHumanlike")); }
            if (_targets.HasTarget(HudTarget.PlayerCreature)) { targets.Add(Lang.Get("Model.Target.PlayerCreature")); }
            if (_targets.HasTarget(HudTarget.OtherHumanlike)) { targets.Add(Lang.Get("Model.Target.OtherHumanlike")); }
            if (_targets.HasTarget(HudTarget.OtherCreature)) { targets.Add(Lang.Get("Model.Target.OtherCreature")); }

            return $" ({targets.ToCommaList()})".Italic();
        }

        private Color GetColor()
        {
            if (_type == LayoutItemType.Stack) { return StackColor; }
            if (_type == LayoutItemType.Panel) { return PanelColor; }
            if (_type == LayoutItemType.Row) { return RowColor; }
            if ((_type == LayoutItemType.Element) || (_type == LayoutItemType.CustomElement)) { return ElementColor; }

            throw new Mod.Exception($"Invalid {nameof(LayoutItemType)}");
        }
    }
}
