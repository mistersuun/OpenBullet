// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.HeightTreeLineNode
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal struct HeightTreeLineNode
{
  internal double height;
  internal List<CollapsedLineSection> collapsedSections;

  internal HeightTreeLineNode(double height)
  {
    this.collapsedSections = (List<CollapsedLineSection>) null;
    this.height = height;
  }

  internal bool IsDirectlyCollapsed => this.collapsedSections != null;

  internal void AddDirectlyCollapsed(CollapsedLineSection section)
  {
    if (this.collapsedSections == null)
      this.collapsedSections = new List<CollapsedLineSection>();
    this.collapsedSections.Add(section);
  }

  internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
  {
    this.collapsedSections.Remove(section);
    if (this.collapsedSections.Count != 0)
      return;
    this.collapsedSections = (List<CollapsedLineSection>) null;
  }

  internal double TotalHeight => !this.IsDirectlyCollapsed ? this.height : 0.0;
}
