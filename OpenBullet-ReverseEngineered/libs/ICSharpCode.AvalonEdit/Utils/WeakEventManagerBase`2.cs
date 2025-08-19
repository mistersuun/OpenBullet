// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.WeakEventManagerBase`2
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public abstract class WeakEventManagerBase<TManager, TEventSource> : WeakEventManager
  where TManager : WeakEventManagerBase<TManager, TEventSource>, new()
  where TEventSource : class
{
  public static void AddListener(TEventSource source, IWeakEventListener listener)
  {
    WeakEventManagerBase<TManager, TEventSource>.CurrentManager.ProtectedAddListener((object) source, listener);
  }

  public static void RemoveListener(TEventSource source, IWeakEventListener listener)
  {
    WeakEventManagerBase<TManager, TEventSource>.CurrentManager.ProtectedRemoveListener((object) source, listener);
  }

  protected sealed override void StartListening(object source)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    this.StartListening((TEventSource) source);
  }

  protected sealed override void StopListening(object source)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    this.StopListening((TEventSource) source);
  }

  protected abstract void StartListening(TEventSource source);

  protected abstract void StopListening(TEventSource source);

  protected static TManager CurrentManager
  {
    get
    {
      Type managerType = typeof (TManager);
      TManager manager = (TManager) WeakEventManager.GetCurrentManager(managerType);
      if ((object) manager == null)
      {
        manager = new TManager();
        WeakEventManager.SetCurrentManager(managerType, (WeakEventManager) manager);
      }
      return manager;
    }
  }
}
