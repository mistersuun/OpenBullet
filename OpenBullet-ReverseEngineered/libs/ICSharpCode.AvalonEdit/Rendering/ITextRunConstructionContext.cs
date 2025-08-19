// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.ITextRunConstructionContext
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public interface ITextRunConstructionContext
{
  TextDocument Document { get; }

  TextView TextView { get; }

  VisualLine VisualLine { get; }

  TextRunProperties GlobalTextRunProperties { get; }

  StringSegment GetText(int offset, int length);
}
