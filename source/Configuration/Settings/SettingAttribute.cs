using System;
using RimHUD.Extensions;
using RimHUD.Interface;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SettingAttribute : Attribute
{
  public string? Category { get; }

  public string? Label { get; }

  public Type? Type { get; }

  public SettingAttribute(string? label, Type? type = null) : this(null, label, type)
  { }

  public SettingAttribute(Type type) : this(null, null, type)
  { }

  public SettingAttribute(string? category, string? label, Type? type = null)
  {
    Category = category;
    Label = label;
    Type = type;
  }

  public string? ConvertToXml(object value)
  {
    if (Type is null) { return null; }
    return Type == typeof(Color) ? ((Color)value).ToHex() : value.ToString();
  }

  public object? ConvertFromXml(string text)
  {
    if (Type is null) { return null; }

    object value;
    if (Type == typeof(string)) { value = text; }
    else if (Type == typeof(bool)) { value = text.ToBool() ?? false; }
    else if (Type == typeof(int)) { value = text.ToInt() ?? 0; }
    else if (Type == typeof(Color)) { value = GUIPlus.HexToColor(text); }
    else { return null; }

    return value;
  }
}
