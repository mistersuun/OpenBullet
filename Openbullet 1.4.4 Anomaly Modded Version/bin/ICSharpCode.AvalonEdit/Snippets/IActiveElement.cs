// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.IActiveElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

public interface IActiveElement
{
  void OnInsertionCompleted();

  void Deactivate(SnippetEventArgs e);

  bool IsEditable { get; }

  ISegment Segment { get; }
}
