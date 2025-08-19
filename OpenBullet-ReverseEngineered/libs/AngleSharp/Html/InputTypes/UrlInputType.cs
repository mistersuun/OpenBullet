// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.UrlInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class UrlInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value ?? string.Empty;
    ValidationErrors errorsFrom = BaseInputType.GetErrorsFrom(current);
    if (BaseInputType.IsInvalidPattern(this.Input.Pattern, str))
      errorsFrom ^= ValidationErrors.PatternMismatch;
    if (UrlInputType.IsInvalidUrl(str) && !string.IsNullOrEmpty(str))
      errorsFrom ^= ValidationErrors.TypeMismatch | ValidationErrors.BadInput;
    return errorsFrom;
  }

  private static bool IsInvalidUrl(string value)
  {
    if (string.IsNullOrEmpty(value))
      return false;
    Url url = new Url(value);
    return url.IsInvalid || url.IsRelative;
  }
}
