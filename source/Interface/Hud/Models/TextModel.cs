using System;

namespace RimHUD.Interface.Hud.Models
{
  public class TextModel : IModelValue
  {
    public string Label => null;
    public string Value { get; }
    public Func<string> Tooltip { get; }
    public bool Hidden => false;

    public Action OnClick { get; }
    public Action OnHover => null;

    private TextModel(string value, Func<string> tooltip, Action onClick)
    {
      Value = value;
      Tooltip = tooltip;
      OnClick = onClick;
    }

    public static TextModel Create(string text, Func<string> tooltip = null, Action onClick = null) => text == null ? null : new TextModel(text, tooltip, onClick);
  }
}
