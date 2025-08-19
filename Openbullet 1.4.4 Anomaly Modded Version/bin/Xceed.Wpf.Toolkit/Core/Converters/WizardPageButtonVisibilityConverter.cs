// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.WizardPageButtonVisibilityConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class WizardPageButtonVisibilityConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values == null || values.Length != 2)
      throw new ArgumentException("Wrong number of arguments for WizardPageButtonVisibilityConverter.");
    Visibility visibility1 = values[0] == null || values[0] == DependencyProperty.UnsetValue ? Visibility.Hidden : (Visibility) values[0];
    WizardPageButtonVisibility buttonVisibility = values[1] == null || values[1] == DependencyProperty.UnsetValue ? WizardPageButtonVisibility.Hidden : (WizardPageButtonVisibility) values[1];
    Visibility visibility2 = Visibility.Visible;
    switch (buttonVisibility)
    {
      case WizardPageButtonVisibility.Inherit:
        visibility2 = visibility1;
        break;
      case WizardPageButtonVisibility.Collapsed:
        visibility2 = Visibility.Collapsed;
        break;
      case WizardPageButtonVisibility.Hidden:
        visibility2 = Visibility.Hidden;
        break;
      case WizardPageButtonVisibility.Visible:
        visibility2 = Visibility.Visible;
        break;
    }
    return (object) visibility2;
  }

  public object[] ConvertBack(
    object value,
    Type[] targetTypes,
    object parameter,
    CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
