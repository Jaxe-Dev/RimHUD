using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public sealed class LayoutElement
  {
    private const float ButtonSize = 18f;
    private const float Indent = GUIPlus.LargePadding;

    private static readonly Color MissingColor = Color.red;

    private readonly LayoutEditor? _editor;

    public LayoutElement? Parent { get; }

    private readonly string _id;

    private string? _label;
    public string Label => _label ??= GetLabel();
    public string? LabelAndId => GetLabelAndId();

    public LayoutElementType Type { get; }

    public HudArgs Args { get; }

    public Def? Def { get; }

    public LayerTarget Targets
    {
      get => Args.Targets;
      set
      {
        if (Args.Targets == value) { return; }
        Args.Targets = value;
        _editor?.Update();
      }
    }

    public bool FillHeight
    {
      get => Args.FillHeight;
      set
      {
        if (Args.FillHeight == value) { return; }
        Args.FillHeight = value;
        _editor?.Update();
      }
    }

    public List<LayoutElement> Children { get; } = new();

    private bool _collapsed;
    private bool Selected => Equals(_editor?.Selected);

    public int Index => Parent?.Children.IndexOf(this) ?? -1;

    public bool CanMoveUp => Parent is not null && Index > 0;
    public bool CanMoveDown => Parent is not null && Index < Parent.Children.LastIndex();
    public bool CanRemove => Parent is not null;
    public bool IsRoot => _editor is not null && Parent is null;

    private bool? _isMissing;
    private bool IsMissing => _isMissing ??= Type is LayoutElementType.Widget && !HudContent.IsValidId(_id, Args.DefName);

    private bool? _isExternal;
    private bool IsExternal => _isExternal ??= HudContent.IsExternalType(_id) && !IsMissing;

    public LayoutElement(LayoutElementType type, string id, Def? def = null)
    {
      Type = type;
      _id = id;

      Args = new HudArgs { DefName = def?.defName };
      Def = def;
    }

    private LayoutElement(LayoutElementType type, string id, HudArgs args, LayoutEditor editor, LayoutElement? parent = null) : this(type, id)
    {
      _editor = editor;
      Parent = parent;

      Args = args;
      Def = args.GetDefFromLayerId(id);
    }

    public LayoutElement(LayoutElement prototype, LayoutEditor editor, LayoutElement parent) : this(prototype.Type, prototype._id, prototype.Args, editor, parent)
    { }

    public LayoutElement(BaseLayer layer, LayoutEditor editor, LayoutElement? parent, IEnumerable<BaseLayer>? children = null) : this(layer.Type, layer.Id, layer.Args, editor, parent)
    {
      if (children is not null) { Children = new List<LayoutElement>(children.Select(child => child.GetLayoutItem(editor, this))); }
    }

    public void MoveUp()
    {
      if (_editor is null || Parent is null) { return; }

      var oldIndex = Parent.Children.IndexOf(this);
      if (oldIndex is 0) { return; }

      Parent.Children.RemoveAt(oldIndex);
      Parent.Children.Insert(oldIndex - 1, this);
      _editor.Update();
    }

    public void MoveDown()
    {
      if (_editor is null || Parent is null) { return; }

      var oldIndex = Parent.Children.IndexOf(this);
      if (oldIndex == Parent.Children.LastIndex()) { return; }

      Parent.Children.RemoveAt(oldIndex);
      Parent.Children.Insert(oldIndex + 1, this);
      _editor.Update();
    }

    public void Remove()
    {
      if (_editor is null) { return; }

      Parent?.Children.RemoveAt(Parent.Children.IndexOf(this));
      _editor.Selected = null;
      _editor.Update();
    }

    public float Draw(float x, float y, float width)
    {
      var labelRect = new Rect(x + ButtonSize, y, width, Text.LineHeight);
      if (Verse.Widgets.ButtonInvisible(labelRect)) { _editor!.Selected = this; }
      if (Selected) { Verse.Widgets.DrawBoxSolid(labelRect, Theme.ItemSelectedColor); }

      var isWidget = Type is LayoutElementType.Widget;
      if (!IsRoot && !isWidget)
      {
        var buttonRect = new Rect(x, y, ButtonSize, ButtonSize);
        if (Verse.Widgets.ButtonImage(buttonRect, _collapsed ? TexButton.Reveal : TexButton.Collapse)) { _collapsed = !_collapsed; }
      }

      var label = IsRoot ? Lang.Get("Layout.Root").Bold() : Label + GetAttributes()?.SmallSize();

      WidgetsPlus.DrawText(labelRect, isWidget ? label.Bold() : label, color: GetColor());
      Verse.Widgets.DrawHighlightIfMouseover(labelRect);
      var curY = y + Text.LineHeight;

      if (_collapsed || Children.Count is 0) { return curY; }

      return Children.Aggregate(curY, (current, item) => item.Draw(x + Indent, current, width - Indent));
    }

    public XElement ToXml()
    {
      var xml = new XElement(Parent is null ? LayoutLayer.RootName : _id);

      Args.DefName ??= Def?.defName;

      Args.ToXml(xml);

      foreach (var item in Children) { xml.Add(item.ToXml()); }

      return xml;
    }

    private string GetLabel()
    {
      if (IsMissing) { return $"<{_id}{(Args.DefName is null ? null : $"[{Args.DefName}]")}>".Colorize(MissingColor); }
      if (Def is null) { return IsExternal ? $"{_id}[Missing Def]".Colorize(MissingColor) : Lang.Get($"Layout.{_id}"); }

      return ColorizeByMod(IsExternal ? Def.GetDefNameOrLabel() : Lang.Get($"Layout.{_id}", Def.GetDefNameOrLabel()));
    }

    private string? GetLabelAndId() => Def is null ? null : $"{Def.defName} [{ColorizeByMod(Def.modContentPack!.Name.Italic())}]";

    private string ColorizeByMod(string text) => Def?.modContentPack?.IsOfficialMod ?? true ? text : text.Colorize(Theme.ExternalModColor);

    private string? GetAttributes()
    {
      var attributes = GetTargets();

      if (Args.BarColorStyle is not null) { attributes = $" ({Args.BarColorStyle.Value.GetLabel()}){attributes}"; }
      if (FillHeight) { attributes = $" {Lang.Get("Layout.ContainerFilled")}{attributes}"; }

      return attributes;
    }

    private string? GetTargets()
    {
      if (Targets is LayerTarget.All) { return null; }

      var targets = new List<string>();
      if (Targets.HasTarget(LayerTarget.PlayerHumanlike)) { targets.Add(Lang.Get("Layout.Target.PlayerHumanlike")); }
      if (Targets.HasTarget(LayerTarget.PlayerCreature)) { targets.Add(Lang.Get("Layout.Target.PlayerCreature")); }
      if (Targets.HasTarget(LayerTarget.OtherHumanlike)) { targets.Add(Lang.Get("Layout.Target.OtherHumanlike")); }
      if (Targets.HasTarget(LayerTarget.OtherCreature)) { targets.Add(Lang.Get("Layout.Target.OtherCreature")); }

      return targets.Count > 0 ? $" ({targets.ToCommaList()})".Italic() : null;
    }

    private Color GetColor()
    {
      if (IsMissing) { return Theme.MissingWidgetColor; }
      if (IsExternal) { return Theme.ExternalModColor; }

      return Type switch
      {
        LayoutElementType.Stack => Theme.StackColor,
        LayoutElementType.Panel => Theme.PanelColor,
        LayoutElementType.Row => Theme.RowColor,
        LayoutElementType.Widget => Theme.WidgetColor,
        _ => throw new Exception("Invalid layout element type.")
      };
    }
  }
}
