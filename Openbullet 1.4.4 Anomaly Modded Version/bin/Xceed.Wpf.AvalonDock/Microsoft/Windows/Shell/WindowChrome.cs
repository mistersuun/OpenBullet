// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Shell.WindowChrome
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Shell;

public class WindowChrome : Freezable
{
  public static readonly DependencyProperty WindowChromeProperty = DependencyProperty.RegisterAttached(nameof (WindowChrome), typeof (WindowChrome), typeof (WindowChrome), new PropertyMetadata((object) null, new PropertyChangedCallback(WindowChrome._OnChromeChanged)));
  public static readonly DependencyProperty IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached("IsHitTestVisibleInChrome", typeof (bool), typeof (WindowChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty CaptionHeightProperty = DependencyProperty.Register(nameof (CaptionHeight), typeof (double), typeof (WindowChrome), new PropertyMetadata((object) 0.0, (PropertyChangedCallback) ((d, e) => ((WindowChrome) d)._OnPropertyChangedThatRequiresRepaint())), (ValidateValueCallback) (value => (double) value >= 0.0));
  public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(nameof (ResizeBorderThickness), typeof (Thickness), typeof (WindowChrome), new PropertyMetadata((object) new Thickness()), (ValidateValueCallback) (value => Utility.IsThicknessNonNegative((Thickness) value)));
  public static readonly DependencyProperty GlassFrameThicknessProperty = DependencyProperty.Register(nameof (GlassFrameThickness), typeof (Thickness), typeof (WindowChrome), new PropertyMetadata((object) new Thickness(), (PropertyChangedCallback) ((d, e) => ((WindowChrome) d)._OnPropertyChangedThatRequiresRepaint()), (CoerceValueCallback) ((d, o) => WindowChrome._CoerceGlassFrameThickness((Thickness) o))));
  public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof (CornerRadius), typeof (CornerRadius), typeof (WindowChrome), new PropertyMetadata((object) new CornerRadius(), (PropertyChangedCallback) ((d, e) => ((WindowChrome) d)._OnPropertyChangedThatRequiresRepaint())), (ValidateValueCallback) (value => Utility.IsCornerRadiusValid((CornerRadius) value)));
  private static readonly List<WindowChrome._SystemParameterBoundProperty> _BoundProperties = new List<WindowChrome._SystemParameterBoundProperty>()
  {
    new WindowChrome._SystemParameterBoundProperty()
    {
      DependencyProperty = WindowChrome.CornerRadiusProperty,
      SystemParameterPropertyName = "WindowCornerRadius"
    },
    new WindowChrome._SystemParameterBoundProperty()
    {
      DependencyProperty = WindowChrome.CaptionHeightProperty,
      SystemParameterPropertyName = "WindowCaptionHeight"
    },
    new WindowChrome._SystemParameterBoundProperty()
    {
      DependencyProperty = WindowChrome.ResizeBorderThicknessProperty,
      SystemParameterPropertyName = "WindowResizeBorderThickness"
    },
    new WindowChrome._SystemParameterBoundProperty()
    {
      DependencyProperty = WindowChrome.GlassFrameThicknessProperty,
      SystemParameterPropertyName = "WindowNonClientFrameThickness"
    }
  };

  public static Thickness GlassFrameCompleteThickness => new Thickness(-1.0);

  private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (DesignerProperties.GetIsInDesignMode(d))
      return;
    Window window = (Window) d;
    WindowChrome newValue = (WindowChrome) e.NewValue;
    WindowChromeWorker chrome = WindowChromeWorker.GetWindowChromeWorker(window);
    if (chrome == null)
    {
      chrome = new WindowChromeWorker();
      WindowChromeWorker.SetWindowChromeWorker(window, chrome);
    }
    chrome.SetWindowChrome(newValue);
  }

  public static WindowChrome GetWindowChrome(Window window)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    return (WindowChrome) window.GetValue(WindowChrome.WindowChromeProperty);
  }

  public static void SetWindowChrome(Window window, WindowChrome chrome)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    window.SetValue(WindowChrome.WindowChromeProperty, (object) chrome);
  }

  public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
  {
    Verify.IsNotNull<IInputElement>(inputElement, nameof (inputElement));
    return inputElement is DependencyObject dependencyObject ? (bool) dependencyObject.GetValue(WindowChrome.IsHitTestVisibleInChromeProperty) : throw new ArgumentException("The element must be a DependencyObject", nameof (inputElement));
  }

  public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
  {
    Verify.IsNotNull<IInputElement>(inputElement, nameof (inputElement));
    if (!(inputElement is DependencyObject dependencyObject))
      throw new ArgumentException("The element must be a DependencyObject", nameof (inputElement));
    dependencyObject.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, (object) hitTestVisible);
  }

  public double CaptionHeight
  {
    get => (double) this.GetValue(WindowChrome.CaptionHeightProperty);
    set => this.SetValue(WindowChrome.CaptionHeightProperty, (object) value);
  }

  public Thickness ResizeBorderThickness
  {
    get => (Thickness) this.GetValue(WindowChrome.ResizeBorderThicknessProperty);
    set => this.SetValue(WindowChrome.ResizeBorderThicknessProperty, (object) value);
  }

  private static object _CoerceGlassFrameThickness(Thickness thickness)
  {
    return !Utility.IsThicknessNonNegative(thickness) ? (object) WindowChrome.GlassFrameCompleteThickness : (object) thickness;
  }

  public Thickness GlassFrameThickness
  {
    get => (Thickness) this.GetValue(WindowChrome.GlassFrameThicknessProperty);
    set => this.SetValue(WindowChrome.GlassFrameThicknessProperty, (object) value);
  }

  public CornerRadius CornerRadius
  {
    get => (CornerRadius) this.GetValue(WindowChrome.CornerRadiusProperty);
    set => this.SetValue(WindowChrome.CornerRadiusProperty, (object) value);
  }

  public bool ShowSystemMenu { get; set; }

  protected override Freezable CreateInstanceCore() => (Freezable) new WindowChrome();

  public WindowChrome()
  {
    foreach (WindowChrome._SystemParameterBoundProperty boundProperty in WindowChrome._BoundProperties)
      BindingOperations.SetBinding((DependencyObject) this, boundProperty.DependencyProperty, (BindingBase) new Binding()
      {
        Source = (object) SystemParameters2.Current,
        Path = new PropertyPath(boundProperty.SystemParameterPropertyName, new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      });
  }

  private void _OnPropertyChangedThatRequiresRepaint()
  {
    EventHandler thatRequiresRepaint = this.PropertyChangedThatRequiresRepaint;
    if (thatRequiresRepaint == null)
      return;
    thatRequiresRepaint((object) this, EventArgs.Empty);
  }

  internal event EventHandler PropertyChangedThatRequiresRepaint;

  private struct _SystemParameterBoundProperty
  {
    public string SystemParameterPropertyName { get; set; }

    public DependencyProperty DependencyProperty { get; set; }
  }
}
