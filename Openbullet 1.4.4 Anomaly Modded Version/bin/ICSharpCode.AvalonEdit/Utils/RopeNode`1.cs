// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.RopeNode`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Diagnostics;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
internal class RopeNode<T>
{
  internal const int NodeSize = 256 /*0x0100*/;
  internal static readonly RopeNode<T> emptyRopeNode = new RopeNode<T>()
  {
    isShared = true,
    contents = new T[256 /*0x0100*/]
  };
  internal RopeNode<T> left;
  internal RopeNode<T> right;
  internal volatile bool isShared;
  internal int length;
  internal byte height;
  internal T[] contents;

  internal int Balance => (int) this.right.height - (int) this.left.height;

  [Conditional("DATACONSISTENCYTEST")]
  internal void CheckInvariants()
  {
    if (this.height == (byte) 0)
    {
      if (this.contents != null)
        ;
    }
    else
    {
      int num = this.isShared ? 1 : 0;
    }
  }

  internal RopeNode<T> Clone()
  {
    if (this.height == (byte) 0)
    {
      if (this.contents == null)
        return this.GetContentNode().Clone();
      T[] objArray = new T[256 /*0x0100*/];
      this.contents.CopyTo((Array) objArray, 0);
      return new RopeNode<T>()
      {
        length = this.length,
        contents = objArray
      };
    }
    return new RopeNode<T>()
    {
      left = this.left,
      right = this.right,
      length = this.length,
      height = this.height
    };
  }

  internal RopeNode<T> CloneIfShared() => this.isShared ? this.Clone() : this;

  internal void Publish()
  {
    if (this.isShared)
      return;
    if (this.left != null)
      this.left.Publish();
    if (this.right != null)
      this.right.Publish();
    this.isShared = true;
  }

  internal static RopeNode<T> CreateFromArray(T[] arr, int index, int length)
  {
    return length == 0 ? RopeNode<T>.emptyRopeNode : RopeNode<T>.CreateNodes(length).StoreElements(0, arr, index, length);
  }

  internal static RopeNode<T> CreateNodes(int totalLength)
  {
    return RopeNode<T>.CreateNodes((totalLength + 256 /*0x0100*/ - 1) / 256 /*0x0100*/, totalLength);
  }

  private static RopeNode<T> CreateNodes(int leafCount, int totalLength)
  {
    RopeNode<T> nodes = new RopeNode<T>();
    nodes.length = totalLength;
    if (leafCount == 1)
    {
      nodes.contents = new T[256 /*0x0100*/];
    }
    else
    {
      int leafCount1 = leafCount / 2;
      int leafCount2 = leafCount - leafCount1;
      int totalLength1 = leafCount2 * 256 /*0x0100*/;
      nodes.left = RopeNode<T>.CreateNodes(leafCount2, totalLength1);
      nodes.right = RopeNode<T>.CreateNodes(leafCount1, totalLength - totalLength1);
      nodes.height = (byte) (1U + (uint) Math.Max(nodes.left.height, nodes.right.height));
    }
    return nodes;
  }

  internal void Rebalance()
  {
    if (this.left == null)
      return;
    while (Math.Abs(this.Balance) > 1)
    {
      if (this.Balance > 1)
      {
        if (this.right.Balance < 0)
        {
          this.right = this.right.CloneIfShared();
          this.right.RotateRight();
        }
        this.RotateLeft();
        this.left.Rebalance();
      }
      else if (this.Balance < -1)
      {
        if (this.left.Balance > 0)
        {
          this.left = this.left.CloneIfShared();
          this.left.RotateLeft();
        }
        this.RotateRight();
        this.right.Rebalance();
      }
    }
    this.height = (byte) (1U + (uint) Math.Max(this.left.height, this.right.height));
  }

  private void RotateLeft()
  {
    RopeNode<T> left1 = this.left;
    RopeNode<T> left2 = this.right.left;
    RopeNode<T> right = this.right.right;
    this.left = this.right.isShared ? new RopeNode<T>() : this.right;
    this.left.left = left1;
    this.left.right = left2;
    this.left.length = left1.length + left2.length;
    this.left.height = (byte) (1U + (uint) Math.Max(left1.height, left2.height));
    this.right = right;
    this.left.MergeIfPossible();
  }

