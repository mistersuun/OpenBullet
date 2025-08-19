// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridColumnDropSeparator
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using MS.Internal;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class DataGridColumnDropSeparator : Separator
{
  private DataGridColumnHeader _referenceHeader;

  static DataGridColumnDropSeparator()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridColumnDropSeparator), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridColumnDropSeparator)));
    FrameworkElement.WidthProperty.OverrideMetadata(typeof (DataGridColumnDropSeparator), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnDropSeparator.OnCoerceWidth)));
    FrameworkElement.HeightProperty.OverrideMetadata(typeof (DataGridColumnDropSeparator), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnDropSeparator.OnCoerceHeight)));
  }

  private static object OnCoerceWidth(DependencyObject d, object baseValue)
  {
    return DoubleUtil.IsNaN((double) baseValue) ? (object) 2.0 : baseValue;
  }

  private static object OnCoerceHeight(DependencyObject d, object baseValue)
  {
    double num = (double) baseValue;
    DataGridColumnDropSeparator columnDropSeparator = (DataGridColumnDropSeparator) d;
    return columnDropSeparator._referenceHeader != null && DoubleUtil.IsNaN(num) ? (object) columnDropSeparator._referenceHeader.ActualHeight : baseValue;
  }

  internal DataGridColumnHeader ReferenceHeader
  {
    get => this._referenceHeader;
    set => this._referenceHeader = value;
  }
}
