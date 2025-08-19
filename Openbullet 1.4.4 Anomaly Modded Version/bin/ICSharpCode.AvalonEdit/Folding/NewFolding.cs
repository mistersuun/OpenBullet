// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.NewFolding
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public class NewFolding : ISegment
{
  public int StartOffset { get; set; }

  public int EndOffset { get; set; }

  public string Name { get; set; }

  public bool DefaultClosed { get; set; }

  public bool IsDefinition { get; set; }

  public NewFolding()
  {
  }

  public NewFolding(int start, int end)
  {
    this.StartOffset = start <= end ? start : throw new ArgumentException("'start' must be less than 'end'");
    this.EndOffset = end;
    this.Name = (string) null;
    this.DefaultClosed = false;
  }

  int ISegment.Offset => this.StartOffset;

  int ISegment.Length => this.EndOffset - this.StartOffset;
}
