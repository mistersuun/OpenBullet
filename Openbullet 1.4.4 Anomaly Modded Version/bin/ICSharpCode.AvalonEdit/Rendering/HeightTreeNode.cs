// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.HeightTreeNode
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class HeightTreeNode
{
  internal readonly DocumentLine documentLine;
  internal HeightTreeLineNode lineNode;
  internal HeightTreeNode left;
  internal HeightTreeNode right;
  internal HeightTreeNode parent;
  internal bool color;
  internal int totalCount;
  internal double totalHeight;
  internal List<CollapsedLineSection> collapsedSections;

  internal HeightTreeNode()
  {
  }

  internal HeightTreeNode(DocumentLine documentLine, double height)
  {
    this.documentLine = documentLine;
    this.totalCount = 1;
    this.lineNode = new HeightTreeLineNode(height);
    this.totalHeight = height;
  }

  internal HeightTreeNode LeftMost
  {
    get
    {
      HeightTreeNode leftMost = this;
      while (leftMost.left != null)
        leftMost = leftMost.left;
      return leftMost;
    }
  }

  internal HeightTreeNode RightMost
  {
    get
    {
      HeightTreeNode rightMost = this;
      while (rightMost.right != null)
        rightMost = rightMost.right;
      return rightMost;
    }
  }

  internal HeightTreeNode Successor
  {
    get
    {
      if (this.right != null)
        return this.right.LeftMost;
      HeightTreeNode successor = this;
      HeightTreeNode heightTreeNode;
      do
      {
        heightTreeNode = successor;
        successor = successor.parent;
      }
      while (successor != null && successor.right == heightTreeNode);
      return successor;
    }
  }

  internal bool IsDirectlyCollapsed => this.collapsedSections != null;

  internal void AddDirectlyCollapsed(CollapsedLineSection section)
  {
    if (this.collapsedSections == null)
    {
      this.collapsedSections = new List<CollapsedLineSection>();
      this.totalHeight = 0.0;
    }
    this.collapsedSections.Add(section);
  }

  internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
  {
    this.collapsedSections.Remove(section);
    if (this.collapsedSections.Count != 0)
      return;
    this.collapsedSections = (List<CollapsedLineSection>) null;
    this.totalHeight = this.lineNode.TotalHeight;
    if (this.left != null)
      this.totalHeight += this.left.totalHeight;
    if (this.right == null)
      return;
    this.totalHeight += this.right.totalHeight;
  }
}
