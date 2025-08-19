// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.PatternInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class PatternInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    ValidationErrors errorsFrom = BaseInputType.GetErrorsFrom(current);
    if (BaseInputType.IsInvalidPattern(this.Input.Pattern, this.Input.Value ?? string.Empty))
      errorsFrom ^= ValidationErrors.PatternMismatch;
    return errorsFrom;
  }
}
