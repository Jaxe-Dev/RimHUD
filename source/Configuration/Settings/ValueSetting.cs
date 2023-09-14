using System;

namespace RimHUD.Configuration.Settings
{
  public abstract class ValueSetting : BaseSetting
  {
    private readonly object _default;

    private object _object;
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

    private readonly Action<ValueSetting>? _onChange;

    protected ValueSetting(object @default, string label, string? tooltip, Action<ValueSetting>? onChange)
    {
      _default = @default;
      _object = @default;

      Label = label;
      Tooltip = tooltip;

      _onChange = onChange;
    }

    public override void Refresh() => _onChange?.Invoke(this);

    public override void ToDefault() => Object = _default;
  }
}
