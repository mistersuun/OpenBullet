// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextSegmentCollection`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextSegmentCollection<T> : 
  ICollection<T>,
  IEnumerable<T>,
  IEnumerable,
  ISegmentTree,
  IWeakEventListener
  where T : TextSegment
{
  internal const bool RED = true;
  internal const bool BLACK = false;
  private int count;
  private TextSegment root;
  private bool isConnectedToDocument;

  public TextSegmentCollection()
  {
  }

  public TextSegmentCollection(TextDocument textDocument)
  {
    if (textDocument == null)
      throw new ArgumentNullException(nameof (textDocument));
    textDocument.VerifyAccess();
    this.isConnectedToDocument = true;
    WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(textDocument, (IWeakEventListener) this);
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (TextDocumentWeakEventManager.Changed)))
      return false;
    this.OnDocumentChanged((DocumentChangeEventArgs) e);
    return true;
  }

  public void UpdateOffsets(DocumentChangeEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    if (this.isConnectedToDocument)
      throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
    this.OnDocumentChanged(e);
  }

  private void OnDocumentChanged(DocumentChangeEventArgs e)
  {
    OffsetChangeMap offsetChangeMapOrNull = e.OffsetChangeMapOrNull;
    if (offsetChangeMapOrNull != null)
    {
      foreach (OffsetChangeMapEntry change in (Collection<OffsetChangeMapEntry>) offsetChangeMapOrNull)
        this.UpdateOffsetsInternal(change);
    }
    else
      this.UpdateOffsetsInternal(e.CreateSingleChangeMapEntry());
  }

  public void UpdateOffsets(OffsetChangeMapEntry change)
  {
    if (this.isConnectedToDocument)
      throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
    this.UpdateOffsetsInternal(change);
  }

  private void UpdateOffsetsInternal(OffsetChangeMapEntry change)
  {
    if (change.RemovalLength == 0)
      this.InsertText(change.Offset, change.InsertionLength);
    else
      this.ReplaceText(change);
  }

  private void InsertText(int offset, int length)
  {
    if (length == 0)
      return;
    foreach (T obj in this.FindSegmentsContaining(offset))
    {
      TextSegment textSegment = (TextSegment) obj;
      if (textSegment.StartOffset < offset && offset < textSegment.EndOffset)
        textSegment.Length += length;
    }
    TextSegment segmentWithStartAfter = (TextSegment) this.FindFirstSegmentWithStartAfter(offset);
    if (segmentWithStartAfter == null)
      return;
    segmentWithStartAfter.nodeLength += length;
    this.UpdateAugmentedData(segmentWithStartAfter);
  }

  private void ReplaceText(OffsetChangeMapEntry change)
  {
    int offset = change.Offset;
    foreach (T overlappingSegment in this.FindOverlappingSegments(offset, change.RemovalLength))
    {
      TextSegment textSegment = (TextSegment) overlappingSegment;
      if (textSegment.StartOffset <= offset)
      {
        if (textSegment.EndOffset >= offset + change.RemovalLength)
          textSegment.Length += change.InsertionLength - change.RemovalLength;
        else
          textSegment.Length = offset - textSegment.StartOffset;
      }
      else
      {
        int val2 = textSegment.EndOffset - (offset + change.RemovalLength);
        this.RemoveSegment(textSegment);
        textSegment.StartOffset = offset + change.RemovalLength;
        textSegment.Length = Math.Max(0, val2);
        this.AddSegment(textSegment);
      }
    }
    TextSegment segmentWithStartAfter = (TextSegment) this.FindFirstSegmentWithStartAfter(offset + 1);
    if (segmentWithStartAfter == null)
      return;
    segmentWithStartAfter.nodeLength += change.InsertionLength - change.RemovalLength;
    this.UpdateAugmentedData(segmentWithStartAfter);
  }

  public void Add(T item)
  {
    if ((object) item == null)
      throw new ArgumentNullException(nameof (item));
    if (item.ownerTree != null)
      throw new ArgumentException("The segment is already added to a SegmentCollection.");
    this.AddSegment((TextSegment) item);
  }

  void ISegmentTree.Add(TextSegment s) => this.AddSegment(s);

  private void AddSegment(TextSegment node)
  {
    int startOffset = node.StartOffset;
    node.distanceToMaxEnd = node.segmentLength;
    if (this.root == null)
    {
      this.root = node;
      node.totalNodeLength = node.nodeLength;
    }
    else if (startOffset >= this.root.totalNodeLength)
    {
      node.nodeLength = node.totalNodeLength = startOffset - this.root.totalNodeLength;
      this.InsertAsRight(this.root.RightMost, node);
    }
    else
    {
      TextSegment node1 = this.FindNode(ref startOffset);
      node.totalNodeLength = node.nodeLength = startOffset;
      node1.nodeLength -= startOffset;
      this.InsertBefore(node1, node);
    }
    node.ownerTree = (ISegmentTree) this;
    ++this.count;
  }

  private void InsertBefore(TextSegment node, TextSegment newNode)
  {
    if (node.left == null)
      this.InsertAsLeft(node, newNode);
    else
      this.InsertAsRight(node.left.RightMost, newNode);
  }

  public T GetNextSegment(T segment)
  {
    return this.Contains(segment) ? (T) segment.Successor : throw new ArgumentException("segment is not inside the segment tree");
  }

  public T GetPreviousSegment(T segment)
  {
    return this.Contains(segment) ? (T) segment.Predecessor : throw new ArgumentException("segment is not inside the segment tree");
  }

  public T FirstSegment => this.root != null ? (T) this.root.LeftMost : default (T);

  public T LastSegment => this.root != null ? (T) this.root.RightMost : default (T);

  public T FindFirstSegmentWithStartAfter(int startOffset)
  {
    if (this.root == null)
      return default (T);
    if (startOffset <= 0)
      return (T) this.root.LeftMost;
    TextSegment segmentWithStartAfter = this.FindNode(ref startOffset);
    while (startOffset == 0)
    {
      TextSegment textSegment = segmentWithStartAfter == null ? this.root.RightMost : segmentWithStartAfter.Predecessor;
      startOffset += textSegment.nodeLength;
      segmentWithStartAfter = textSegment;
    }
    return (T) segmentWithStartAfter;
  }

  private TextSegment FindNode(ref int offset)
  {
    TextSegment node = this.root;
    while (true)
    {
      for (; node.left != null; node = node.left)
      {
        if (offset >= node.left.totalNodeLength)
        {
          offset -= node.left.totalNodeLength;
          break;
        }
      }
      if (offset >= node.nodeLength)
      {
        offset -= node.nodeLength;
        if (node.right != null)
          node = node.right;
        else
          goto label_9;
      }
      else
        break;
    }
    return node;
label_9:
    return (TextSegment) null;
  }

  public ReadOnlyCollection<T> FindSegmentsContaining(int offset)
  {
    return this.FindOverlappingSegments(offset, 0);
  }

  public ReadOnlyCollection<T> FindOverlappingSegments(ISegment segment)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    return this.FindOverlappingSegments(segment.Offset, segment.Length);
  }

  public ReadOnlyCollection<T> FindOverlappingSegments(int offset, int length)
  {
    ThrowUtil.CheckNotNegative(length, nameof (length));
    List<T> results = new List<T>();
    if (this.root != null)
      this.FindOverlappingSegments(results, this.root, offset, offset + length);
    return results.AsReadOnly();
  }

  private void FindOverlappingSegments(List<T> results, TextSegment node, int low, int high)
  {
    if (high < 0)
      return;
    int low1 = low - node.nodeLength;
    int high1 = high - node.nodeLength;
    if (node.left != null)
    {
      low1 -= node.left.totalNodeLength;
      high1 -= node.left.totalNodeLength;
    }
    if (node.distanceToMaxEnd < low1)
      return;
    if (node.left != null)
      this.FindOverlappingSegments(results, node.left, low, high);
    if (high1 < 0)
      return;
    if (low1 <= node.segmentLength)
      results.Add((T) node);
    if (node.right == null)
      return;
    this.FindOverlappingSegments(results, node.right, low1, high1);
  }

  private void UpdateAugmentedData(TextSegment node)
  {
    int nodeLength = node.nodeLength;
    int num1 = node.segmentLength;
    if (node.left != null)
    {
      nodeLength += node.left.totalNodeLength;
      int distanceToMaxEnd = node.left.distanceToMaxEnd;
      if (node.left.right != null)
        distanceToMaxEnd -= node.left.right.totalNodeLength;
      int num2 = distanceToMaxEnd - node.nodeLength;
      if (num2 > num1)
        num1 = num2;
    }
    if (node.right != null)
    {
      nodeLength += node.right.totalNodeLength;
      int num3 = node.right.distanceToMaxEnd + node.right.nodeLength;
      if (node.right.left != null)
        num3 += node.right.left.totalNodeLength;
      if (num3 > num1)
        num1 = num3;
    }
    if (node.totalNodeLength == nodeLength && node.distanceToMaxEnd == num1)
      return;
    node.totalNodeLength = nodeLength;
    node.distanceToMaxEnd = num1;
    if (node.parent == null)
      return;
    this.UpdateAugmentedData(node.parent);
  }

  void ISegmentTree.UpdateAugmentedData(TextSegment node) => this.UpdateAugmentedData(node);

  public bool Remove(T item)
  {
    if (!this.Contains(item))
      return false;
    this.RemoveSegment((TextSegment) item);
    return true;
  }

  void ISegmentTree.Remove(TextSegment s) => this.RemoveSegment(s);

  private void RemoveSegment(TextSegment s)
  {
    int startOffset = s.StartOffset;
    TextSegment successor = s.Successor;
    if (successor != null)
      successor.nodeLength += s.nodeLength;
    this.RemoveNode(s);
    if (successor != null)
      this.UpdateAugmentedData(successor);
    this.Disconnect(s, startOffset);
  }

  private void Disconnect(TextSegment s, int offset)
  {
    s.left = s.right = s.parent = (TextSegment) null;
    s.ownerTree = (ISegmentTree) null;
    s.nodeLength = offset;
    --this.count;
  }

  public void Clear()
  {
    T[] array = this.ToArray<T>();
    this.root = (TextSegment) null;
    int offset = 0;
    foreach (T obj in array)
    {
      TextSegment s = (TextSegment) obj;
      offset += s.nodeLength;
      this.Disconnect(s, offset);
    }
  }

  [Conditional("DATACONSISTENCYTEST")]
  internal void CheckProperties()
  {
  }

  internal string GetTreeAsString() => "Not available in release build.";

  private void InsertAsLeft(TextSegment parentNode, TextSegment newNode)
  {
    parentNode.left = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void InsertAsRight(TextSegment parentNode, TextSegment newNode)
  {
    parentNode.right = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void FixTreeOnInsert(TextSegment node)
  {
    TextSegment parent1 = node.parent;
    if (parent1 == null)
    {
      node.color = false;
    }
    else
    {
      if (!parent1.color)
        return;
      TextSegment parent2 = parent1.parent;
      TextSegment textSegment = TextSegmentCollection<T>.Sibling(parent1);
      if (textSegment != null && textSegment.color)
      {
        parent1.color = false;
        textSegment.color = false;
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
        TextSegment parent3 = node.parent;
        TextSegment parent4 = parent3.parent;
        parent3.color = false;
        parent4.color = true;
        if (node == parent3.left && parent3 == parent4.left)
          this.RotateRight(parent4);
        else
          this.RotateLeft(parent4);
      }
    }
  }

  private void RemoveNode(TextSegment removedNode)
  {
    if (removedNode.left != null && removedNode.right != null)
    {
      TextSegment leftMost = removedNode.right.LeftMost;
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
      TextSegment parent = removedNode.parent;
      TextSegment textSegment = removedNode.left ?? removedNode.right;
      this.ReplaceNode(removedNode, textSegment);
      if (parent != null)
        this.UpdateAugmentedData(parent);
      if (removedNode.color)
        return;
      if (textSegment != null && textSegment.color)
        textSegment.color = false;
      else
        this.FixTreeOnDelete(textSegment, parent);
    }
  }

  private void FixTreeOnDelete(TextSegment node, TextSegment parentNode)
  {
    if (parentNode == null)
      return;
    TextSegment p = TextSegmentCollection<T>.Sibling(node, parentNode);
    if (p.color)
    {
      parentNode.color = true;
      p.color = false;
      if (node == parentNode.left)
        this.RotateLeft(parentNode);
      else
        this.RotateRight(parentNode);
      p = TextSegmentCollection<T>.Sibling(node, parentNode);
    }
    if (!parentNode.color && !p.color && !TextSegmentCollection<T>.GetColor(p.left) && !TextSegmentCollection<T>.GetColor(p.right))
    {
      p.color = true;
      this.FixTreeOnDelete(parentNode, parentNode.parent);
    }
    else if (parentNode.color && !p.color && !TextSegmentCollection<T>.GetColor(p.left) && !TextSegmentCollection<T>.GetColor(p.right))
    {
      p.color = true;
      parentNode.color = false;
    }
    else
    {
      if (node == parentNode.left && !p.color && TextSegmentCollection<T>.GetColor(p.left) && !TextSegmentCollection<T>.GetColor(p.right))
      {
        p.color = true;
        p.left.color = false;
        this.RotateRight(p);
      }
      else if (node == parentNode.right && !p.color && TextSegmentCollection<T>.GetColor(p.right) && !TextSegmentCollection<T>.GetColor(p.left))
      {
        p.color = true;
        p.right.color = false;
        this.RotateLeft(p);
      }
      TextSegment textSegment = TextSegmentCollection<T>.Sibling(node, parentNode);
      textSegment.color = parentNode.color;
      parentNode.color = false;
      if (node == parentNode.left)
      {
        if (textSegment.right != null)
          textSegment.right.color = false;
        this.RotateLeft(parentNode);
      }
      else
      {
        if (textSegment.left != null)
          textSegment.left.color = false;
        this.RotateRight(parentNode);
      }
    }
  }

  private void ReplaceNode(TextSegment replacedNode, TextSegment newNode)
  {
    if (replacedNode.parent == null)
      this.root = newNode;
    else if (replacedNode.parent.left == replacedNode)
      replacedNode.parent.left = newNode;
    else
      replacedNode.parent.right = newNode;
    if (newNode != null)
      newNode.parent = replacedNode.parent;
    replacedNode.parent = (TextSegment) null;
  }

  private void RotateLeft(TextSegment p)
  {
    TextSegment right = p.right;
    this.ReplaceNode(p, right);
    p.right = right.left;
    if (p.right != null)
      p.right.parent = p;
    right.left = p;
    p.parent = right;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(right);
  }

  private void RotateRight(TextSegment p)
  {
    TextSegment left = p.left;
    this.ReplaceNode(p, left);
    p.left = left.right;
    if (p.left != null)
      p.left.parent = p;
    left.right = p;
    p.parent = left;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(left);
  }

  private static TextSegment Sibling(TextSegment node)
  {
    return node == node.parent.left ? node.parent.right : node.parent.left;
  }

  private static TextSegment Sibling(TextSegment node, TextSegment parentNode)
  {
    return node == parentNode.left ? parentNode.right : parentNode.left;
  }

  private static bool GetColor(TextSegment node) => node != null && node.color;

  public int Count => this.count;

  bool ICollection<T>.IsReadOnly => false;

  public bool Contains(T item) => (object) item != null && item.ownerTree == this;

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (array.Length < this.Count)
      throw new ArgumentException("The array is too small", nameof (array));
    if (arrayIndex < 0 || arrayIndex + this.count > array.Length)
      throw new ArgumentOutOfRangeException(nameof (arrayIndex), (object) arrayIndex, "Value must be between 0 and " + (object) (array.Length - this.count));
    foreach (T obj in this)
      array[arrayIndex++] = obj;
  }

  public IEnumerator<T> GetEnumerator()
  {
    if (this.root != null)
    {
      for (TextSegment current = this.root.LeftMost; current != null; current = current.Successor)
        yield return (T) current;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
