// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.CallbackOnDispose
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class CallbackOnDispose : IDisposable
{
  private Action action;

  public CallbackOnDispose(Action action)
  {
    this.action = action != null ? action : throw new ArgumentNullException(nameof (action));
  }

  public void Dispose()
  {
    Action action = Interlocked.Exchange<Action>(ref this.action, (Action) null);
    if (action == null)
      return;
    action();
  }
}
