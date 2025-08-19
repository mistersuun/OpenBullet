// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.CaretWeakEventManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public static class CaretWeakEventManager
{
  public sealed class PositionChanged : 
    WeakEventManagerBase<CaretWeakEventManager.PositionChanged, Caret>
  {
    protected override void StartListening(Caret source)
    {
      source.PositionChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(Caret source)
    {
      source.PositionChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }
}
