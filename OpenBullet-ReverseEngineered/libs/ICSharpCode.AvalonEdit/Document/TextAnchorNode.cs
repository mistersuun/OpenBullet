// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextAnchorNode
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class TextAnchorNode(TextAnchor anchor) : WeakReference((object) anchor)
{
  internal TextAnchorNode left;
  internal TextAnchorNode right;
  internal TextAnchorNode parent;
  internal bool color;
  internal int length;
  internal int totalLength;

  internal TextAnchorNode LeftMost
  {
    get
    {
      TextAnchorNode leftMost = this;
      while (leftMost.left != null)
        leftMost = leftMost.left;
      return leftMost;
    }
  }

  internal TextAnchorNode RightMost
  {
    get
    {
      TextAnchorNode rightMost = this;
      while (rightMost.right != null)
        rightMost = rightMost.right;
      return rightMost;
    }
  }

  internal TextAnchorNode Successor
  {
    get
    {
      if (this.right != null)
        return this.right.LeftMost;
      TextAnchorNode successor = this;
      TextAnchorNode textAnchorNode;
      do
      {
        textAnchorNode = successor;
        successor = successor.parent;
      }
      while (successor != null && successor.right == textAnchorNode);
      return successor;
    }
  }

  internal TextAnchorNode Predecessor
  {
    get
    {
      if (this.left != null)
        return this.left.RightMost;
      TextAnchorNode predecessor = this;
      TextAnchorNode textAnchorNode;
      do
      {
        textAnchorNode = predecessor;
        predecessor = predecessor.parent;
      }
      while (predecessor != null && predecessor.left == textAnchorNode);
      return predecessor;
    }
  }

  public override string ToString()
  {
    return $"[TextAnchorNode Length={(object) this.length} TotalLength={(object) this.totalLength} Target={this.Target}]";
  }
}
