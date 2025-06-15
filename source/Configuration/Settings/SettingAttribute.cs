using System;
using RimHUD.Extensions;
using RimHUD.Interface;
using UnityEngine;

namespace RimHUD.Configuration.Settings;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SettingAttribute(string? category, string? label, Type? type = null) : Attribute
{
  public string? Category { get; } = category;

  public string? Label { get; } = label;

  public Type? Type { get; } = type;

  public SettingAttribute(string? label, Type? type = null) : this(null, label, type)
  { }

  public SettingAttribute(Type type) : this(null, null, type)
  { }

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