  private void RotateRight()
  {
    RopeNode<T> left = this.left.left;
    RopeNode<T> right1 = this.left.right;
    RopeNode<T> right2 = this.right;
    this.right = this.left.isShared ? new RopeNode<T>() : this.left;
    this.right.left = right1;
    this.right.right = right2;
    this.right.length = right1.length + right2.length;
    this.right.height = (byte) (1U + (uint) Math.Max(right1.height, right2.height));
    this.left = left;
    this.right.MergeIfPossible();
  }

  private void MergeIfPossible()
  {
    if (this.length > 256 /*0x0100*/)
      return;
    this.height = (byte) 0;
    int length = this.left.length;
    if (this.left.isShared)
    {
      this.contents = new T[256 /*0x0100*/];
      this.left.CopyTo(0, this.contents, 0, length);
    }
    else
      this.contents = this.left.contents;
    this.left = (RopeNode<T>) null;
    this.right.CopyTo(0, this.contents, length, this.right.length);
    this.right = (RopeNode<T>) null;
  }

  internal RopeNode<T> StoreElements(int index, T[] array, int arrayIndex, int count)
  {
    RopeNode<T> ropeNode = this.CloneIfShared();
    if (ropeNode.height == (byte) 0)
    {
      Array.Copy((Array) array, arrayIndex, (Array) ropeNode.contents, index, count);
    }
    else
    {
      if (index + count <= ropeNode.left.length)
        ropeNode.left = ropeNode.left.StoreElements(index, array, arrayIndex, count);
      else if (index >= this.left.length)
      {
        ropeNode.right = ropeNode.right.StoreElements(index - ropeNode.left.length, array, arrayIndex, count);
      }
      else
      {
        int count1 = ropeNode.left.length - index;
        ropeNode.left = ropeNode.left.StoreElements(index, array, arrayIndex, count1);
        ropeNode.right = ropeNode.right.StoreElements(0, array, arrayIndex + count1, count - count1);
      }
      ropeNode.Rebalance();
    }
    return ropeNode;
  }

  internal void CopyTo(int index, T[] array, int arrayIndex, int count)
  {
    if (this.height == (byte) 0)
    {
      if (this.contents == null)
        this.GetContentNode().CopyTo(index, array, arrayIndex, count);
      else
        Array.Copy((Array) this.contents, index, (Array) array, arrayIndex, count);
    }
    else if (index + count <= this.left.length)
      this.left.CopyTo(index, array, arrayIndex, count);
    else if (index >= this.left.length)
    {
      this.right.CopyTo(index - this.left.length, array, arrayIndex, count);
    }
    else
    {
      int count1 = this.left.length - index;
      this.left.CopyTo(index, array, arrayIndex, count1);
      this.right.CopyTo(0, array, arrayIndex + count1, count - count1);
    }
  }

  internal RopeNode<T> SetElement(int offset, T value)
  {
    RopeNode<T> ropeNode = this.CloneIfShared();
    if (ropeNode.height == (byte) 0)
    {
      ropeNode.contents[offset] = value;
    }
    else
    {
      if (offset < ropeNode.left.length)
        ropeNode.left = ropeNode.left.SetElement(offset, value);
      else
        ropeNode.right = ropeNode.right.SetElement(offset - ropeNode.left.length, value);
      ropeNode.Rebalance();
    }
    return ropeNode;
  }

  internal static RopeNode<T> Concat(RopeNode<T> left, RopeNode<T> right)
  {
    if (left.length == 0)
      return right;
    if (right.length == 0)
      return left;
    if (left.length + right.length <= 256 /*0x0100*/)
    {
      left = left.CloneIfShared();
      right.CopyTo(0, left.contents, left.length, right.length);
      left.length += right.length;
      return left;
    }
    RopeNode<T> ropeNode = new RopeNode<T>();
    ropeNode.left = left;
    ropeNode.right = right;
    ropeNode.length = left.length + right.length;
    ropeNode.Rebalance();
    return ropeNode;
  }

