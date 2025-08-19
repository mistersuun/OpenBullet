// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.ValidityState
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class ValidityState : IValidityState
{
  private ValidationErrors _err;

  internal ValidityState() => this._err = ValidationErrors.None;

  public bool IsValueMissing
  {
    get => this._err.HasFlag((Enum) ValidationErrors.ValueMissing);
    set => this.Set(this.IsValueMissing, value, ValidationErrors.ValueMissing);
  }

  public bool IsTypeMismatch
  {
    get => this._err.HasFlag((Enum) ValidationErrors.TypeMismatch);
    set => this.Set(this.IsTypeMismatch, value, ValidationErrors.TypeMismatch);
  }

  public bool IsPatternMismatch
  {
    get => this._err.HasFlag((Enum) ValidationErrors.PatternMismatch);
    set => this.Set(this.IsPatternMismatch, value, ValidationErrors.PatternMismatch);
  }

  public bool IsBadInput
  {
    get => this._err.HasFlag((Enum) ValidationErrors.BadInput);
    set => this.Set(this.IsBadInput, value, ValidationErrors.BadInput);
  }

  public bool IsTooLong
  {
    get => this._err.HasFlag((Enum) ValidationErrors.TooLong);
    set => this.Set(this.IsTooLong, value, ValidationErrors.TooLong);
  }

  public bool IsTooShort
  {
    get => this._err.HasFlag((Enum) ValidationErrors.TooShort);
    set => this.Set(this.IsTooShort, value, ValidationErrors.TooShort);
  }

  public bool IsRangeUnderflow
  {
    get => this._err.HasFlag((Enum) ValidationErrors.RangeUnderflow);
    set => this.Set(this.IsRangeUnderflow, value, ValidationErrors.RangeUnderflow);
  }

  public bool IsRangeOverflow
  {
    get => this._err.HasFlag((Enum) ValidationErrors.RangeOverflow);
    set => this.Set(this.IsRangeOverflow, value, ValidationErrors.RangeOverflow);
  }

  public bool IsStepMismatch
  {
    get => this._err.HasFlag((Enum) ValidationErrors.StepMismatch);
    set => this.Set(this.IsStepMismatch, value, ValidationErrors.StepMismatch);
  }

  public bool IsCustomError
  {
    get => this._err.HasFlag((Enum) ValidationErrors.Custom);
    set => this.Set(this.IsCustomError, value, ValidationErrors.Custom);
  }

  public bool IsValid => this._err == ValidationErrors.None;

  public void Reset(ValidationErrors err = ValidationErrors.None) => this._err = err;

  private void Set(bool oldValue, bool newValue, ValidationErrors err)
  {
    if (newValue == oldValue)
      return;
    this._err ^= err;
  }
}
