// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.DelayedEvents
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class DelayedEvents
{
  private Queue<DelayedEvents.EventCall> eventCalls = new Queue<DelayedEvents.EventCall>();

  public void DelayedRaise(EventHandler handler, object sender, EventArgs e)
  {
    if (handler == null)
      return;
    this.eventCalls.Enqueue(new DelayedEvents.EventCall(handler, sender, e));
  }

  public void RaiseEvents()
  {
    while (this.eventCalls.Count > 0)
      this.eventCalls.Dequeue().Call();
  }

  private struct EventCall(EventHandler handler, object sender, EventArgs e)
  {
    private EventHandler handler = handler;
    private object sender = sender;
    private EventArgs e = e;

    public void Call() => this.handler(this.sender, this.e);
  }
}
