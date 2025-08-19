// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.VisibilityToBoolConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class VisibilityToBoolConverter : IValueConverter
{
  private bool _inverted;
  private bool _not;

  public bool Inverted
  {
    get => this._inverted;
    set => this._inverted = value;
  }

  public bool Not
  {
    get => this._not;
    set => this._not = value;
  }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return !this.Inverted ? this.VisibilityToBool(value) : this.BoolToVisibility(value);
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return !this.Inverted ? this.BoolToVisibility(value) : this.VisibilityToBool(value);
  }

  private object VisibilityToBool(object value)
  {
    if (!(value is Visibility visibility))
      throw new InvalidOperationException(ErrorMessages.GetMessage("SuppliedValueWasNotVisibility"));
    return (object) (visibility == Visibility.Visible ^ this.Not);
  }

  private object BoolToVisibility(object value)
  {
    if (!(value is bool flag))
      throw new InvalidOperationException(ErrorMessages.GetMessage("SuppliedValueWasNotBool"));
    return (object) (Visibility) (flag ^ this.Not ? 0 : 2);
  }
}