  private RopeNode<T> SplitAfter(int offset)
  {
    RopeNode<T> ropeNode = new RopeNode<T>();
    ropeNode.contents = new T[256 /*0x0100*/];
    ropeNode.length = this.length - offset;
    Array.Copy((Array) this.contents, offset, (Array) ropeNode.contents, 0, ropeNode.length);
    this.length = offset;
    return ropeNode;
  }

  internal RopeNode<T> Insert(int offset, RopeNode<T> newElements)
  {
    if (offset == 0)
      return RopeNode<T>.Concat(newElements, this);
    if (offset == this.length)
      return RopeNode<T>.Concat(this, newElements);
    RopeNode<T> ropeNode = this.CloneIfShared();
    if (ropeNode.height == (byte) 0)
    {
      RopeNode<T> left = ropeNode;
      RopeNode<T> right = left.SplitAfter(offset);
      return RopeNode<T>.Concat(RopeNode<T>.Concat(left, newElements), right);
    }
    if (offset < ropeNode.left.length)
      ropeNode.left = ropeNode.left.Insert(offset, newElements);
    else
      ropeNode.right = ropeNode.right.Insert(offset - ropeNode.left.length, newElements);
    ropeNode.length += newElements.length;
    ropeNode.Rebalance();
    return ropeNode;
  }

  internal RopeNode<T> Insert(int offset, T[] array, int arrayIndex, int count)
  {
    if (this.length + count < 256 /*0x0100*/)
    {
      RopeNode<T> ropeNode = this.CloneIfShared();
      int num = ropeNode.length - offset;
      T[] contents = ropeNode.contents;
      for (int index = num; index >= 0; --index)
        contents[index + offset + count] = contents[index + offset];
      Array.Copy((Array) array, arrayIndex, (Array) contents, offset, count);
      ropeNode.length += count;
      return ropeNode;
    }
    if (this.height == (byte) 0)
      return this.Insert(offset, RopeNode<T>.CreateFromArray(array, arrayIndex, count));
    RopeNode<T> ropeNode1 = this.CloneIfShared();
    if (offset < ropeNode1.left.length)
      ropeNode1.left = ropeNode1.left.Insert(offset, array, arrayIndex, count);
    else
      ropeNode1.right = ropeNode1.right.Insert(offset - ropeNode1.left.length, array, arrayIndex, count);
    ropeNode1.length += count;
    ropeNode1.Rebalance();
    return ropeNode1;
  }

  internal RopeNode<T> RemoveRange(int index, int count)
  {
    if (index == 0 && count == this.length)
      return RopeNode<T>.emptyRopeNode;
    int num1 = index + count;
    RopeNode<T> ropeNode = this.CloneIfShared();
    if (ropeNode.height == (byte) 0)
    {
      int num2 = ropeNode.length - num1;
      for (int index1 = 0; index1 < num2; ++index1)
        ropeNode.contents[index + index1] = ropeNode.contents[num1 + index1];
      ropeNode.length -= count;
    }
    else
    {
      if (num1 <= ropeNode.left.length)
        ropeNode.left = ropeNode.left.RemoveRange(index, count);
      else if (index >= ropeNode.left.length)
      {
        ropeNode.right = ropeNode.right.RemoveRange(index - ropeNode.left.length, count);
      }
      else
      {
        int count1 = ropeNode.left.length - index;
        ropeNode.left = ropeNode.left.RemoveRange(index, count1);
        ropeNode.right = ropeNode.right.RemoveRange(0, count - count1);
      }
      if (ropeNode.left.length == 0)
        return ropeNode.right;
      if (ropeNode.right.length == 0)
        return ropeNode.left;
      ropeNode.length -= count;
      ropeNode.MergeIfPossible();
      ropeNode.Rebalance();
    }
    return ropeNode;
  }

  internal virtual RopeNode<T> GetContentNode()
  {
    throw new InvalidOperationException("Called GetContentNode() on non-FunctionNode.");
  }
}
