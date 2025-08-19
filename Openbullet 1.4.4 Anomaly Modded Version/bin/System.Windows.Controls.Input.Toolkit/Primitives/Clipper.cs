// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Primitives.Clipper
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

#nullable disable
namespace System.Windows.Controls.Primitives;

public abstract class Clipper : ContentControl
{
  public static readonly DependencyProperty RatioVisibleProperty = DependencyProperty.Register(nameof (RatioVisible), typeof (double), typeof (Clipper), new PropertyMetadata((object) 1.0, new PropertyChangedCallback(Clipper.OnRatioVisibleChanged)));

  public double RatioVisible
  {
    get => (double) this.GetValue(Clipper.RatioVisibleProperty);
    set => this.SetValue(Clipper.RatioVisibleProperty, (object) value);
  }

  private static void OnRatioVisibleChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Clipper) d).OnRatioVisibleChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnRatioVisibleChanged(double oldValue, double newValue)
  {
    if (newValue >= 0.0 && newValue <= 1.0)
      this.ClipContent();
    else if (newValue < 0.0)
    {
      this.RatioVisible = 0.0;
    }
    else
    {
      if (newValue <= 1.0)
        return;
      this.RatioVisible = 1.0;
    }
  }

  protected Clipper()
  {
    this.SizeChanged += (SizeChangedEventHandler) delegate
    {
      this.ClipContent();
    };
  }

  protected abstract void ClipContent();
}
