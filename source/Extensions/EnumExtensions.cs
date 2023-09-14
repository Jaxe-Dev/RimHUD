using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Widgets;
using UnityEngine;

namespace RimHUD.Extensions
{
  public static class EnumExtensions
  {
    public static string GetLabel(this BarColorStyle self) => Lang.Get($"Layout.BarColorStyle.{self}");

    public static Color GetColor(this BarColorStyle self, float percentage) => self switch
    {
      BarColorStyle.LowOnly => Theme.BarLowColor.Value,
      BarColorStyle.MainToLow => Color.Lerp(Theme.BarMainColor.Value, Theme.BarLowColor.Value, percentage),
      BarColorStyle.MainOnly => Theme.BarMainColor.Value,
      _ => Color.Lerp(Theme.BarLowColor.Value, Theme.BarMainColor.Value, percentage)
    };

    public static TextStyle GetActual(this WidgetTextStyle self) => self switch
    {
      WidgetTextStyle.Small => Theme.SmallTextStyle,
      WidgetTextStyle.Large => Theme.LargeTextStyle,
      _ => Theme.RegularTextStyle
    };
  }
}
