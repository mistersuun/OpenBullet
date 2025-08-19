// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.TextEditorWeakEventManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.ComponentModel;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit;

public static class TextEditorWeakEventManager
{
  public sealed class DocumentChanged : 
    WeakEventManagerBase<TextEditorWeakEventManager.DocumentChanged, ITextEditorComponent>
  {
    protected override void StartListening(ITextEditorComponent source)
    {
      source.DocumentChanged += new EventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(ITextEditorComponent source)
    {
      source.DocumentChanged -= new EventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }

  public sealed class OptionChanged : 
    WeakEventManagerBase<TextEditorWeakEventManager.OptionChanged, ITextEditorComponent>
  {
    protected override void StartListening(ITextEditorComponent source)
    {
      source.OptionChanged += new PropertyChangedEventHandler(((WeakEventManager) this).DeliverEvent);
    }

    protected override void StopListening(ITextEditorComponent source)
    {
      source.OptionChanged -= new PropertyChangedEventHandler(((WeakEventManager) this).DeliverEvent);
    }
  }
}
