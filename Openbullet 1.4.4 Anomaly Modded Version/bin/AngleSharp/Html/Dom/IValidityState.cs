// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IValidityState
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("ValidityState")]
public interface IValidityState
{
  [DomName("valueMissing")]
  bool IsValueMissing { get; }

  [DomName("typeMismatch")]
  bool IsTypeMismatch { get; }

  [DomName("patternMismatch")]
  bool IsPatternMismatch { get; }

  [DomName("tooLong")]
  bool IsTooLong { get; }

  [DomName("tooShort")]
  bool IsTooShort { get; }

  [DomName("badInput")]
  bool IsBadInput { get; }

  [DomName("rangeUnderflow")]
  bool IsRangeUnderflow { get; }

  [DomName("rangeOverflow")]
  bool IsRangeOverflow { get; }

  [DomName("stepMismatch")]
  bool IsStepMismatch { get; }

  [DomName("customError")]
  bool IsCustomError { get; }

  [DomName("valid")]
  bool IsValid { get; }
}
