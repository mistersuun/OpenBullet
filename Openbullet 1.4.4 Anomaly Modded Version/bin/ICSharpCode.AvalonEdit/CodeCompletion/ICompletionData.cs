// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public interface ICompletionData
{
  ImageSource Image { get; }

  string Text { get; }

  object Content { get; }

  object Description { get; }

  double Priority { get; }

  void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs);
}
