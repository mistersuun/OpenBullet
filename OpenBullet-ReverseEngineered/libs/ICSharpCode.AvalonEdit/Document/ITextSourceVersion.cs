// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.ITextSourceVersion
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public interface ITextSourceVersion
{
  bool BelongsToSameDocumentAs(ITextSourceVersion other);

  int CompareAge(ITextSourceVersion other);

  IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other);

  int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement = AnchorMovementType.Default);
}
