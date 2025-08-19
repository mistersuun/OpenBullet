// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Primitives.LinearClipper
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls.Primitives;

public class LinearClipper : Clipper
{
  public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(nameof (ExpandDirection), typeof (ExpandDirection), typeof (LinearClipper), new PropertyMetadata((object) ExpandDirection.Right, new PropertyChangedCallback(LinearClipper.OnExpandDirectionChanged)));

  public ExpandDirection ExpandDirection
  {
    get => (ExpandDirection) this.GetValue(LinearClipper.ExpandDirectionProperty);
    set => this.SetValue(LinearClipper.ExpandDirectionProperty, (object) value);
  }

  private static void OnExpandDirectionChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LinearClipper) d).OnExpandDirectionChanged((ExpandDirection) e.OldValue, (ExpandDirection) e.NewValue);
  }

  protected virtual void OnExpandDirectionChanged(
    ExpandDirection oldValue,
    ExpandDirection newValue)
  {
    this.ClipContent();
  }

  protected override void ClipContent()
  {
    if (this.ExpandDirection == ExpandDirection.Right)
    {
      double width = this.RenderSize.Width * this.RatioVisible;
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, width, this.RenderSize.Height)
      };
    }
    else if (this.ExpandDirection == ExpandDirection.Left)
    {
      double width = this.RenderSize.Width * this.RatioVisible;
      double x = this.RenderSize.Width - width;
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(x, 0.0, width, this.RenderSize.Height)
      };
    }
    else if (this.ExpandDirection == ExpandDirection.Up)
    {
      double height = this.RenderSize.Height * this.RatioVisible;
      double y = this.RenderSize.Height - height;
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, y, this.RenderSize.Width, height)
      };
    }
    else
    {
      if (this.ExpandDirection != ExpandDirection.Down)
        return;
      double height = this.RenderSize.Height * this.RatioVisible;
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, this.RenderSize.Width, height)
      };
    }
  }
}
