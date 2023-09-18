using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimHUD.Configuration.Settings;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Tooltips;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
  public sealed class ListingPlus : Listing_Standard
  {
    private const float LabelWidth = 150f;
    private const float ValueWidth = 100f;
    private const float ElementPadding = 1f;

    private static readonly Color LinkHoverColor = new(0.3f, 0.7f, 1f);
    private static readonly Regex RangeSliderEntryRegex = new(@"^[-]?\d{0,3}$");
    private static readonly Regex HexRegex = new(@"^[A-Fa-f0-9]{0,8}$");

    public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect scrollRect)
    {
      if (scrollRect == default) { scrollRect = new Rect(rect.x, rect.y, rect.width - WidgetsPlus.ScrollbarWidth, 99999f); }

      Widgets.BeginScrollView(rect, ref scrollPosition, scrollRect);

      Begin(scrollRect);
    }

    public void EndScrollView(ref Rect scrollRect)
    {
      End();
      Widgets.EndScrollView();
      scrollRect.height = CurHeight;
    }

    public bool Label(string? label, Func<string>? tooltip = null, GameFont? font = null, Color? color = null, Color? hoverColor = null, bool? wrap = null)
    {
      GUIPlus.SetFont(font);
      var rect = GetRect(Text.CalcHeight(label, ColumnWidth));

      GUIPlus.SetColor(hoverColor is not null && Mouse.IsOver(rect) ? hoverColor : color);
      GUIPlus.SetWrap(wrap);

      Widgets.Label(rect, label);
      TooltipsPlus.DrawStandard(rect, tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetWrap();
      GUIPlus.ResetColor();
      GUIPlus.ResetFont();

      return Widgets.ButtonInvisible(rect);
    }

    public void LinkLabel(string text, string? url, Color? color = null, Color? hoverColor = null)
    {
      if (!Label(text, color: color, hoverColor: hoverColor ?? LinkHoverColor) || url.NullOrWhitespace()) { return; }

      var menuText = $"Click to visit URL:\n{url!.SmallSize().Italic()}";
      var menu = new List<FloatMenuOption> { new(menuText, () => Application.OpenURL(url)) };

      Find.WindowStack!.Add(new FloatMenu(menu));
    }

    public void ColorSettingSelect(ColorSetting setting, ref ColorSetting? selected)
    {
      GUIPlus.SetColor(setting.Value);
      if (RadioButton(setting.Label, selected == setting, tooltip: setting.Tooltip)) { selected = setting; }
      GUIPlus.ResetColor();
    }

    public void TextStyleEditor(TextStyle style, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      Label(style.Label?.Bold());
      RangeSlider(style.Size, enabled);
      RangeSlider(style.Height, enabled);

      GUIPlus.ResetColor();
    }

    public void BoolToggle(BoolSetting setting, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      setting.Value = CheckboxLabeled(setting.Label, setting.Value, setting.Tooltip, enabled);
      GUIPlus.ResetColor();
    }

    public void RangeSlider(RangeSetting setting, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

      WidgetsPlus.DrawText(grid[1], setting.Label);
      WidgetsPlus.DrawText(grid[2], setting.ToString());

      var value = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], setting.Value, setting.Min, setting.Max));

      if (enabled) { setting.Value = value; }

      TooltipsPlus.DrawSimple(grid[0], setting.Tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();
    }

    public bool RangeSliderEntry(RangeSetting range, ref string? text, int id, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

      WidgetsPlus.DrawText(grid[1], range.Label);

      var entryName = $"RangeSliderEntry_Text{id}";
      var isFocused = GUI.GetNameOfFocusedControl() == entryName;
      if (!isFocused) { text = range.Value.ToString(); }

      var original = text;

      GUI.SetNextControlName(entryName);
      var newText = Widgets.TextField(grid[2], text, 5, RangeSliderEntryRegex);
      if (enabled) { text = newText; }

      var textValue = text?.ToInt();

      if (textValue.HasValue)
      {
        if (textValue.Value < range.Min) { range.Value = range.Min; }
        else if (textValue.Value > range.Max) { range.Value = range.Max; }
        else { range.Value = textValue.Value; }
      }

      var sliderName = $"RangeSliderEntry_Slider{id}";
      GUI.SetNextControlName(sliderName);
      var sliderValue = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max));
      if (enabled && range.Value != sliderValue)
      {
        range.Value = sliderValue;
        text = range.Value.ToString();
      }
      if (Widgets.ButtonInvisible(grid[3])) { GUI.FocusControl(sliderName); }

      TooltipsPlus.DrawSimple(grid[0], range.Tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();

      return original != text;
    }

    public bool HexEntry(string label, ColorSetting color, ref string? text, string? tooltip = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, -1f);

      WidgetsPlus.DrawText(grid[1], label);

      text ??= color.Value.ToHex();

      var original = text;

      text = Widgets.TextField(grid[2], text, 8, HexRegex);
      if (enabled) { color.Value = GUIPlus.HexToColor(text); }

      TooltipsPlus.DrawSimple(grid[0], tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();

      return original != text;
    }

    public bool CheckboxLabeled(string label, bool value, string? tooltip = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      var previous = value;
      CheckboxLabeled(label, ref value, tooltip);
      GUIPlus.ResetColor();

      return enabled ? value : previous;
    }

    public bool ButtonText(string label, Func<string>? tooltip = null, GameFont? font = null, bool enabled = true)
    {
      var result = WidgetsPlus.DrawButton(GetRect(WidgetsPlus.ButtonHeight), label, tooltip, font, enabled);
      Gap(verticalSpacing);
      return result;
    }

    public Rect GetRemaining()
    {
      var height = listingRect.height - curY;

      NewColumnIfNeeded((float)Math.Floor(height));

      var result = new Rect(curX, curY, ColumnWidth, height);
      curY += height;
      return result;
    }

    public Rect[] GetButtonGrid(params float[] widths)
    {
      var rect = GetRect(WidgetsPlus.ButtonHeight);
      Gap(verticalSpacing);

      return rect.GetHGrid(GUIPlus.SmallPadding, widths);
    }
  }
}
