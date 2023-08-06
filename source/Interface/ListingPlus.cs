using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimHUD.Configuration;
using RimHUD.Extensions;
using UnityEngine;
using Verse;
using ColorOption = RimHUD.Configuration.ColorOption;

namespace RimHUD.Interface
{
  public class ListingPlus : Listing_Standard
  {
    private const float LabelWidth = 150f;
    private const float ValueWidth = 100f;
    private const float ElementPadding = 1f;

    private static readonly Color LinkHoverColor = new Color(0.3f, 0.7f, 1f);
    private static readonly Regex RangeSliderEntryRegex = new Regex(@"^[-]?\d{0,3}$");
    private static readonly Regex HexRegex = new Regex(@"^[A-Fa-f0-9]{0,8}$");

    public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect)
    {
      if (viewRect == default) { viewRect = new Rect(rect.x, rect.y, rect.width - WidgetsPlus.ScrollbarWidth, 99999f); }

      Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);

      Begin(viewRect);
    }

    public void EndScrollView(ref Rect viewRect)
    {
      End();
      Widgets.EndScrollView();
      viewRect.height = CurHeight;
    }

    public bool Label(string label, TipSignal? tooltip = null, GameFont? font = null, Color? color = null, Color? hoverColor = null)
    {
      GUIPlus.SetFont(font);

      var rect = GetRect(Text.CalcHeight(label, ColumnWidth));

      GUIPlus.SetColor(hoverColor != null && Mouse.IsOver(rect) ? hoverColor : color);

      Widgets.Label(rect, label);
      WidgetsPlus.DrawTooltip(rect, tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();
      GUIPlus.ResetFont();

      return Widgets.ButtonInvisible(rect);
    }

    public void LinkLabel(string text, string url, Color? color = null, Color? hoverColor = null)
    {
      if (!Label(text, color: color, hoverColor: hoverColor ?? LinkHoverColor) || string.IsNullOrWhiteSpace(url)) { return; }

      var menuText = $"Click to visit URL:\n{url.SmallSize().Italic()}";
      var menu = new List<FloatMenuOption> { new FloatMenuOption(menuText, () => Application.OpenURL(url)) };

      Find.WindowStack.Add(new FloatMenu(menu));
    }

    public void ColorOptionSelect(ColorOption colorOption, ref ColorOption selected, bool enabled = true)
    {
      GUIPlus.SetColor(colorOption.Value);
      if (RadioButton(colorOption.Label, selected == colorOption, tooltip: colorOption.Tooltip)) { selected = colorOption; }
      GUIPlus.ResetColor();
    }

    public void TextStyleEditor(TextStyle style, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      Label(style.Label.Bold());
      RangeSlider(style.Size, enabled);
      RangeSlider(style.Height, enabled);

      GUIPlus.ResetColor();
    }

    public void BoolToggle(BoolOption option, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      option.Value = CheckboxLabeled(option.Label, option.Value, option.Tooltip, enabled);
      GUIPlus.ResetColor();
    }

    public void RangeSlider(RangeOption range, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

      WidgetsPlus.DrawText(grid[1], range.Label);
      WidgetsPlus.DrawText(grid[2], range.ToString());

      var value = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
      if (enabled) { range.Value = value; }

      WidgetsPlus.DrawTooltip(grid[0], range.Tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();
    }

    public bool RangeSliderEntry(RangeOption range, ref string text, int id, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, ValueWidth, -1f);

      WidgetsPlus.DrawText(grid[1], range.Label);

      var entryName = "RangeSliderEntry_Text" + id;
      var isFocused = GUI.GetNameOfFocusedControl() == entryName;
      if (!isFocused) { text = range.Value.ToString(); }

      var original = text;

      GUI.SetNextControlName(entryName);
      var newText = Widgets.TextField(grid[2], text, 5, RangeSliderEntryRegex);
      if (enabled) { text = newText; }

      var textValue = text.ToInt();

      if (textValue.HasValue)
      {
        if (textValue.Value < range.Min) { range.Value = range.Min; }
        else if (textValue.Value > range.Max) { range.Value = range.Max; }
        else { range.Value = textValue.Value; }
      }

      var sliderName = "RangeSliderEntry_Slider" + id;
      GUI.SetNextControlName(sliderName);
      var sliderValue = Mathf.RoundToInt(Widgets.HorizontalSlider(grid[3], range.Value, range.Min, range.Max, true));
      if (enabled && range.Value != sliderValue)
      {
        range.Value = sliderValue;
        text = range.Value.ToString();
      }
      if (Widgets.ButtonInvisible(grid[3])) { GUI.FocusControl(sliderName); }

      WidgetsPlus.DrawTooltip(grid[0], range.Tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();

      return original != text;
    }

    public bool HexEntry(string label, ColorOption color, ref string text, string tooltip = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);

      var grid = GetRect(Text.LineHeight).GetHGrid(ElementPadding, LabelWidth, -1f);

      WidgetsPlus.DrawText(grid[1], label);

      if (text == null) { text = color.Value.ToHex(); }

      var original = text;

      text = Widgets.TextField(grid[2], text, 8, HexRegex);
      if (enabled) { color.Value = GUIPlus.HexToColor(text); }

      WidgetsPlus.DrawTooltip(grid[0], tooltip);
      Gap(verticalSpacing);

      GUIPlus.ResetColor();

      return original != text;
    }

    public bool CheckboxLabeled(string label, bool value, string tooltip = null, bool enabled = true)
    {
      GUIPlus.SetEnabledColor(enabled);
      var previous = value;
      CheckboxLabeled(label, ref value, tooltip);
      GUIPlus.ResetColor();

      return enabled ? value : previous;
    }

    public bool ButtonText(string label, string tooltip = null, GameFont? font = null, bool enabled = true)
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

      return rect.GetHGrid(WidgetsPlus.SmallPadding, widths);
    }
  }
}
