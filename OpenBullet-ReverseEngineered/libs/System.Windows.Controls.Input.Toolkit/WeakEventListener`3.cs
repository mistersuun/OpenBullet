// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.WeakEventListener`3
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

#nullable disable
namespace System.Windows.Controls;

internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
{
  private WeakReference _weakInstance;

  public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

  public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

  public WeakEventListener(TInstance instance)
  {
    this._weakInstance = (object) instance != null ? new WeakReference((object) instance) : throw new ArgumentNullException(nameof (instance));
  }

  public void OnEvent(TSource source, TEventArgs eventArgs)
  {
    TInstance target = (TInstance) this._weakInstance.Target;
    if ((object) target != null)
    {
      if (this.OnEventAction == null)
        return;
      this.OnEventAction(target, source, eventArgs);
    }
    else
      this.Detach();
  }

  public void Detach()
  {
    if (this.OnDetachAction == null)
      return;
    this.OnDetachAction(this);
    this.OnDetachAction = (Action<WeakEventListener<TInstance, TSource, TEventArgs>>) null;
  }
}
