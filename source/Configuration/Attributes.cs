using System;
using RimHUD.Extensions;
using RimHUD.Interface;
using UnityEngine;

namespace RimHUD.Configuration
{
  public static class Attributes
  {
    [AttributeUsage(AttributeTargets.Property)]
    public class Option : Attribute
    {
      public Type Type { get; }
      public string Category { get; }
      public string Label { get; }

      public Option(string label, Type type = null) : this(null, label, type)
      { }

      public Option(Type type) : this(null, null, type)
      { }

      public Option(string category, string label, Type type = null)
      {
        Category = category;
        Label = label;
        Type = type;
      }

      public string ConvertToXml(object value)
      {
        if (Type == null) { return null; }
        return Type == typeof(Color) ? ((Color)value).ToHex() : value.ToString();
      }

      public object ConvertFromXml(string text)
      {
        if (Type == null) { return null; }

        object value;
        if (Type == typeof(string)) { value = text; }
        else if (Type == typeof(bool)) { value = text.ToBool(); }
        else if (Type == typeof(int)) { value = text.ToInt(); }
        else if (Type == typeof(Color)) { value = GUIPlus.HexToColor(text); }
        else { return null; }

        return value;
      }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IntegratedOptions : Attribute
    { }
  }
}
