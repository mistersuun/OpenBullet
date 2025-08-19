// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.FontComboBoxEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class FontComboBoxEditor : ComboBoxEditor
{
  protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
  {
    if (propertyItem.PropertyType == typeof (FontFamily))
      return (IEnumerable) FontUtilities.Families.OrderBy<FontFamily, string>((Func<FontFamily, string>) (x => x.Source));
    if (propertyItem.PropertyType == typeof (FontWeight))
      return (IEnumerable) FontUtilities.Weights;
    if (propertyItem.PropertyType == typeof (FontStyle))
      return (IEnumerable) FontUtilities.Styles;
    return propertyItem.PropertyType == typeof (FontStretch) ? (IEnumerable) FontUtilities.Stretches : (IEnumerable) null;
  }
}
