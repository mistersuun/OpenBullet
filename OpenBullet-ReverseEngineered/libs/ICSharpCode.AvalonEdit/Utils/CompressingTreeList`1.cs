// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.CompressingTreeList`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public sealed class CompressingTreeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  internal const bool RED = true;
  internal const bool BLACK = false;
  private readonly Func<T, T, bool> comparisonFunc;
  private CompressingTreeList<T>.Node root;

  public CompressingTreeList(IEqualityComparer<T> equalityComparer)
  {
    this.comparisonFunc = equalityComparer != null ? new Func<T, T, bool>(equalityComparer.Equals) : throw new ArgumentNullException(nameof (equalityComparer));
  }

  public CompressingTreeList(Func<T, T, bool> comparisonFunc)
  {
    this.comparisonFunc = comparisonFunc != null ? comparisonFunc : throw new ArgumentNullException(nameof (comparisonFunc));
  }

  public void InsertRange(int index, int count, T item)
  {
    if (index < 0 || index > this.Count)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) this.Count);
    if (count < 0)
      throw new ArgumentOutOfRangeException(nameof (count), (object) count, "Value must not be negative");
    if (count == 0)
      return;
    if (this.Count + count < 0)
      throw new OverflowException("Cannot insert elements: total number of elements must not exceed int.MaxValue.");
    if (this.root == null)
    {
      this.root = new CompressingTreeList<T>.Node(item, count);
    }
    else
    {
      CompressingTreeList<T>.Node node = this.GetNode(ref index);
      if (this.comparisonFunc(node.value, item))
      {
        node.count += count;
        this.UpdateAugmentedData(node);
      }
      else if (index == node.count)
        this.InsertAsRight(node, new CompressingTreeList<T>.Node(item, count));
      else if (index == 0)
      {
        CompressingTreeList<T>.Node predecessor = node.Predecessor;
        if (predecessor != null && this.comparisonFunc(predecessor.value, item))
        {
          predecessor.count += count;
          this.UpdateAugmentedData(predecessor);
        }
        else
          this.InsertBefore(node, new CompressingTreeList<T>.Node(item, count));
      }
      else
      {
        node.count -= index;
        this.InsertBefore(node, new CompressingTreeList<T>.Node(node.value, index));
        this.InsertBefore(node, new CompressingTreeList<T>.Node(item, count));
        this.UpdateAugmentedData(node);
      }
    }
  }

  private void InsertBefore(CompressingTreeList<T>.Node node, CompressingTreeList<T>.Node newNode)
  {
    if (node.left == null)
      this.InsertAsLeft(node, newNode);
    else
      this.InsertAsRight(node.left.RightMost, newNode);
  }

  public void RemoveRange(int index, int count)
  {
    if (index < 0 || index > this.Count)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) this.Count);
    if (count < 0 || index + count > this.Count)
      throw new ArgumentOutOfRangeException(nameof (count), (object) count, $"0 <= length, index({(object) index})+count <= {(object) this.Count}");
    if (count == 0)
      return;
    CompressingTreeList<T>.Node node1 = this.GetNode(ref index);
    if (index + count < node1.count)
    {
      node1.count -= count;
      this.UpdateAugmentedData(node1);
    }
    else
    {
      CompressingTreeList<T>.Node node2;
      if (index > 0)
      {
        count -= node1.count - index;
        node1.count = index;
        this.UpdateAugmentedData(node1);
        node2 = node1;
        node1 = node1.Successor;
      }
      else
        node2 = node1.Predecessor;
      CompressingTreeList<T>.Node successor;
      for (; node1 != null && count >= node1.count; node1 = successor)
      {
        count -= node1.count;
        successor = node1.Successor;
        this.RemoveNode(node1);
      }
      if (count > 0)
      {
        node1.count -= count;
        this.UpdateAugmentedData(node1);
      }
      if (node1 == null || node2 == null || !this.comparisonFunc(node2.value, node1.value))
        return;
      node2.count += node1.count;
      this.RemoveNode(node1);
      this.UpdateAugmentedData(node2);
    }
  }

  public void SetRange(int index, int count, T item)
  {
    this.RemoveRange(index, count);
    this.InsertRange(index, count, item);
  }

  private CompressingTreeList<T>.Node GetNode(ref int index)
  {
    CompressingTreeList<T>.Node node = this.root;
    while (true)
    {
      for (; node.left == null || index >= node.left.totalCount; node = node.right)
      {
        if (node.left != null)
          index -= node.left.totalCount;
        if (index < node.count || node.right == null)
          return node;
        index -= node.count;
      }
      node = node.left;
    }
  }

  private void UpdateAugmentedData(CompressingTreeList<T>.Node node)
  {
    int count = node.count;
    if (node.left != null)
      count += node.left.totalCount;
    if (node.right != null)
      count += node.right.totalCount;
    if (node.totalCount == count)
      return;
    node.totalCount = count;
    if (node.parent == null)
      return;
    this.UpdateAugmentedData(node.parent);
  }

  public T this[int index]
  {
    get
    {
      return index >= 0 && index < this.Count ? this.GetNode(ref index).value : throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) (this.Count - 1));
    }
    set
    {
      this.RemoveAt(index);
      this.Insert(index, value);
    }
  }

  public int Count => this.root != null ? this.root.totalCount : 0;

  bool ICollection<T>.IsReadOnly => false;

  public int IndexOf(T item)
  {
    int num = 0;
    if (this.root != null)
    {
      for (CompressingTreeList<T>.Node node = this.root.LeftMost; node != null; node = node.Successor)
      {
        if (this.comparisonFunc(node.value, item))
          return num;
        num += node.count;
      }
    }
    return -1;
  }

  public int GetStartOfRun(int index)
  {
    int index1 = index >= 0 && index < this.Count ? index : throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) (this.Count - 1));
    this.GetNode(ref index1);
    return index - index1;
  }

  public int GetEndOfRun(int index)
  {
    int index1 = index >= 0 && index < this.Count ? index : throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) (this.Count - 1));
    int count = this.GetNode(ref index1).count;
    return index - index1 + count;
  }

  [Obsolete("This method may be confusing as it returns only the remaining run length after index. Use GetStartOfRun/GetEndOfRun instead.")]
  public int GetRunLength(int index)
  {
    if (index < 0 || index >= this.Count)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "Value must be between 0 and " + (object) (this.Count - 1));
    return this.GetNode(ref index).count - index;
  }

  public void Transform(Func<T, T> converter)
  {
    if (this.root == null)
      return;
    CompressingTreeList<T>.Node removedNode = (CompressingTreeList<T>.Node) null;
    for (CompressingTreeList<T>.Node node = this.root.LeftMost; node != null; node = node.Successor)
    {
      node.value = converter(node.value);
      if (removedNode != null && this.comparisonFunc(removedNode.value, node.value))
      {
        node.count += removedNode.count;
        this.UpdateAugmentedData(node);
        this.RemoveNode(removedNode);
      }
      removedNode = node;
    }
  }

  public void TransformRange(int index, int length, Func<T, T> converter)
  {
    if (this.root == null)
      return;
    int val1 = index + length;
    int num;
    for (int index1 = index; index1 < val1; index1 = num)
    {
      num = Math.Min(val1, this.GetEndOfRun(index1));
      T obj1 = this[index1];
      T obj2 = converter(obj1);
      this.SetRange(index1, num - index1, obj2);
    }
  }

  public void Insert(int index, T item) => this.InsertRange(index, 1, item);

  public void RemoveAt(int index) => this.RemoveRange(index, 1);

  public void Add(T item) => this.InsertRange(this.Count, 1, item);

  public void Clear() => this.root = (CompressingTreeList<T>.Node) null;

  public bool Contains(T item) => this.IndexOf(item) >= 0;

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (array.Length < this.Count)
      throw new ArgumentException("The array is too small", nameof (array));
    if (arrayIndex < 0 || arrayIndex + this.Count > array.Length)
      throw new ArgumentOutOfRangeException(nameof (arrayIndex), (object) arrayIndex, "Value must be between 0 and " + (object) (array.Length - this.Count));
    foreach (T obj in this)
      array[arrayIndex++] = obj;
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index < 0)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public IEnumerator<T> GetEnumerator()
  {
    if (this.root != null)
    {
      for (CompressingTreeList<T>.Node n = this.root.LeftMost; n != null; n = n.Successor)
      {
        for (int i = 0; i < n.count; ++i)
          yield return n.value;
      }
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  private void InsertAsLeft(
    CompressingTreeList<T>.Node parentNode,
    CompressingTreeList<T>.Node newNode)
  {
    parentNode.left = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void InsertAsRight(
    CompressingTreeList<T>.Node parentNode,
    CompressingTreeList<T>.Node newNode)
  {
    parentNode.right = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void FixTreeOnInsert(CompressingTreeList<T>.Node node)
  {
    CompressingTreeList<T>.Node parent1 = node.parent;
    if (parent1 == null)
    {
      node.color = false;
    }
    else
    {
      if (!parent1.color)
        return;
      CompressingTreeList<T>.Node parent2 = parent1.parent;
      CompressingTreeList<T>.Node node1 = CompressingTreeList<T>.Sibling(parent1);
      if (node1 != null && node1.color)
      {
        parent1.color = false;
        node1.color = false;
        parent2.color = true;
        this.FixTreeOnInsert(parent2);
      }
      else
      {
        if (node == parent1.right && parent1 == parent2.left)
        {
          this.RotateLeft(parent1);
          node = node.left;
        }
        else if (node == parent1.left && parent1 == parent2.right)
        {
          this.RotateRight(parent1);
          node = node.right;
        }
        CompressingTreeList<T>.Node parent3 = node.parent;
        CompressingTreeList<T>.Node parent4 = parent3.parent;
        parent3.color = false;
        parent4.color = true;
        if (node == parent3.left && parent3 == parent4.left)
          this.RotateRight(parent4);
        else
          this.RotateLeft(parent4);
      }
    }
  }

  private void RemoveNode(CompressingTreeList<T>.Node removedNode)
  {
    if (removedNode.left != null && removedNode.right != null)
    {
      CompressingTreeList<T>.Node leftMost = removedNode.right.LeftMost;
      this.RemoveNode(leftMost);
      this.ReplaceNode(removedNode, leftMost);
      leftMost.left = removedNode.left;
      if (leftMost.left != null)
        leftMost.left.parent = leftMost;
      leftMost.right = removedNode.right;
      if (leftMost.right != null)
        leftMost.right.parent = leftMost;
      leftMost.color = removedNode.color;
      this.UpdateAugmentedData(leftMost);
      if (leftMost.parent == null)
        return;
      this.UpdateAugmentedData(leftMost.parent);
    }
    else
    {
      CompressingTreeList<T>.Node parent = removedNode.parent;
      CompressingTreeList<T>.Node node = removedNode.left ?? removedNode.right;
      this.ReplaceNode(removedNode, node);
      if (parent != null)
        this.UpdateAugmentedData(parent);
      if (removedNode.color)
        return;
      if (node != null && node.color)
        node.color = false;
      else
        this.FixTreeOnDelete(node, parent);
    }
  }

  private void FixTreeOnDelete(
    CompressingTreeList<T>.Node node,
    CompressingTreeList<T>.Node parentNode)
  {
    if (parentNode == null)
      return;
    CompressingTreeList<T>.Node p = CompressingTreeList<T>.Sibling(node, parentNode);
    if (p.color)
    {
      parentNode.color = true;
      p.color = false;
      if (node == parentNode.left)
        this.RotateLeft(parentNode);
      else
        this.RotateRight(parentNode);
      p = CompressingTreeList<T>.Sibling(node, parentNode);
    }
    if (!parentNode.color && !p.color && !CompressingTreeList<T>.GetColor(p.left) && !CompressingTreeList<T>.GetColor(p.right))
    {
      p.color = true;
      this.FixTreeOnDelete(parentNode, parentNode.parent);
    }
    else if (parentNode.color && !p.color && !CompressingTreeList<T>.GetColor(p.left) && !CompressingTreeList<T>.GetColor(p.right))
    {
      p.color = true;
      parentNode.color = false;
    }
    else
    {
      if (node == parentNode.left && !p.color && CompressingTreeList<T>.GetColor(p.left) && !CompressingTreeList<T>.GetColor(p.right))
      {
        p.color = true;
        p.left.color = false;
        this.RotateRight(p);
      }
      else if (node == parentNode.right && !p.color && CompressingTreeList<T>.GetColor(p.right) && !CompressingTreeList<T>.GetColor(p.left))
      {
        p.color = true;
        p.right.color = false;
        this.RotateLeft(p);
      }
      CompressingTreeList<T>.Node node1 = CompressingTreeList<T>.Sibling(node, parentNode);
      node1.color = parentNode.color;
      parentNode.color = false;
      if (node == parentNode.left)
      {
        if (node1.right != null)
          node1.right.color = false;
        this.RotateLeft(parentNode);
      }
      else
      {
        if (node1.left != null)
          node1.left.color = false;
        this.RotateRight(parentNode);
      }
    }
  }

  private void ReplaceNode(
    CompressingTreeList<T>.Node replacedNode,
    CompressingTreeList<T>.Node newNode)
  {
    if (replacedNode.parent == null)
      this.root = newNode;
    else if (replacedNode.parent.left == replacedNode)
      replacedNode.parent.left = newNode;
    else
      replacedNode.parent.right = newNode;
    if (newNode != null)
      newNode.parent = replacedNode.parent;
    replacedNode.parent = (CompressingTreeList<T>.Node) null;
  }

  private void RotateLeft(CompressingTreeList<T>.Node p)
  {
    CompressingTreeList<T>.Node right = p.right;
    this.ReplaceNode(p, right);
    p.right = right.left;
    if (p.right != null)
      p.right.parent = p;
    right.left = p;
    p.parent = right;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(right);
  }

  private void RotateRight(CompressingTreeList<T>.Node p)
  {
    CompressingTreeList<T>.Node left = p.left;
    this.ReplaceNode(p, left);
    p.left = left.right;
    if (p.left != null)
      p.left.parent = p;
    left.right = p;
    p.parent = left;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(left);
  }

  private static CompressingTreeList<T>.Node Sibling(CompressingTreeList<T>.Node node)
  {
    return node == node.parent.left ? node.parent.right : node.parent.left;
  }

  private static CompressingTreeList<T>.Node Sibling(
    CompressingTreeList<T>.Node node,
    CompressingTreeList<T>.Node parentNode)
  {
    return node == parentNode.left ? parentNode.right : parentNode.left;
  }

  private static bool GetColor(CompressingTreeList<T>.Node node) => node != null && node.color;

  [Conditional("DATACONSISTENCYTEST")]
  internal void CheckProperties()
  {
  }

  internal string GetTreeAsString() => "Not available in release build.";

  private sealed class Node
  {
    internal CompressingTreeList<T>.Node left;
    internal CompressingTreeList<T>.Node right;
    internal CompressingTreeList<T>.Node parent;
    internal bool color;
    internal int count;
    internal int totalCount;
    internal T value;

    public Node(T value, int count)
    {
      this.value = value;
      this.count = count;
      this.totalCount = count;
    }

    internal CompressingTreeList<T>.Node LeftMost
    {
      get
      {
        CompressingTreeList<T>.Node leftMost = this;
        while (leftMost.left != null)
          leftMost = leftMost.left;
        return leftMost;
      }
    }

    internal CompressingTreeList<T>.Node RightMost
    {
      get
      {
        CompressingTreeList<T>.Node rightMost = this;
        while (rightMost.right != null)
          rightMost = rightMost.right;
        return rightMost;
      }
    }

    internal CompressingTreeList<T>.Node Predecessor
    {
      get
      {
        if (this.left != null)
          return this.left.RightMost;
        CompressingTreeList<T>.Node predecessor = this;
        CompressingTreeList<T>.Node node;
        do
        {
          node = predecessor;
          predecessor = predecessor.parent;
        }
        while (predecessor != null && predecessor.left == node);
        return predecessor;
      }
    }

    internal CompressingTreeList<T>.Node Successor
    {
      get
      {
        if (this.right != null)
          return this.right.LeftMost;
        CompressingTreeList<T>.Node successor = this;
        CompressingTreeList<T>.Node node;
        do
        {
          node = successor;
          successor = successor.parent;
        }
        while (successor != null && successor.right == node);
        return successor;
      }
    }

    public override string ToString()
    {
      return $"[TotalCount={(object) this.totalCount} Count={(object) this.count} Value={(object) this.value}]";
    }
  }
}
