// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.EmailInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using System.Text.RegularExpressions;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class EmailInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  private static readonly Regex email = new Regex("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");

  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value ?? string.Empty;
    ValidationErrors errorsFrom = BaseInputType.GetErrorsFrom(current);
    if (BaseInputType.IsInvalidPattern(this.Input.Pattern, str))
      errorsFrom ^= ValidationErrors.PatternMismatch;
    if (EmailInputType.IsInvalidEmail(this.Input.IsMultiple, str) && !string.IsNullOrEmpty(str))
      errorsFrom ^= ValidationErrors.TypeMismatch | ValidationErrors.BadInput;
    return errorsFrom;
  }

  private static bool IsInvalidEmail(bool multiple, string value)
  {
    if (!multiple)
      return !EmailInputType.email.IsMatch(value.Trim());
    string str1 = value;
    char[] chArray = new char[1]{ ',' };
    foreach (string str2 in str1.Split(chArray))
    {
      if (!EmailInputType.email.IsMatch(str2.Trim()))
        return true;
    }
    return false;
  }
}
