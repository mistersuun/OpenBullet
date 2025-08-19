// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.CollapsedLineSection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public sealed class CollapsedLineSection
{
  private const string ID = "";
  private DocumentLine start;
  private DocumentLine end;
  private HeightTree heightTree;

  internal CollapsedLineSection(HeightTree heightTree, DocumentLine start, DocumentLine end)
  {
    this.heightTree = heightTree;
    this.start = start;
    this.end = end;
  }

  public bool IsCollapsed => this.start != null;

  public DocumentLine Start
  {
    get => this.start;
    internal set => this.start = value;
  }

  public DocumentLine End
  {
    get => this.end;
    internal set => this.end = value;
  }

  public void Uncollapse()
  {
    if (this.start == null)
      return;
    this.heightTree.Uncollapse(this);
    this.start = (DocumentLine) null;
    this.end = (DocumentLine) null;
  }

  public override string ToString()
  {
    return $"[CollapsedSection Start={(this.start != null ? this.start.LineNumber.ToString() : "null")} End={(this.end != null ? this.end.LineNumber.ToString() : "null")}]";
  }
}
