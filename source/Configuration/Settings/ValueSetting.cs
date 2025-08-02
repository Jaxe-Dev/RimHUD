using System;

namespace RimHUD.Configuration.Settings;

public abstract class ValueSetting : BaseSetting
{
  protected readonly object Default;

  private readonly Action<ValueSetting>? _onChange;
  private readonly Func<BaseSetting, bool> _extraDefaultCheck;

  private object _object;
  protected object Object
  {
    get => _object;
    set
    {
      if (Equals(_object, value)) { return; }

      _object = value;
      if (!Presets.IsLoading && !IsDefault()) { Presets.ClearCurrent(); }
      _onChange?.Invoke(this);
    }
  }

  public string Label { get; }
  public string? Tooltip { get; }

  protected ValueSetting(object @default, string label, string? tooltip, Action<ValueSetting>? onChange, Func<BaseSetting, bool> extraDefaultCheck)
  {
    Default = @default;
    Label = label;
    Tooltip = tooltip;

    _object = @default;
    _onChange = onChange;
    _extraDefaultCheck = extraDefaultCheck;
  }

  public override void ToDefault() => Object = Default;
  public override bool IsDefault() => ExtraDefaultCheck() || Object.Equals(Default);

  protected bool ExtraDefaultCheck() => _extraDefaultCheck.Invoke(this);
}
