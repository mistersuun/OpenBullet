// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.TextViewWeakEventManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public static class TextViewWeakEventManager
{
  public sealed class DocumentChanged : 
    WeakEventManagerBase<TextViewWeakEventManager.DocumentChanged, TextView>
  {
    protected override void StartListening(TextView source)
    {
      source.DocumentChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextView source)
    {
      source.DocumentChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class VisualLinesChanged : 
    WeakEventManagerBase<TextViewWeakEventManager.VisualLinesChanged, TextView>
  {
    protected override void StartListening(TextView source)
    {
      source.VisualLinesChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextView source)
    {
      source.VisualLinesChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class ScrollOffsetChanged : 
    WeakEventManagerBase<TextViewWeakEventManager.ScrollOffsetChanged, TextView>
  {
    protected override void StartListening(TextView source)
    {
      source.ScrollOffsetChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextView source)
    {
      source.ScrollOffsetChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }
}
