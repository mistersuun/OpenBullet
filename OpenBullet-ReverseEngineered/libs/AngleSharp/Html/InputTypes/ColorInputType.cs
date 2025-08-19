// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.ColorInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using System.Text.RegularExpressions;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class ColorInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  private static readonly Regex color = new Regex("^\\#[0-9A-Fa-f]{6}$");

  public override ValidationErrors Check(IValidityState current)
  {
    ValidationErrors errorsFrom = BaseInputType.GetErrorsFrom(current);
    if (ColorInputType.color.IsMatch(this.Input.Value ?? string.Empty))
      return ValidationErrors.None;
    ValidationErrors validationErrors = errorsFrom ^ ValidationErrors.BadInput;
    if (this.Input.IsRequired)
      validationErrors ^= ValidationErrors.ValueMissing;
    return validationErrors;
  }
}
