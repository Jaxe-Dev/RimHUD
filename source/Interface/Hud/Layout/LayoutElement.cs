using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Compatibility;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public class LayoutElement
  {
    private const float Indent = WidgetsPlus.LargePadding;
    private const float ButtonSize = 18f;

    private readonly LayoutEditor _editor;
    public readonly LayoutElement Parent;

    public string Id { get; }

    public readonly string CustomLabel;

    public string Label => GetLabel();
    public string LabelAndId => GetLabelAndId();

    public LayoutElementType Type { get; }
    public Color Color => GetColor();

    public Def Def { get; }

    public string Variant { get; }

    private HudTarget _targets;
    public HudTarget Targets
    {
      get => _targets;
      set
      {
        if ((int)_targets == (int)value) { return; }
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

    public List<LayoutElement> Contents { get; } = new List<LayoutElement>();

    private bool _collapsed;
    private bool Selected => Equals(_editor.Selected);

    public int Index => Parent?.Contents.IndexOf(this) ?? -1;

    public bool CanMoveUp => Parent != null && Index > 0;
    public bool CanMoveDown => Parent != null && Index < Parent.Contents.LastIndex();
    public bool CanRemove => Parent != null;
    public bool IsRoot => _editor != null && Parent == null;

    public LayoutElement(LayoutElementType type, string id, Def def = null, string variant = null, LayoutEditor editor = null, LayoutElement parent = null, string label = null)
    {
      _editor = editor;
      Parent = parent;
      Type = type;
      Id = id;
      Def = def;
      Variant = variant;
      _targets = HudTarget.All;

      CustomLabel = label;
    }

    public LayoutElement(LayoutEditor editor, LayoutElement parent, BaseLayer layer)
    {
      _editor = editor;
      Parent = parent;

      Id = layer.Id;

      var type = layer.GetType();
      if (type.IsSubclassOf(typeof(ContainerLayer)))
      {
        _fillHeight = ((ContainerLayer)layer).FillHeight;
        if (type.IsSubclassOf(typeof(StackLayer))) { Type = LayoutElementType.Stack; }
        else if (type == typeof(PanelLayer)) { Type = LayoutElementType.Panel; }
      }
      else if (type == typeof(RowLayer)) { Type = LayoutElementType.Row; }
      else if (layer is WidgetLayer widget)
      {
        Variant = widget.Variant;
        if (widget.DefName == null)
        {
          Type = IntegrationManager.ThirdPartyWidgets.TryGetValue(Id, out var tuple) ? LayoutElementType.ThirdPartyWidget : LayoutElementType.Widget;
          CustomLabel = tuple.Item1;
        }
        else
        {
          if (widget.Id == HudContent.StatTypeName) { Def = DefDatabase<StatDef>.GetNamed(widget.DefName, false); }
          else if (widget.Id == HudContent.RecordTypeName) { Def = DefDatabase<RecordDef>.GetNamed(widget.DefName, false); }
          else if (widget.Id == HudContent.NeedTypeName) { Def = DefDatabase<NeedDef>.GetNamed(widget.DefName, false); }
          else if (widget.Id == HudContent.SkillTypeName) { Def = DefDatabase<SkillDef>.GetNamed(widget.DefName, false); }
          else if (widget.Id == HudContent.TrainingTypeName) { Def = DefDatabase<TrainableDef>.GetNamed(widget.DefName, false); }

          if (Def == null)
          {
            Mod.Error($"Unexpected DefName for {widget.Id}, using blank instead");
            Type = LayoutElementType.Widget;
            Id = BlankWidget.Id;
          }
        }
      }
      else { throw new Mod.Exception("Invalid layout element type"); }

      _targets = layer.Targets;
    }

    public void MoveUp()
    {
      if (Parent == null) { return; }

      var oldIndex = Parent.Contents.IndexOf(this);
      if (oldIndex == 0) { return; }

      Parent.Contents.RemoveAt(oldIndex);
      Parent.Contents.Insert(oldIndex - 1, this);
      _editor.Update();
    }

    public void MoveDown()
    {
      if (Parent == null) { return; }

      var oldIndex = Parent.Contents.IndexOf(this);
      if (oldIndex == Parent.Contents.LastIndex()) { return; }

      Parent.Contents.RemoveAt(oldIndex);
      Parent.Contents.Insert(oldIndex + 1, this);
      _editor.Update();
    }

    public void Remove()
    {
      Parent?.Contents.RemoveAt(Parent.Contents.IndexOf(this));
      _editor.Selected = null;
      _editor.Update();
    }

    public float Draw(float x, float y, float width)
    {
      var labelRect = new Rect(x + ButtonSize, y, width, Text.LineHeight);
      if (Verse.Widgets.ButtonInvisible(labelRect)) { _editor.Selected = this; }
      if (Selected) { Verse.Widgets.DrawBoxSolid(labelRect, Theme.ItemSelectedColor); }

      if (!IsRoot && Type != LayoutElementType.Widget && Type != LayoutElementType.DefWidget)
      {
        var buttonRect = new Rect(x, y, ButtonSize, ButtonSize);
        if (Verse.Widgets.ButtonImage(buttonRect, _collapsed ? Textures.Reveal : Textures.Collapse)) { _collapsed = !_collapsed; }
      }

      var label = IsRoot ? Lang.Get("Layout.Root").Bold() : GetLabel() + GetAttributes().SmallSize();

      WidgetsPlus.DrawText(labelRect, Type == LayoutElementType.Widget || Type == LayoutElementType.DefWidget || Type == LayoutElementType.ThirdPartyWidget ? label.Bold() : label, color: Color);
      Verse.Widgets.DrawHighlightIfMouseover(labelRect);
      var curY = y + Text.LineHeight;

      if (_collapsed || Contents.Count == 0) { return curY; }

      return Contents.Aggregate(curY, (current, item) => item.Draw(x + Indent, current, width - Indent));
    }

    private string GetLabel() => CustomLabel?.Colorize(Theme.ThirdPartyModColor) ?? (Def == null ? Lang.Get("Layout." + Id) : ColorizeByMod(Lang.Get("Layout." + Id, Def.GetLabelCap())));

    private string GetLabelAndId() => Def == null ? null : $"{Def.defName} [{ColorizeByMod(Def.modContentPack?.Name.Italic()) ?? "?"}]";

    private string ColorizeByMod(string text) => Def?.modContentPack?.IsOfficialMod ?? true ? text : text.Colorize(Theme.ThirdPartyModColor);

    private string GetAttributes()
    {
      var attributes = GetTargets();

      if (!Variant.NullOrEmpty()) { attributes = $" ({Lang.Get("Layout.Variant." + Variant)})" + attributes; }
      if (FillHeight) { attributes = $" {Lang.Get("Layout.ContainerFilled")}" + attributes; }

      return attributes;
    }

    private string GetTargets()
    {
      if (Targets == HudTarget.All) { return null; }

      var targets = new List<string>();
      if (Targets.HasTarget(HudTarget.PlayerHumanlike)) { targets.Add(Lang.Get("Layout.Target.PlayerHumanlike")); }
      if (Targets.HasTarget(HudTarget.PlayerCreature)) { targets.Add(Lang.Get("Layout.Target.PlayerCreature")); }
      if (Targets.HasTarget(HudTarget.OtherHumanlike)) { targets.Add(Lang.Get("Layout.Target.OtherHumanlike")); }
      if (Targets.HasTarget(HudTarget.OtherCreature)) { targets.Add(Lang.Get("Layout.Target.OtherCreature")); }

      return targets.Count > 0 ? $" ({targets.ToCommaList()})".Italic() : null;
    }

    private Color GetColor()
    {
      switch (Type)
      {
        case LayoutElementType.Stack:
          return Theme.StackColor;
        case LayoutElementType.Panel:
          return Theme.PanelColor;
        case LayoutElementType.Row:
          return Theme.RowColor;
        case LayoutElementType.Widget:
        case LayoutElementType.DefWidget:
        case LayoutElementType.ThirdPartyWidget:
          return Theme.WidgetColor;
        default:
          throw new Mod.Exception("Invalid layout element type");
      }
    }

    public XElement ToXml()
    {
      var xml = new XElement(Parent == null ? LayoutLayer.RootName : Id);
      if (FillHeight) { xml.Add(new XAttribute(StackLayer.FillAttributeName, true)); }
      if (Targets != HudTarget.All) { xml.Add(new XAttribute(BaseLayer.TargetAttribute, Targets.ToId())); }
      if (Def != null) { xml.Add(new XAttribute(WidgetLayer.DefNameAttribute, Def.defName)); }
      if (!Variant.NullOrEmpty()) { xml.Add(new XAttribute(WidgetLayer.VariantAttribute, Variant)); }

      foreach (var item in Contents) { xml.Add(item.ToXml()); }

      return xml;
    }
  }
}
