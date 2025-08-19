// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.MouseHoverLogic
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class MouseHoverLogic : IDisposable
{
  private UIElement target;
  private DispatcherTimer mouseHoverTimer;
  private Point mouseHoverStartPoint;
  private MouseEventArgs mouseHoverLastEventArgs;
  private bool mouseHovering;
  private bool disposed;

  public MouseHoverLogic(UIElement target)
  {
    this.target = target != null ? target : throw new ArgumentNullException(nameof (target));
    this.target.MouseLeave += new MouseEventHandler(this.MouseHoverLogicMouseLeave);
    this.target.MouseMove += new MouseEventHandler(this.MouseHoverLogicMouseMove);
    this.target.MouseEnter += new MouseEventHandler(this.MouseHoverLogicMouseEnter);
  }

  private void MouseHoverLogicMouseMove(object sender, MouseEventArgs e)
  {
    Vector vector = this.mouseHoverStartPoint - e.GetPosition((IInputElement) this.target);
    if (Math.Abs(vector.X) <= SystemParameters.MouseHoverWidth && Math.Abs(vector.Y) <= SystemParameters.MouseHoverHeight)
      return;
    this.StartHovering(e);
  }

  private void MouseHoverLogicMouseEnter(object sender, MouseEventArgs e) => this.StartHovering(e);

  private void StartHovering(MouseEventArgs e)
  {
    this.StopHovering();
    this.mouseHoverStartPoint = e.GetPosition((IInputElement) this.target);
    this.mouseHoverLastEventArgs = e;
    this.mouseHoverTimer = new DispatcherTimer(SystemParameters.MouseHoverTime, DispatcherPriority.Background, new EventHandler(this.OnMouseHoverTimerElapsed), this.target.Dispatcher);
    this.mouseHoverTimer.Start();
  }

  private void MouseHoverLogicMouseLeave(object sender, MouseEventArgs e) => this.StopHovering();

  private void StopHovering()
  {
    if (this.mouseHoverTimer != null)
    {
      this.mouseHoverTimer.Stop();
      this.mouseHoverTimer = (DispatcherTimer) null;
    }
    if (!this.mouseHovering)
      return;
    this.mouseHovering = false;
    this.OnMouseHoverStopped(this.mouseHoverLastEventArgs);
  }

  private void OnMouseHoverTimerElapsed(object sender, EventArgs e)
  {
    this.mouseHoverTimer.Stop();
    this.mouseHoverTimer = (DispatcherTimer) null;
    this.mouseHovering = true;
    this.OnMouseHover(this.mouseHoverLastEventArgs);
  }

  public event EventHandler<MouseEventArgs> MouseHover;

  protected virtual void OnMouseHover(MouseEventArgs e)
  {
    if (this.MouseHover == null)
      return;
    this.MouseHover((object) this, e);
  }

  public event EventHandler<MouseEventArgs> MouseHoverStopped;

  protected virtual void OnMouseHoverStopped(MouseEventArgs e)
  {
    if (this.MouseHoverStopped == null)
      return;
    this.MouseHoverStopped((object) this, e);
  }

  public void Dispose()
  {
    if (!this.disposed)
    {
      this.target.MouseLeave -= new MouseEventHandler(this.MouseHoverLogicMouseLeave);
      this.target.MouseMove -= new MouseEventHandler(this.MouseHoverLogicMouseMove);
      this.target.MouseEnter -= new MouseEventHandler(this.MouseHoverLogicMouseEnter);
    }
    this.disposed = true;
  }
}
