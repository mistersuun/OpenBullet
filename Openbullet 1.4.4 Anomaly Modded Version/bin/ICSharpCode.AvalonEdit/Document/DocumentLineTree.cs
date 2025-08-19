// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.DocumentLineTree
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class DocumentLineTree : 
  IList<DocumentLine>,
  ICollection<DocumentLine>,
  IEnumerable<DocumentLine>,
  IEnumerable
{
  internal const bool RED = true;
  internal const bool BLACK = false;
  private readonly TextDocument document;
  private DocumentLine root;

  public DocumentLineTree(TextDocument document)
  {
    this.document = document;
    this.root = new DocumentLine(document).InitLineNode();
  }

  internal static void UpdateAfterChildrenChange(DocumentLine node)
  {
    int num = 1;
    int totalLength = node.TotalLength;
    if (node.left != null)
    {
      num += node.left.nodeTotalCount;
      totalLength += node.left.nodeTotalLength;
    }
    if (node.right != null)
    {
      num += node.right.nodeTotalCount;
      totalLength += node.right.nodeTotalLength;
    }
    if (num == node.nodeTotalCount && totalLength == node.nodeTotalLength)
      return;
    node.nodeTotalCount = num;
    node.nodeTotalLength = totalLength;
    if (node.parent == null)
      return;
    DocumentLineTree.UpdateAfterChildrenChange(node.parent);
  }

  private static void UpdateAfterRotateLeft(DocumentLine node)
  {
    DocumentLineTree.UpdateAfterChildrenChange(node);
  }

  private static void UpdateAfterRotateRight(DocumentLine node)
  {
    DocumentLineTree.UpdateAfterChildrenChange(node);
  }

  public void RebuildTree(List<DocumentLine> documentLines)
  {
    DocumentLine[] nodes = new DocumentLine[documentLines.Count];
    for (int index = 0; index < documentLines.Count; ++index)
    {
      DocumentLine documentLine = documentLines[index].InitLineNode();
      nodes[index] = documentLine;
    }
    int treeHeight = DocumentLineTree.GetTreeHeight(nodes.Length);
    this.root = this.BuildTree(nodes, 0, nodes.Length, treeHeight);
    this.root.color = false;
  }

  internal static int GetTreeHeight(int size)
  {
    return size == 0 ? 0 : DocumentLineTree.GetTreeHeight(size / 2) + 1;
  }

  private DocumentLine BuildTree(DocumentLine[] nodes, int start, int end, int subtreeHeight)
  {
    if (start == end)
      return (DocumentLine) null;
    int end1 = (start + end) / 2;
    DocumentLine node = nodes[end1];
    node.left = this.BuildTree(nodes, start, end1, subtreeHeight - 1);
    node.right = this.BuildTree(nodes, end1 + 1, end, subtreeHeight - 1);
    if (node.left != null)
      node.left.parent = node;
    if (node.right != null)
      node.right.parent = node;
    if (subtreeHeight == 1)
      node.color = true;
    DocumentLineTree.UpdateAfterChildrenChange(node);
    return node;
  }

  private DocumentLine GetNodeByIndex(int index)
  {
    DocumentLine nodeByIndex = this.root;
    while (true)
    {
      for (; nodeByIndex.left == null || index >= nodeByIndex.left.nodeTotalCount; nodeByIndex = nodeByIndex.right)
      {
        if (nodeByIndex.left != null)
          index -= nodeByIndex.left.nodeTotalCount;
        if (index == 0)
          return nodeByIndex;
        --index;
      }
      nodeByIndex = nodeByIndex.left;
    }
  }

  internal static int GetIndexFromNode(DocumentLine node)
  {
    int nodeTotalCount = node.left != null ? node.left.nodeTotalCount : 0;
    for (; node.parent != null; node = node.parent)
    {
      if (node == node.parent.right)
      {
        if (node.parent.left != null)
          nodeTotalCount += node.parent.left.nodeTotalCount;
        ++nodeTotalCount;
      }
    }
    return nodeTotalCount;
  }

  private DocumentLine GetNodeByOffset(int offset)
  {
    if (offset == this.root.nodeTotalLength)
      return this.root.RightMost;
    DocumentLine nodeByOffset = this.root;
    while (true)
    {
      for (; nodeByOffset.left == null || offset >= nodeByOffset.left.nodeTotalLength; nodeByOffset = nodeByOffset.right)
      {
        if (nodeByOffset.left != null)
          offset -= nodeByOffset.left.nodeTotalLength;
        offset -= nodeByOffset.TotalLength;
        if (offset < 0)
          return nodeByOffset;
      }
      nodeByOffset = nodeByOffset.left;
    }
  }

  internal static int GetOffsetFromNode(DocumentLine node)
  {
    int nodeTotalLength = node.left != null ? node.left.nodeTotalLength : 0;
    for (; node.parent != null; node = node.parent)
    {
      if (node == node.parent.right)
      {
        if (node.parent.left != null)
          nodeTotalLength += node.parent.left.nodeTotalLength;
        nodeTotalLength += node.parent.TotalLength;
      }
    }
    return nodeTotalLength;
  }

  public DocumentLine GetByNumber(int number) => this.GetNodeByIndex(number - 1);

  public DocumentLine GetByOffset(int offset) => this.GetNodeByOffset(offset);

  public int LineCount => this.root.nodeTotalCount;

  public void RemoveLine(DocumentLine line)
  {
    this.RemoveNode(line);
    line.isDeleted = true;
  }

  public DocumentLine InsertLineAfter(DocumentLine line, int totalLength)
  {
    DocumentLine newLine = new DocumentLine(this.document);
    newLine.TotalLength = totalLength;
    this.InsertAfter(line, newLine);
    return newLine;
  }

  private void InsertAfter(DocumentLine node, DocumentLine newLine)
  {
    DocumentLine newNode = newLine.InitLineNode();
    if (node.right == null)
      this.InsertAsRight(node, newNode);
    else
      this.InsertAsLeft(node.right.LeftMost, newNode);
  }

  private void InsertAsLeft(DocumentLine parentNode, DocumentLine newNode)
  {
    parentNode.left = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    DocumentLineTree.UpdateAfterChildrenChange(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void InsertAsRight(DocumentLine parentNode, DocumentLine newNode)
  {
    parentNode.right = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    DocumentLineTree.UpdateAfterChildrenChange(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void FixTreeOnInsert(DocumentLine node)
  {
    DocumentLine parent1 = node.parent;
    if (parent1 == null)
    {
      node.color = false;
    }
    else
    {
      if (!parent1.color)
        return;
      DocumentLine parent2 = parent1.parent;
      DocumentLine documentLine = DocumentLineTree.Sibling(parent1);
      if (documentLine != null && documentLine.color)
      {
        parent1.color = false;
        documentLine.color = false;
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
        DocumentLine parent3 = node.parent;
        DocumentLine parent4 = parent3.parent;
        parent3.color = false;
        parent4.color = true;
        if (node == parent3.left && parent3 == parent4.left)
          this.RotateRight(parent4);
        else
          this.RotateLeft(parent4);
      }
    }
  }

  private void RemoveNode(DocumentLine removedNode)
  {
    if (removedNode.left != null && removedNode.right != null)
    {
      DocumentLine leftMost = removedNode.right.LeftMost;
      this.RemoveNode(leftMost);
      this.ReplaceNode(removedNode, leftMost);
      leftMost.left = removedNode.left;
      if (leftMost.left != null)
        leftMost.left.parent = leftMost;
      leftMost.right = removedNode.right;
      if (leftMost.right != null)
        leftMost.right.parent = leftMost;
      leftMost.color = removedNode.color;
      DocumentLineTree.UpdateAfterChildrenChange(leftMost);
      if (leftMost.parent == null)
        return;
      DocumentLineTree.UpdateAfterChildrenChange(leftMost.parent);
    }
    else
    {
      DocumentLine parent = removedNode.parent;
      DocumentLine documentLine = removedNode.left ?? removedNode.right;
      this.ReplaceNode(removedNode, documentLine);
      if (parent != null)
        DocumentLineTree.UpdateAfterChildrenChange(parent);
      if (removedNode.color)
        return;
      if (documentLine != null && documentLine.color)
        documentLine.color = false;
      else
        this.FixTreeOnDelete(documentLine, parent);
    }
  }

  private void FixTreeOnDelete(DocumentLine node, DocumentLine parentNode)
  {
    if (parentNode == null)
      return;
    DocumentLine p = DocumentLineTree.Sibling(node, parentNode);
    if (p.color)
    {
      parentNode.color = true;
      p.color = false;
      if (node == parentNode.left)
        this.RotateLeft(parentNode);
      else
        this.RotateRight(parentNode);
      p = DocumentLineTree.Sibling(node, parentNode);
    }
    if (!parentNode.color && !p.color && !DocumentLineTree.GetColor(p.left) && !DocumentLineTree.GetColor(p.right))
    {
      p.color = true;
      this.FixTreeOnDelete(parentNode, parentNode.parent);
    }
    else if (parentNode.color && !p.color && !DocumentLineTree.GetColor(p.left) && !DocumentLineTree.GetColor(p.right))
    {
      p.color = true;
      parentNode.color = false;
    }
    else
    {
      if (node == parentNode.left && !p.color && DocumentLineTree.GetColor(p.left) && !DocumentLineTree.GetColor(p.right))
      {
        p.color = true;
        p.left.color = false;
        this.RotateRight(p);
      }
      else if (node == parentNode.right && !p.color && DocumentLineTree.GetColor(p.right) && !DocumentLineTree.GetColor(p.left))
      {
        p.color = true;
        p.right.color = false;
        this.RotateLeft(p);
      }
      DocumentLine documentLine = DocumentLineTree.Sibling(node, parentNode);
      documentLine.color = parentNode.color;
      parentNode.color = false;
      if (node == parentNode.left)
      {
        if (documentLine.right != null)
          documentLine.right.color = false;
        this.RotateLeft(parentNode);
      }
      else
      {
        if (documentLine.left != null)
          documentLine.left.color = false;
        this.RotateRight(parentNode);
      }
    }
  }

  private void ReplaceNode(DocumentLine replacedNode, DocumentLine newNode)
  {
    if (replacedNode.parent == null)
      this.root = newNode;
    else if (replacedNode.parent.left == replacedNode)
      replacedNode.parent.left = newNode;
    else
      replacedNode.parent.right = newNode;
    if (newNode != null)
      newNode.parent = replacedNode.parent;
    replacedNode.parent = (DocumentLine) null;
  }

  private void RotateLeft(DocumentLine p)
  {
    DocumentLine right = p.right;
    this.ReplaceNode(p, right);
    p.right = right.left;
    if (p.right != null)
      p.right.parent = p;
    right.left = p;
    p.parent = right;
    DocumentLineTree.UpdateAfterRotateLeft(p);
  }

  private void RotateRight(DocumentLine p)
  {
    DocumentLine left = p.left;
    this.ReplaceNode(p, left);
    p.left = left.right;
    if (p.left != null)
      p.left.parent = p;
    left.right = p;
    p.parent = left;
    DocumentLineTree.UpdateAfterRotateRight(p);
  }

  private static DocumentLine Sibling(DocumentLine node)
  {
    return node == node.parent.left ? node.parent.right : node.parent.left;
  }

  private static DocumentLine Sibling(DocumentLine node, DocumentLine parentNode)
  {
    return node == parentNode.left ? parentNode.right : parentNode.left;
  }

  private static bool GetColor(DocumentLine node) => node != null && node.color;

  DocumentLine IList<DocumentLine>.this[int index]
  {
    get
    {
      this.document.VerifyAccess();
      return this.GetByNumber(1 + index);
    }
    set => throw new NotSupportedException();
  }

  int ICollection<DocumentLine>.Count
  {
    get
    {
      this.document.VerifyAccess();
      return this.LineCount;
    }
  }

  bool ICollection<DocumentLine>.IsReadOnly => true;

  int IList<DocumentLine>.IndexOf(DocumentLine item)
  {
    this.document.VerifyAccess();
    if (item == null || item.IsDeleted)
      return -1;
    int index = item.LineNumber - 1;
    return index < this.LineCount && this.GetNodeByIndex(index) == item ? index : -1;
  }

  void IList<DocumentLine>.Insert(int index, DocumentLine item)
  {
    throw new NotSupportedException();
  }

  void IList<DocumentLine>.RemoveAt(int index) => throw new NotSupportedException();

  void ICollection<DocumentLine>.Add(DocumentLine item) => throw new NotSupportedException();

  void ICollection<DocumentLine>.Clear() => throw new NotSupportedException();

  bool ICollection<DocumentLine>.Contains(DocumentLine item)
  {
    return ((IList<DocumentLine>) this).IndexOf(item) >= 0;
  }

  void ICollection<DocumentLine>.CopyTo(DocumentLine[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (array.Length < this.LineCount)
      throw new ArgumentException("The array is too small", nameof (array));
    if (arrayIndex < 0 || arrayIndex + this.LineCount > array.Length)
      throw new ArgumentOutOfRangeException(nameof (arrayIndex), (object) arrayIndex, "Value must be between 0 and " + (object) (array.Length - this.LineCount));
    foreach (DocumentLine documentLine in this)
      array[arrayIndex++] = documentLine;
  }

  bool ICollection<DocumentLine>.Remove(DocumentLine item) => throw new NotSupportedException();

  public IEnumerator<DocumentLine> GetEnumerator()
  {
    this.document.VerifyAccess();
    return this.Enumerate();
  }

  private IEnumerator<DocumentLine> Enumerate()
  {
    this.document.VerifyAccess();
    for (DocumentLine line = this.root.LeftMost; line != null; line = line.NextLine)
      yield return line;
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
