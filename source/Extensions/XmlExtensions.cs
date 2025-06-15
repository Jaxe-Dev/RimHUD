using System.Xml.Linq;

namespace RimHUD.Extensions;

public static class XmlExtensions
{
  public static void AddAttribute(this XElement self, string name, string? value)
  {
    if (value.NullOrWhitespace()) { return; }
    self.Add(new XAttribute(name, value));
  }

  public static void AddAttribute<T>(this XElement self, string name, T value, T? @default = default)
  {
    if (!Equals(value, @default)) { self.AddAttribute(name, value?.ToString()); }
  }

  public static string? GetAttribute(this XElement self, string name) => self.Attribute(name)?.Value ?? null;
}
