// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.NoReadOnlySections
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class NoReadOnlySections : IReadOnlySectionProvider
{
  public static readonly NoReadOnlySections Instance = new NoReadOnlySections();

  public bool CanInsert(int offset) => true;

  public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
  {
    return segment != null ? ExtensionMethods.Sequence<ISegment>(segment) : throw new ArgumentNullException(nameof (segment));
  }
}
