using System.Collections.Generic;
using System.Xml.Linq;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Extensions;
using RimHUD.Data.Models;
using RimHUD.Interface.HUD;
using RimWorld;
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

        private readonly LayoutEditor _editor;
        private readonly LayoutItem _parent;
        public string Id { get; }
        public string Label => GetLabel();
        public LayoutItemType Type { get; }
        public Color Color => GetColor();
        public Def Def { get; }
        private HudTarget _targets;
        public HudTarget Targets
        {
            get => _targets;
            set
            {
                if ((int) _targets == (int) value) { return; }
                _targets = value;
                _editor.Update();
            }
        }

        private bool _fillHeight;
        public bool FillHeight
        {
            get => _fillHeight;
            set
            {
                if (_fillHeight == value) { return; }
                _fillHeight = value;
                _editor.Update();
            }
        }

        public List<LayoutItem> Contents { get; } = new List<LayoutItem>();

        private bool _collapsed;
        private bool Selected => Equals(_editor.Selected);

        public bool CanMoveUp => _parent != null && _parent.Contents.IndexOf(this) > 0;
        public bool CanMoveDown => _parent != null && _parent.Contents.IndexOf(this) < _parent.Contents.LastIndex();
        public bool CanRemove => _parent != null;
        public bool IsRoot => _editor != null && _parent == null;

        public LayoutItem(LayoutItemType type, string id, Def def = null, LayoutEditor editor = null, LayoutItem parent = null)
        {
            _editor = editor;
            _parent = parent;
            Type = type;
            Id = id;
            Def = def;
            _targets = HudTarget.All;
        }

        public LayoutItem(LayoutEditor editor, LayoutItem parent, HudComponent component)
        {
            _editor = editor;
            _parent = parent;

            Id = component.ElementName;

            var type = component.GetType();
            if (type.IsSubclassOf(typeof(HudContainer)))
            {
                _fillHeight = ((HudContainer) component).FillHeight;
                if (type.IsSubclassOf(typeof(HudStack))) { Type = LayoutItemType.Stack; }
                else if (type == typeof(HudPanel)) { Type = LayoutItemType.Panel; }
            }
            else if (type == typeof(HudRow)) { Type = LayoutItemType.Row; }
            else if (component is HudElement element)
            {
                Type = element.DefName == null ? LayoutItemType.Element : LayoutItemType.CustomElement;
                if (element.DefName != null)
                {
                    if (element.ElementName == HudModel.StatTypeName) { Def = DefDatabase<StatDef>.GetNamed(element.DefName, false); }
                    else if (element.ElementName == HudModel.RecordTypeName) { Def = DefDatabase<RecordDef>.GetNamed(element.DefName, false); }
                    else if (element.ElementName == HudModel.NeedTypeName) { Def = DefDatabase<NeedDef>.GetNamed(element.DefName, false); }
                    else if (element.ElementName == HudModel.SkillTypeName) { Def = DefDatabase<SkillDef>.GetNamed(element.DefName, false); }
                    else if (element.ElementName == HudModel.TrainingTypeName) { Def = DefDatabase<TrainableDef>.GetNamed(element.DefName, false); }

                    if (Def == null)
                    {
                        Mod.Error($"Unexpected DefName for {element.ElementName}, using blank instead");
                        Type = LayoutItemType.Element;
                        Id = HudBlank.Name;
                    }
                }
            }
            else { throw new Mod.Exception($"Invalid {nameof(LayoutItem)} type"); }

            _targets = component.Targets;
        }

        public void MoveUp()
        {
            if (_parent == null) { return; }

            var oldIndex = _parent.Contents.IndexOf(this);
            if (oldIndex == 0) { return; }

            _parent.Contents.RemoveAt(oldIndex);
            _parent.Contents.Insert(oldIndex - 1, this);
            _editor.Update();
        }

        public void MoveDown()
        {
            if (_parent == null) { return; }

            var oldIndex = _parent.Contents.IndexOf(this);
            if (oldIndex == _parent.Contents.LastIndex()) { return; }

            _parent.Contents.RemoveAt(oldIndex);
            _parent.Contents.Insert(oldIndex + 1, this);
            _editor.Update();
        }

        public void Remove()
        {
            _parent?.Contents.RemoveAt(_parent.Contents.IndexOf(this));
            _editor.Selected = null;
            _editor.Update();
        }

        public void Select() => _editor.Selected = this;

        public float Draw(float x, float y, float width)
        {
            var labelRect = new Rect(x + ButtonSize, y, width, Text.LineHeight);
            if (Widgets.ButtonInvisible(labelRect)) { Select(); }
            if (Selected) { Widgets.DrawBoxSolid(labelRect, GUIPlus.ItemSelectedColor); }

            if (!IsRoot && Type != LayoutItemType.Element && Type != LayoutItemType.CustomElement)
            {
                var buttonRect = new Rect(x, y, ButtonSize, ButtonSize);
                if (Widgets.ButtonImage(buttonRect, _collapsed ? Textures.Reveal : Textures.Collapse)) { _collapsed = !_collapsed; }
            }

            var label = IsRoot ? Lang.Get("Model.Component.Root").Bold() : GetLabel() + GetAttributes().Size(Theme.SmallTextStyle.ActualSize);
            GUIPlus.DrawText(labelRect, Type == LayoutItemType.Element || Type == LayoutItemType.CustomElement ? label.Bold() : label, Color);
            Widgets.DrawHighlightIfMouseover(labelRect);
            var curY = y + Text.LineHeight;

            if (_collapsed || Contents.Count == 0) { return curY; }

            foreach (var item in Contents) { curY = item.Draw(x + Indent, curY, width - Indent); }

            return curY;
        }

        private string GetLabel() => Def != null ? Lang.Get("Model.Component." + Id, Def.LabelCap) : Lang.Get("Model.Component." + Id);
        private string GetAttributes() => (FillHeight ? $" {Lang.Get("Model.Component.Container.Filled")}" : null) + GetTargets();

        private string GetTargets()
        {
            if (Targets == HudTarget.All) { return null; }

            var targets = new List<string>();
            if (Targets.HasTarget(HudTarget.PlayerHumanlike)) { targets.Add(Lang.Get("Model.Target.PlayerHumanlike")); }
            if (Targets.HasTarget(HudTarget.PlayerCreature)) { targets.Add(Lang.Get("Model.Target.PlayerCreature")); }
            if (Targets.HasTarget(HudTarget.OtherHumanlike)) { targets.Add(Lang.Get("Model.Target.OtherHumanlike")); }
            if (Targets.HasTarget(HudTarget.OtherCreature)) { targets.Add(Lang.Get("Model.Target.OtherCreature")); }

            return targets.Count > 0 ? $" ({targets.ToCommaList()})".Italic() : null;
        }

        private Color GetColor()
        {
            if (Type == LayoutItemType.Stack) { return StackColor; }
            if (Type == LayoutItemType.Panel) { return PanelColor; }
            if (Type == LayoutItemType.Row) { return RowColor; }
            if (Type == LayoutItemType.Element || Type == LayoutItemType.CustomElement) { return ElementColor; }

            throw new Mod.Exception($"Invalid {nameof(LayoutItemType)}");
        }

        public XElement ToXml()
        {
            var xml = new XElement(_parent == null ? HudLayout.RootName : Id);
            if (FillHeight) { xml.Add(new XAttribute(HudStack.FillAttributeName, true)); }
            if (Targets != HudTarget.All) { xml.Add(new XAttribute(HudComponent.TargetAttribute, Targets.ToId())); }
            if (Def != null) { xml.Add(new XAttribute(HudElement.DefNameAttribute, Def.defName)); }

            foreach (var item in Contents) { xml.Add(item.ToXml()); }

            return xml;
        }
    }
}
