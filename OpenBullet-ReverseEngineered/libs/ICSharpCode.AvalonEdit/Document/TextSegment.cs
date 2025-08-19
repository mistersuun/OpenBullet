// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextSegment
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public class TextSegment : ISegment
{
  internal ISegmentTree ownerTree;
  internal TextSegment left;
  internal TextSegment right;
  internal TextSegment parent;
  internal bool color;
  internal int nodeLength;
  internal int totalNodeLength;
  internal int segmentLength;
  internal int distanceToMaxEnd;

  int ISegment.Offset => this.StartOffset;

  protected bool IsConnectedToCollection => this.ownerTree != null;

  public int StartOffset
  {
    get
    {
      TextSegment textSegment = this;
      int nodeLength = textSegment.nodeLength;
      if (textSegment.left != null)
        nodeLength += textSegment.left.totalNodeLength;
      for (; textSegment.parent != null; textSegment = textSegment.parent)
      {
        if (textSegment == textSegment.parent.right)
        {
          if (textSegment.parent.left != null)
            nodeLength += textSegment.parent.left.totalNodeLength;
          nodeLength += textSegment.parent.nodeLength;
        }
      }
      return nodeLength;
    }
    set
    {
      if (value < 0)
        throw new ArgumentOutOfRangeException(nameof (value), "Offset must not be negative");
      if (this.StartOffset == value)
        return;
      ISegmentTree ownerTree = this.ownerTree;
      if (ownerTree != null)
      {
        ownerTree.Remove(this);
        this.nodeLength = value;
        ownerTree.Add(this);
      }
      else
        this.nodeLength = value;
      this.OnSegmentChanged();
    }
  }

  public int EndOffset
  {
    get => this.StartOffset + this.Length;
    set
    {
      int num = value - this.StartOffset;
      this.Length = num >= 0 ? num : throw new ArgumentOutOfRangeException(nameof (value), "EndOffset must be greater or equal to StartOffset");
    }
  }

  public int Length
  {
    get => this.segmentLength;
    set
    {
      if (value < 0)
        throw new ArgumentOutOfRangeException(nameof (value), "Length must not be negative");
      if (this.segmentLength == value)
        return;
      this.segmentLength = value;
      if (this.ownerTree != null)
        this.ownerTree.UpdateAugmentedData(this);
      this.OnSegmentChanged();
    }
  }

  protected virtual void OnSegmentChanged()
  {
  }

  internal TextSegment LeftMost
  {
    get
    {
      TextSegment leftMost = this;
      while (leftMost.left != null)
        leftMost = leftMost.left;
      return leftMost;
    }
  }

  internal TextSegment RightMost
  {
    get
    {
      TextSegment rightMost = this;
      while (rightMost.right != null)
        rightMost = rightMost.right;
      return rightMost;
    }
  }

  internal TextSegment Successor
  {
    get
    {
      if (this.right != null)
        return this.right.LeftMost;
      TextSegment successor = this;
      TextSegment textSegment;
      do
      {
        textSegment = successor;
        successor = successor.parent;
      }
      while (successor != null && successor.right == textSegment);
      return successor;
    }
  }

  internal TextSegment Predecessor
  {
    get
    {
      if (this.left != null)
        return this.left.RightMost;
      TextSegment predecessor = this;
      TextSegment textSegment;
      do
      {
        textSegment = predecessor;
        predecessor = predecessor.parent;
      }
      while (predecessor != null && predecessor.left == textSegment);
      return predecessor;
    }
  }

  public override string ToString()
  {
    return $"[{this.GetType().Name} Offset={(object) this.StartOffset} Length={(object) this.Length} EndOffset={(object) this.EndOffset}]";
  }
}
