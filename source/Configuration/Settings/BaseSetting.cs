using System;

namespace RimHUD.Configuration.Settings;

public abstract class BaseSetting
{
  public abstract void ToDefault();

  public abstract bool IsDefault();

  private readonly Action<BaseSetting>? _onChange;
  private readonly Func<BaseSetting, bool>? _saveCheck;
  private readonly bool _canIncludeInPreset;

  protected bool IsUpdating;

  protected BaseSetting(Action<BaseSetting>? onChange, Func<BaseSetting, bool>? saveCheck, bool canIncludeInPreset)
  {
    _onChange = onChange;
    _saveCheck = saveCheck;
    _canIncludeInPreset = canIncludeInPreset;
  }

  protected static Action<BaseSetting>? ConvertOnChange<T>(Action<T>? action) where T : BaseSetting => action is null ? null : setting => action.Invoke((T)setting);
  protected static Func<BaseSetting, bool>? ConvertSaveCheck<T>(Func<T, bool>? func) where T : BaseSetting => func is null ? null : setting => func.Invoke((T)setting);

  public bool IsSaved()
  {
    IsUpdating = true;
    var result = (_saveCheck?.Invoke(this) ?? false) || IsDefault();
    IsUpdating = false;

    return result;
  }

  protected void OnChange()
  {
    if (IsUpdating || Presets.IsLoading) { return; }

    if (_canIncludeInPreset && !IsSaved()) { Presets.Current = null; }

    _onChange?.Invoke(this);
  }
}
