using System;

namespace RimHUD.Configuration.Settings;

public abstract class ValueSetting : BaseSetting
{
  private readonly object _default;

  private object _object;
  private readonly Action<ValueSetting>? _onChange;

  public object Object
  {
    get => _object;
    protected set
    {
      if (Equals(_object, value)) { return; }

      _object = value;
      _onChange?.Invoke(this);
    }
  }

  public string Label { get; }
  public string? Tooltip { get; }

  protected ValueSetting(object @default, string label, string? tooltip, Action<ValueSetting>? onChange)
  {
    _onChange = onChange;
    _default = @default;
    _object = @default;
    Label = label;
    Tooltip = tooltip;
  }

  public override void Refresh() => _onChange?.Invoke(this);

  public override void ToDefault() => Object = _default;
}
