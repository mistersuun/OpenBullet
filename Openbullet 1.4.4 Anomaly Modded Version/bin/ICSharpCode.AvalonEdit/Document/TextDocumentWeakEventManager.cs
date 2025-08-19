// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public static class TextDocumentWeakEventManager
{
  public sealed class UpdateStarted : 
    WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.UpdateStarted += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.UpdateStarted -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class UpdateFinished : 
    WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.UpdateFinished += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.UpdateFinished -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class Changing : 
    WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.Changing += new EventHandler<DocumentChangeEventArgs>(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.Changing -= new EventHandler<DocumentChangeEventArgs>(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class Changed : 
    WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.Changed += new EventHandler<DocumentChangeEventArgs>(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.Changed -= new EventHandler<DocumentChangeEventArgs>(((WeakEventManager) this).DeliverEvent);
    }
  }

  [Obsolete("The TextDocument.LineCountChanged event will be removed in a future version. Use PropertyChangedEventManager instead.")]
  public sealed class LineCountChanged : 
    WeakEventManagerBase<TextDocumentWeakEventManager.LineCountChanged, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.LineCountChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.LineCountChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  [Obsolete("The TextDocument.TextLengthChanged event will be removed in a future version. Use PropertyChangedEventManager instead.")]
  public sealed class TextLengthChanged : 
    WeakEventManagerBase<TextDocumentWeakEventManager.TextLengthChanged, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.TextLengthChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.TextLengthChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class TextChanged : 
    WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>
  {
    protected override void StartListening(TextDocument source)
    {
      source.TextChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(TextDocument source)
    {
      source.TextChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }
}
