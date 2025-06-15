using System;

namespace RimHUD.Configuration.Settings;

public abstract class ValueSetting(object @default, string label, string? tooltip, Action<ValueSetting>? onChange) : BaseSetting
{
  private readonly object _default = @default;

  private object _object = @default;
  public object Object
  {
    get => _object;
    protected set
    {
      if (Equals(_object, value)) { return; }

      _object = value;
      onChange?.Invoke(this);
    }
  }

  public string Label { get; } = label;
  public string? Tooltip { get; } = tooltip;

  public override void Refresh() => onChange?.Invoke(this);

  public override void ToDefault() => Object = _default;
}
