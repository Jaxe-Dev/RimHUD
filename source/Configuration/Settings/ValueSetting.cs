using System;

namespace RimHUD.Configuration.Settings;

public abstract class ValueSetting : BaseSetting
{
  protected readonly object Default;

  private readonly bool _initialized;

  private object _object;
  protected object Object
  {
    get => _object;
    set
    {
      if (Equals(_object, value)) { return; }

      _object = value;

      if (!_initialized || Presets.IsLoading) { return; }

      OnChange();
    }
  }

  public string Label { get; }
  public string? Tooltip { get; }

  protected ValueSetting(object @default, string label, string? tooltip, Action<BaseSetting>? onChange, Func<BaseSetting, bool>? saveCheck, bool canIncludeInPreset) : base(ConvertOnChange(onChange), ConvertSaveCheck(saveCheck), canIncludeInPreset)
  {
    Default = @default;
    Label = label;
    Tooltip = tooltip;

    _object = @default;
    _initialized = true;
  }

  public override void ToDefault() => Object = Default;
  public override bool IsDefault() => Object.Equals(Default);
}
