// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.CommonPropertyExceptionValidationRule
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal class CommonPropertyExceptionValidationRule : ValidationRule
{
  private TypeConverter _propertyTypeConverter;
  private Type _type;

  internal CommonPropertyExceptionValidationRule(Type type)
  {
    this._propertyTypeConverter = TypeDescriptor.GetConverter(type);
    this._type = type;
  }

  public override ValidationResult Validate(object value, CultureInfo cultureInfo)
  {
    ValidationResult validationResult = new ValidationResult(true, (object) null);
    if (GeneralUtilities.CanConvertValue(value, (object) this._type))
    {
      try
      {
        this._propertyTypeConverter.ConvertFrom(value);
      }
      catch (Exception ex)
      {
        validationResult = new ValidationResult(false, (object) ex.Message);
      }
    }
    return validationResult;
  }
}
