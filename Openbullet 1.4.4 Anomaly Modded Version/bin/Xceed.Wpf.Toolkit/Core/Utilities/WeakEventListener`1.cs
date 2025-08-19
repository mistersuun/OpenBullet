// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.WeakEventListener`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class WeakEventListener<TArgs> : IWeakEventListener where TArgs : EventArgs
{
  private Action<object, TArgs> _callback;

  public WeakEventListener(Action<object, TArgs> callback)
  {
    this._callback = callback != null ? callback : throw new ArgumentNullException(nameof (callback));
  }

  public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    this._callback(sender, (TArgs) e);
    return true;
  }
}
