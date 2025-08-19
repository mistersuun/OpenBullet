// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.SelectiveScrollingGrid
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

public class SelectiveScrollingGrid : Grid
{
  public static readonly DependencyProperty SelectiveScrollingOrientationProperty = DependencyProperty.RegisterAttached("SelectiveScrollingOrientation", typeof (Microsoft.Windows.Controls.SelectiveScrollingOrientation), typeof (SelectiveScrollingGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) Microsoft.Windows.Controls.SelectiveScrollingOrientation.Both, new PropertyChangedCallback(SelectiveScrollingGrid.OnSelectiveScrollingOrientationChanged)));

  public static Microsoft.Windows.Controls.SelectiveScrollingOrientation GetSelectiveScrollingOrientation(
    DependencyObject obj)
  {
    return (Microsoft.Windows.Controls.SelectiveScrollingOrientation) obj.GetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty);
  }

  public static void SetSelectiveScrollingOrientation(
    DependencyObject obj,
    Microsoft.Windows.Controls.SelectiveScrollingOrientation value)
  {
    obj.SetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty, (object) value);
  }

  private static void OnSelectiveScrollingOrientationChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    UIElement element = d as UIElement;
    Microsoft.Windows.Controls.SelectiveScrollingOrientation newValue = (Microsoft.Windows.Controls.SelectiveScrollingOrientation) e.NewValue;
    ScrollViewer visualParent = Microsoft.Windows.Controls.DataGridHelper.FindVisualParent<ScrollViewer>(element);
    if (visualParent == null || element == null)
      return;
    Transform renderTransform = element.RenderTransform;
    if (renderTransform != null)
    {
      BindingOperations.ClearBinding((DependencyObject) renderTransform, TranslateTransform.XProperty);
      BindingOperations.ClearBinding((DependencyObject) renderTransform, TranslateTransform.YProperty);
    }
    if (newValue == Microsoft.Windows.Controls.SelectiveScrollingOrientation.Both)
    {
      element.RenderTransform = (Transform) null;
    }
    else
    {
      TranslateTransform target = new TranslateTransform();
      if (newValue != Microsoft.Windows.Controls.SelectiveScrollingOrientation.Horizontal)
        BindingOperations.SetBinding((DependencyObject) target, TranslateTransform.XProperty, (BindingBase) new Binding("ContentHorizontalOffset")
        {
          Source = (object) visualParent
        });
      if (newValue != Microsoft.Windows.Controls.SelectiveScrollingOrientation.Vertical)
        BindingOperations.SetBinding((DependencyObject) target, TranslateTransform.YProperty, (BindingBase) new Binding("ContentVerticalOffset")
        {
          Source = (object) visualParent
        });
      element.RenderTransform = (Transform) target;
    }
  }
}
