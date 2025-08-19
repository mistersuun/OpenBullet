// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextAnchorTree
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class TextAnchorTree
{
  internal const bool RED = true;
  internal const bool BLACK = false;
  private readonly TextDocument document;
  private readonly List<TextAnchorNode> nodesToDelete = new List<TextAnchorNode>();
  private TextAnchorNode root;

  public TextAnchorTree(TextDocument document) => this.document = document;

  [Conditional("DEBUG")]
  private static void Log(string text)
  {
  }

  private void InsertText(int offset, int length, bool defaultAnchorMovementIsBeforeInsertion)
  {
    if (length == 0 || this.root == null || offset > this.root.totalLength)
      return;
    if (offset == this.root.totalLength)
    {
      this.PerformInsertText(this.FindActualBeginNode(this.root.RightMost), (TextAnchorNode) null, length, defaultAnchorMovementIsBeforeInsertion);
    }
    else
    {
      TextAnchorNode node = this.FindNode(ref offset);
      if (offset > 0)
      {
        node.length += length;
        this.UpdateAugmentedData(node);
      }
      else
        this.PerformInsertText(this.FindActualBeginNode(node.Predecessor), node, length, defaultAnchorMovementIsBeforeInsertion);
    }
    this.DeleteMarkedNodes();
  }

  private TextAnchorNode FindActualBeginNode(TextAnchorNode node)
  {
    while (node != null && node.length == 0)
      node = node.Predecessor;
    if (node == null)
      node = this.root.LeftMost;
    return node;
  }

  private void PerformInsertText(
    TextAnchorNode beginNode,
    TextAnchorNode endNode,
    int length,
    bool defaultAnchorMovementIsBeforeInsertion)
  {
    List<TextAnchorNode> textAnchorNodeList = new List<TextAnchorNode>();
    for (TextAnchorNode node = beginNode; node != endNode; node = node.Successor)
    {
      TextAnchor target = (TextAnchor) node.Target;
      if (target == null)
        this.MarkNodeForDelete(node);
      else if ((defaultAnchorMovementIsBeforeInsertion ? (target.MovementType != AnchorMovementType.AfterInsertion ? 1 : 0) : (target.MovementType == AnchorMovementType.BeforeInsertion ? 1 : 0)) != 0)
        textAnchorNodeList.Add(node);
    }
    TextAnchorNode textAnchorNode = beginNode;
    foreach (TextAnchorNode n1 in textAnchorNodeList)
    {
      this.SwapAnchors(n1, textAnchorNode);
      textAnchorNode = textAnchorNode.Successor;
    }
    if (textAnchorNode == null)
      return;
    textAnchorNode.length += length;
    this.UpdateAugmentedData(textAnchorNode);
  }

  private void SwapAnchors(TextAnchorNode n1, TextAnchorNode n2)
  {
    if (n1 == n2)
      return;
    TextAnchor target1 = (TextAnchor) n1.Target;
    TextAnchor target2 = (TextAnchor) n2.Target;
    if (target1 == null && target2 == null)
      return;
    n1.Target = (object) target2;
    n2.Target = (object) target1;
    if (target1 == null)
    {
      this.nodesToDelete.Remove(n1);
      this.MarkNodeForDelete(n2);
      target2.node = n1;
    }
    else if (target2 == null)
    {
      this.nodesToDelete.Remove(n2);
      this.MarkNodeForDelete(n1);
      target1.node = n2;
    }
    else
    {
      target1.node = n2;
      target2.node = n1;
    }
  }

  public void HandleTextChange(OffsetChangeMapEntry entry, DelayedEvents delayedEvents)
  {
    if (entry.RemovalLength == 0)
    {
      this.InsertText(entry.Offset, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
    }
    else
    {
      int offset = entry.Offset;
      int removalLength = entry.RemovalLength;
      if (this.root == null || offset >= this.root.totalLength)
        return;
      TextAnchorNode textAnchorNode = this.FindNode(ref offset);
      TextAnchorNode beginNode = (TextAnchorNode) null;
      while (textAnchorNode != null && offset + removalLength > textAnchorNode.length)
      {
        TextAnchor target = (TextAnchor) textAnchorNode.Target;
        if (target != null && (target.SurviveDeletion || entry.RemovalNeverCausesAnchorDeletion))
        {
          if (beginNode == null)
            beginNode = textAnchorNode;
          removalLength -= textAnchorNode.length - offset;
          textAnchorNode.length = offset;
          offset = 0;
          this.UpdateAugmentedData(textAnchorNode);
          textAnchorNode = textAnchorNode.Successor;
        }
        else
        {
          TextAnchorNode successor = textAnchorNode.Successor;
          removalLength -= textAnchorNode.length;
          this.RemoveNode(textAnchorNode);
          this.nodesToDelete.Remove(textAnchorNode);
          target?.OnDeleted(delayedEvents);
          textAnchorNode = successor;
        }
      }
      if (textAnchorNode != null)
        textAnchorNode.length -= removalLength;
      if (entry.InsertionLength > 0)
      {
        if (beginNode != null)
          this.PerformInsertText(beginNode, textAnchorNode, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
        else if (textAnchorNode != null)
          textAnchorNode.length += entry.InsertionLength;
      }
      if (textAnchorNode != null)
        this.UpdateAugmentedData(textAnchorNode);
      this.DeleteMarkedNodes();
    }
  }

  private void MarkNodeForDelete(TextAnchorNode node)
  {
    if (this.nodesToDelete.Contains(node))
      return;
    this.nodesToDelete.Add(node);
  }

  private void DeleteMarkedNodes()
  {
    while (this.nodesToDelete.Count > 0)
    {
      int index = this.nodesToDelete.Count - 1;
      TextAnchorNode removedNode = this.nodesToDelete[index];
      TextAnchorNode successor = removedNode.Successor;
      if (successor != null)
        successor.length += removedNode.length;
      this.RemoveNode(removedNode);
      if (successor != null)
        this.UpdateAugmentedData(successor);
      this.nodesToDelete.RemoveAt(index);
    }
  }

  private TextAnchorNode FindNode(ref int offset)
  {
    TextAnchorNode node = this.root;
    while (true)
    {
      for (; node.left != null; node = node.left)
      {
        if (offset >= node.left.totalLength)
        {
          offset -= node.left.totalLength;
          break;
        }
      }
      if (!node.IsAlive)
        this.MarkNodeForDelete(node);
      if (offset >= node.length)
      {
        offset -= node.length;
        if (node.right != null)
          node = node.right;
        else
          goto label_11;
      }
      else
        break;
    }
    return node;
label_11:
    return (TextAnchorNode) null;
  }

  private void UpdateAugmentedData(TextAnchorNode n)
  {
    if (!n.IsAlive)
      this.MarkNodeForDelete(n);
    int length = n.length;
    if (n.left != null)
      length += n.left.totalLength;
    if (n.right != null)
      length += n.right.totalLength;
    if (n.totalLength == length)
      return;
    n.totalLength = length;
    if (n.parent == null)
      return;
    this.UpdateAugmentedData(n.parent);
  }

  public TextAnchor CreateAnchor(int offset)
  {
    TextAnchor anchor = new TextAnchor(this.document);
    anchor.node = new TextAnchorNode(anchor);
    if (this.root == null)
    {
      this.root = anchor.node;
      this.root.totalLength = this.root.length = offset;
    }
    else if (offset >= this.root.totalLength)
    {
      anchor.node.totalLength = anchor.node.length = offset - this.root.totalLength;
      this.InsertAsRight(this.root.RightMost, anchor.node);
    }
    else
    {
      TextAnchorNode node = this.FindNode(ref offset);
      anchor.node.totalLength = anchor.node.length = offset;
      node.length -= offset;
      this.InsertBefore(node, anchor.node);
    }
    this.DeleteMarkedNodes();
    return anchor;
  }

  private void InsertBefore(TextAnchorNode node, TextAnchorNode newNode)
  {
    if (node.left == null)
      this.InsertAsLeft(node, newNode);
    else
      this.InsertAsRight(node.left.RightMost, newNode);
  }

  private void InsertAsLeft(TextAnchorNode parentNode, TextAnchorNode newNode)
  {
    parentNode.left = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void InsertAsRight(TextAnchorNode parentNode, TextAnchorNode newNode)
  {
    parentNode.right = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    this.UpdateAugmentedData(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void FixTreeOnInsert(TextAnchorNode node)
  {
    TextAnchorNode parent1 = node.parent;
    if (parent1 == null)
    {
      node.color = false;
    }
    else
    {
      if (!parent1.color)
        return;
      TextAnchorNode parent2 = parent1.parent;
      TextAnchorNode textAnchorNode = TextAnchorTree.Sibling(parent1);
      if (textAnchorNode != null && textAnchorNode.color)
      {
        parent1.color = false;
        textAnchorNode.color = false;
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
        TextAnchorNode parent3 = node.parent;
        TextAnchorNode parent4 = parent3.parent;
        parent3.color = false;
        parent4.color = true;
        if (node == parent3.left && parent3 == parent4.left)
          this.RotateRight(parent4);
        else
          this.RotateLeft(parent4);
      }
    }
  }

  private void RemoveNode(TextAnchorNode removedNode)
  {
    if (removedNode.left != null && removedNode.right != null)
    {
      TextAnchorNode leftMost = removedNode.right.LeftMost;
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
      TextAnchorNode parent = removedNode.parent;
      TextAnchorNode textAnchorNode = removedNode.left ?? removedNode.right;
      this.ReplaceNode(removedNode, textAnchorNode);
      if (parent != null)
        this.UpdateAugmentedData(parent);
      if (removedNode.color)
        return;
      if (textAnchorNode != null && textAnchorNode.color)
        textAnchorNode.color = false;
      else
        this.FixTreeOnDelete(textAnchorNode, parent);
    }
  }

  private void FixTreeOnDelete(TextAnchorNode node, TextAnchorNode parentNode)
  {
    if (parentNode == null)
      return;
    TextAnchorNode p = TextAnchorTree.Sibling(node, parentNode);
    if (p.color)
    {
      parentNode.color = true;
      p.color = false;
      if (node == parentNode.left)
        this.RotateLeft(parentNode);
      else
        this.RotateRight(parentNode);
      p = TextAnchorTree.Sibling(node, parentNode);
    }
    if (!parentNode.color && !p.color && !TextAnchorTree.GetColor(p.left) && !TextAnchorTree.GetColor(p.right))
    {
      p.color = true;
      this.FixTreeOnDelete(parentNode, parentNode.parent);
    }
    else if (parentNode.color && !p.color && !TextAnchorTree.GetColor(p.left) && !TextAnchorTree.GetColor(p.right))
    {
      p.color = true;
      parentNode.color = false;
    }
    else
    {
      if (node == parentNode.left && !p.color && TextAnchorTree.GetColor(p.left) && !TextAnchorTree.GetColor(p.right))
      {
        p.color = true;
        p.left.color = false;
        this.RotateRight(p);
      }
      else if (node == parentNode.right && !p.color && TextAnchorTree.GetColor(p.right) && !TextAnchorTree.GetColor(p.left))
      {
        p.color = true;
        p.right.color = false;
        this.RotateLeft(p);
      }
      TextAnchorNode textAnchorNode = TextAnchorTree.Sibling(node, parentNode);
      textAnchorNode.color = parentNode.color;
      parentNode.color = false;
      if (node == parentNode.left)
      {
        if (textAnchorNode.right != null)
          textAnchorNode.right.color = false;
        this.RotateLeft(parentNode);
      }
      else
      {
        if (textAnchorNode.left != null)
          textAnchorNode.left.color = false;
        this.RotateRight(parentNode);
      }
    }
  }

  private void ReplaceNode(TextAnchorNode replacedNode, TextAnchorNode newNode)
  {
    if (replacedNode.parent == null)
      this.root = newNode;
    else if (replacedNode.parent.left == replacedNode)
      replacedNode.parent.left = newNode;
    else
      replacedNode.parent.right = newNode;
    if (newNode != null)
      newNode.parent = replacedNode.parent;
    replacedNode.parent = (TextAnchorNode) null;
  }

  private void RotateLeft(TextAnchorNode p)
  {
    TextAnchorNode right = p.right;
    this.ReplaceNode(p, right);
    p.right = right.left;
    if (p.right != null)
      p.right.parent = p;
    right.left = p;
    p.parent = right;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(right);
  }

  private void RotateRight(TextAnchorNode p)
  {
    TextAnchorNode left = p.left;
    this.ReplaceNode(p, left);
    p.left = left.right;
    if (p.left != null)
      p.left.parent = p;
    left.right = p;
    p.parent = left;
    this.UpdateAugmentedData(p);
    this.UpdateAugmentedData(left);
  }

  private static TextAnchorNode Sibling(TextAnchorNode node)
  {
    return node == node.parent.left ? node.parent.right : node.parent.left;
  }

  private static TextAnchorNode Sibling(TextAnchorNode node, TextAnchorNode parentNode)
  {
    return node == parentNode.left ? parentNode.right : parentNode.left;
  }

  private static bool GetColor(TextAnchorNode node) => node != null && node.color;

  [Conditional("DATACONSISTENCYTEST")]
  internal void CheckProperties()
  {
  }
}
