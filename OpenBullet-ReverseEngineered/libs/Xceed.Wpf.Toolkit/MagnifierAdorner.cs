// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.MagnifierAdorner
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class MagnifierAdorner : Adorner
{
  private Magnifier _magnifier;
  private Point _currentMousePosition;
  private double _currentZoomFactor;

  public MagnifierAdorner(UIElement element, Magnifier magnifier)
    : base(element)
  {
    this._magnifier = magnifier;
    this._currentZoomFactor = this._magnifier.ZoomFactor;
    this.UpdateViewBox();
    this.AddVisualChild((Visual) this._magnifier);
    this.Loaded += (RoutedEventHandler) ((s, e) => InputManager.Current.PostProcessInput += new ProcessInputEventHandler(this.OnProcessInput));
    this.Unloaded += (RoutedEventHandler) ((s, e) => InputManager.Current.PostProcessInput -= new ProcessInputEventHandler(this.OnProcessInput));
  }

  private void OnProcessInput(object sender, ProcessInputEventArgs e)
  {
    Point position = Mouse.GetPosition((IInputElement) this);
    if (this._currentMousePosition == position && this._magnifier.ZoomFactor == this._currentZoomFactor || this._magnifier.IsFrozen)
      return;
    this._currentMousePosition = position;
    this._currentZoomFactor = this._magnifier.ZoomFactor;
    this.UpdateViewBox();
    this.InvalidateArrange();
  }

  internal void UpdateViewBox()
  {
    this._magnifier.ViewBox = new Rect(this.CalculateViewBoxLocation(), this._magnifier.ViewBox.Size);
  }

  private Point CalculateViewBoxLocation()
  {
    Point position1 = Mouse.GetPosition((IInputElement) this);
    Point position2 = Mouse.GetPosition((IInputElement) this.AdornedElement);
    double num1 = position2.X - position1.X;
    double num2 = position2.Y - position1.Y;
    Vector offset = VisualTreeHelper.GetOffset((Visual) this._magnifier.Target);
    Point point = new Point(offset.X, offset.Y);
    return new Point(this._currentMousePosition.X - (this._magnifier.ViewBox.Width / 2.0 + num1) + point.X, this._currentMousePosition.Y - (this._magnifier.ViewBox.Height / 2.0 + num2) + point.Y);
  }

  protected override Visual GetVisualChild(int index) => (Visual) this._magnifier;

  protected override int VisualChildrenCount => 1;

  protected override Size MeasureOverride(Size constraint)
  {
    this._magnifier.Measure(constraint);
    return base.MeasureOverride(constraint);
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    this._magnifier.Arrange(new Rect(this._currentMousePosition.X - this._magnifier.Width / 2.0, this._currentMousePosition.Y - this._magnifier.Height / 2.0, this._magnifier.Width, this._magnifier.Height));
    return base.ArrangeOverride(finalSize);
  }
}
