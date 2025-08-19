// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.HeightTree
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class HeightTree : ILineTracker, IDisposable
{
  private const bool RED = true;
  private const bool BLACK = false;
  private readonly TextDocument document;
  private HeightTreeNode root;
  private WeakLineTracker weakLineTracker;
  private double defaultLineHeight;
  private bool inRemoval;
  private List<HeightTreeNode> nodesToCheckForMerging;

  public HeightTree(TextDocument document, double defaultLineHeight)
  {
    this.document = document;
    this.weakLineTracker = WeakLineTracker.Register(document, (ILineTracker) this);
    this.DefaultLineHeight = defaultLineHeight;
    this.RebuildDocument();
  }

  public void Dispose()
  {
    if (this.weakLineTracker != null)
      this.weakLineTracker.Deregister();
    this.root = (HeightTreeNode) null;
    this.weakLineTracker = (WeakLineTracker) null;
  }

  public double DefaultLineHeight
  {
    get => this.defaultLineHeight;
    set
    {
      double defaultLineHeight = this.defaultLineHeight;
      if (defaultLineHeight == value)
        return;
      this.defaultLineHeight = value;
      foreach (HeightTreeNode allNode in this.AllNodes)
      {
        if (allNode.lineNode.height == defaultLineHeight)
        {
          allNode.lineNode.height = value;
          HeightTree.UpdateAugmentedData(allNode, HeightTree.UpdateAfterChildrenChangeRecursionMode.IfRequired);
        }
      }
    }
  }

  private HeightTreeNode GetNode(DocumentLine ls) => this.GetNodeByIndex(ls.LineNumber - 1);

  void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
  {
  }

  void ILineTracker.SetLineLength(DocumentLine ls, int newTotalLength)
  {
  }

  public void RebuildDocument()
  {
    foreach (CollapsedLineSection collapsedSection in this.GetAllCollapsedSections())
    {
      collapsedSection.Start = (DocumentLine) null;
      collapsedSection.End = (DocumentLine) null;
    }
    HeightTreeNode[] nodes = new HeightTreeNode[this.document.LineCount];
    int num = 0;
    foreach (DocumentLine line in (IEnumerable<DocumentLine>) this.document.Lines)
      nodes[num++] = new HeightTreeNode(line, this.defaultLineHeight);
    int treeHeight = DocumentLineTree.GetTreeHeight(nodes.Length);
    this.root = this.BuildTree(nodes, 0, nodes.Length, treeHeight);
    this.root.color = false;
  }

  private HeightTreeNode BuildTree(HeightTreeNode[] nodes, int start, int end, int subtreeHeight)
  {
    if (start == end)
      return (HeightTreeNode) null;
    int end1 = (start + end) / 2;
    HeightTreeNode node = nodes[end1];
    node.left = this.BuildTree(nodes, start, end1, subtreeHeight - 1);
    node.right = this.BuildTree(nodes, end1 + 1, end, subtreeHeight - 1);
    if (node.left != null)
      node.left.parent = node;
    if (node.right != null)
      node.right.parent = node;
    if (subtreeHeight == 1)
      node.color = true;
    HeightTree.UpdateAugmentedData(node, HeightTree.UpdateAfterChildrenChangeRecursionMode.None);
    return node;
  }

  void ILineTracker.BeforeRemoveLine(DocumentLine line)
  {
    HeightTreeNode node = this.GetNode(line);
    if (node.lineNode.collapsedSections != null)
    {
      foreach (CollapsedLineSection section in node.lineNode.collapsedSections.ToArray())
      {
        if (section.Start == line && section.End == line)
        {
          section.Start = (DocumentLine) null;
          section.End = (DocumentLine) null;
        }
        else if (section.Start == line)
        {
          this.Uncollapse(section);
          section.Start = line.NextLine;
          this.AddCollapsedSection(section, section.End.LineNumber - section.Start.LineNumber + 1);
        }
        else if (section.End == line)
        {
          this.Uncollapse(section);
          section.End = line.PreviousLine;
          this.AddCollapsedSection(section, section.End.LineNumber - section.Start.LineNumber + 1);
        }
      }
    }
    this.BeginRemoval();
    this.RemoveNode(node);
    node.lineNode.collapsedSections = (List<CollapsedLineSection>) null;
    this.EndRemoval();
  }

  void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
  {
    this.InsertAfter(this.GetNode(insertionPos), newLine);
  }

  private HeightTreeNode InsertAfter(HeightTreeNode node, DocumentLine newLine)
  {
    HeightTreeNode newNode = new HeightTreeNode(newLine, this.defaultLineHeight);
    if (node.right == null)
    {
      if (node.lineNode.collapsedSections != null)
      {
        foreach (CollapsedLineSection collapsedSection in node.lineNode.collapsedSections)
        {
          if (collapsedSection.End != node.documentLine)
            newNode.AddDirectlyCollapsed(collapsedSection);
        }
      }
      this.InsertAsRight(node, newNode);
    }
    else
    {
      node = node.right.LeftMost;
      if (node.lineNode.collapsedSections != null)
      {
        foreach (CollapsedLineSection collapsedSection in node.lineNode.collapsedSections)
        {
          if (collapsedSection.Start != node.documentLine)
            newNode.AddDirectlyCollapsed(collapsedSection);
        }
      }
      this.InsertAsLeft(node, newNode);
    }
    return newNode;
  }

  private static void UpdateAfterChildrenChange(HeightTreeNode node)
  {
    HeightTree.UpdateAugmentedData(node, HeightTree.UpdateAfterChildrenChangeRecursionMode.IfRequired);
  }

  private static void UpdateAugmentedData(
    HeightTreeNode node,
    HeightTree.UpdateAfterChildrenChangeRecursionMode mode)
  {
    int num = 1;
    double d1 = node.lineNode.TotalHeight;
    if (node.left != null)
    {
      num += node.left.totalCount;
      d1 += node.left.totalHeight;
    }
    if (node.right != null)
    {
      num += node.right.totalCount;
      d1 += node.right.totalHeight;
    }
    if (node.IsDirectlyCollapsed)
      d1 = 0.0;
    if (num == node.totalCount && d1.IsClose(node.totalHeight) && mode != HeightTree.UpdateAfterChildrenChangeRecursionMode.WholeBranch)
      return;
    node.totalCount = num;
    node.totalHeight = d1;
    if (node.parent == null || mode == HeightTree.UpdateAfterChildrenChangeRecursionMode.None)
      return;
    HeightTree.UpdateAugmentedData(node.parent, mode);
  }

  private void UpdateAfterRotateLeft(HeightTreeNode node)
  {
    List<CollapsedLineSection> collapsedSections1 = node.parent.collapsedSections;
    List<CollapsedLineSection> collapsedSections2 = node.collapsedSections;
    node.parent.collapsedSections = collapsedSections2;
    node.collapsedSections = (List<CollapsedLineSection>) null;
    if (collapsedSections1 != null)
    {
      foreach (CollapsedLineSection section in collapsedSections1)
      {
        if (node.parent.right != null)
          node.parent.right.AddDirectlyCollapsed(section);
        node.parent.lineNode.AddDirectlyCollapsed(section);
        if (node.right != null)
          node.right.AddDirectlyCollapsed(section);
      }
    }
    this.MergeCollapsedSectionsIfPossible(node);
    HeightTree.UpdateAfterChildrenChange(node);
  }

  private void UpdateAfterRotateRight(HeightTreeNode node)
  {
    List<CollapsedLineSection> collapsedSections1 = node.parent.collapsedSections;
    List<CollapsedLineSection> collapsedSections2 = node.collapsedSections;
    node.parent.collapsedSections = collapsedSections2;
    node.collapsedSections = (List<CollapsedLineSection>) null;
    if (collapsedSections1 != null)
    {
      foreach (CollapsedLineSection section in collapsedSections1)
      {
        if (node.parent.left != null)
          node.parent.left.AddDirectlyCollapsed(section);
        node.parent.lineNode.AddDirectlyCollapsed(section);
        if (node.left != null)
          node.left.AddDirectlyCollapsed(section);
      }
    }
    this.MergeCollapsedSectionsIfPossible(node);
    HeightTree.UpdateAfterChildrenChange(node);
  }

  private void BeforeNodeRemove(HeightTreeNode removedNode)
  {
    List<CollapsedLineSection> collapsedSections = removedNode.collapsedSections;
    if (collapsedSections != null)
    {
      HeightTreeNode heightTreeNode = removedNode.left ?? removedNode.right;
      if (heightTreeNode != null)
      {
        foreach (CollapsedLineSection section in collapsedSections)
          heightTreeNode.AddDirectlyCollapsed(section);
      }
    }
    if (removedNode.parent == null)
      return;
    this.MergeCollapsedSectionsIfPossible(removedNode.parent);
  }

  private void BeforeNodeReplace(
    HeightTreeNode removedNode,
    HeightTreeNode newNode,
    HeightTreeNode newNodeOldParent)
  {
    for (; newNodeOldParent != removedNode; newNodeOldParent = newNodeOldParent.parent)
    {
      if (newNodeOldParent.collapsedSections != null)
      {
        foreach (CollapsedLineSection collapsedSection in newNodeOldParent.collapsedSections)
          newNode.lineNode.AddDirectlyCollapsed(collapsedSection);
      }
    }
    if (newNode.collapsedSections != null)
    {
      foreach (CollapsedLineSection collapsedSection in newNode.collapsedSections)
        newNode.lineNode.AddDirectlyCollapsed(collapsedSection);
    }
    newNode.collapsedSections = removedNode.collapsedSections;
    this.MergeCollapsedSectionsIfPossible(newNode);
  }

  private void BeginRemoval()
  {
    if (this.nodesToCheckForMerging == null)
      this.nodesToCheckForMerging = new List<HeightTreeNode>();
    this.inRemoval = true;
  }

  private void EndRemoval()
  {
    this.inRemoval = false;
    foreach (HeightTreeNode node in this.nodesToCheckForMerging)
      this.MergeCollapsedSectionsIfPossible(node);
    this.nodesToCheckForMerging.Clear();
  }

  private void MergeCollapsedSectionsIfPossible(HeightTreeNode node)
  {
    if (this.inRemoval)
    {
      this.nodesToCheckForMerging.Add(node);
    }
    else
    {
      bool flag = false;
      List<CollapsedLineSection> collapsedSections = node.lineNode.collapsedSections;
      if (collapsedSections != null)
      {
        for (int index = collapsedSections.Count - 1; index >= 0; --index)
        {
          CollapsedLineSection section = collapsedSections[index];
          if (section.Start != node.documentLine && section.End != node.documentLine && (node.left == null || node.left.collapsedSections != null && node.left.collapsedSections.Contains(section)) && (node.right == null || node.right.collapsedSections != null && node.right.collapsedSections.Contains(section)))
          {
            if (node.left != null)
              node.left.RemoveDirectlyCollapsed(section);
            if (node.right != null)
              node.right.RemoveDirectlyCollapsed(section);
            collapsedSections.RemoveAt(index);
            node.AddDirectlyCollapsed(section);
            flag = true;
          }
        }
        if (collapsedSections.Count == 0)
          node.lineNode.collapsedSections = (List<CollapsedLineSection>) null;
      }
      if (!flag || node.parent == null)
        return;
      this.MergeCollapsedSectionsIfPossible(node.parent);
    }
  }

  private HeightTreeNode GetNodeByIndex(int index)
  {
    HeightTreeNode nodeByIndex = this.root;
    while (true)
    {
      for (; nodeByIndex.left == null || index >= nodeByIndex.left.totalCount; nodeByIndex = nodeByIndex.right)
      {
        if (nodeByIndex.left != null)
          index -= nodeByIndex.left.totalCount;
        if (index == 0)
          return nodeByIndex;
        --index;
      }
      nodeByIndex = nodeByIndex.left;
    }
  }

  private HeightTreeNode GetNodeByVisualPosition(double position)
  {
    HeightTreeNode byVisualPosition = this.root;
    while (true)
    {
      double num1 = position;
      if (byVisualPosition.left != null)
      {
        num1 -= byVisualPosition.left.totalHeight;
        if (num1 < 0.0)
        {
          byVisualPosition = byVisualPosition.left;
          continue;
        }
      }
      double num2 = num1 - byVisualPosition.lineNode.TotalHeight;
      if (num2 >= 0.0)
      {
        if (byVisualPosition.right == null || byVisualPosition.right.totalHeight == 0.0)
        {
          if (byVisualPosition.lineNode.TotalHeight <= 0.0 && byVisualPosition.left != null)
            byVisualPosition = byVisualPosition.left;
          else
            goto label_8;
        }
        else
        {
          position = num2;
          byVisualPosition = byVisualPosition.right;
        }
      }
      else
        break;
    }
    return byVisualPosition;
label_8:
    return byVisualPosition;
  }

  private static double GetVisualPositionFromNode(HeightTreeNode node)
  {
    double positionFromNode = node.left != null ? node.left.totalHeight : 0.0;
    for (; node.parent != null; node = node.parent)
    {
      if (node.IsDirectlyCollapsed)
        positionFromNode = 0.0;
      if (node == node.parent.right)
      {
        if (node.parent.left != null)
          positionFromNode += node.parent.left.totalHeight;
        positionFromNode += node.parent.lineNode.TotalHeight;
      }
    }
    return positionFromNode;
  }

  public DocumentLine GetLineByNumber(int number) => this.GetNodeByIndex(number - 1).documentLine;

  public DocumentLine GetLineByVisualPosition(double position)
  {
    return this.GetNodeByVisualPosition(position).documentLine;
  }

  public double GetVisualPosition(DocumentLine line)
  {
    return HeightTree.GetVisualPositionFromNode(this.GetNode(line));
  }

  public double GetHeight(DocumentLine line) => this.GetNode(line).lineNode.height;

  public void SetHeight(DocumentLine line, double val)
  {
    HeightTreeNode node = this.GetNode(line);
    node.lineNode.height = val;
    HeightTree.UpdateAfterChildrenChange(node);
  }

  public bool GetIsCollapsed(int lineNumber)
  {
    HeightTreeNode nodeByIndex = this.GetNodeByIndex(lineNumber - 1);
    return nodeByIndex.lineNode.IsDirectlyCollapsed || HeightTree.GetIsCollapedFromNode(nodeByIndex);
  }

  public CollapsedLineSection CollapseText(DocumentLine start, DocumentLine end)
  {
    if (!this.document.Lines.Contains(start))
      throw new ArgumentException("Line is not part of this document", nameof (start));
    if (!this.document.Lines.Contains(end))
      throw new ArgumentException("Line is not part of this document", nameof (end));
    int sectionLength = end.LineNumber - start.LineNumber + 1;
    if (sectionLength < 0)
      throw new ArgumentException("start must be a line before end");
    CollapsedLineSection section = new CollapsedLineSection(this, start, end);
    this.AddCollapsedSection(section, sectionLength);
    return section;
  }

  public int LineCount => this.root.totalCount;

  public double TotalHeight => this.root.totalHeight;

  private IEnumerable<HeightTreeNode> AllNodes
  {
    get
    {
      if (this.root != null)
      {
        for (HeightTreeNode node = this.root.LeftMost; node != null; node = node.Successor)
          yield return node;
      }
    }
  }

  internal IEnumerable<CollapsedLineSection> GetAllCollapsedSections()
  {
    List<CollapsedLineSection> emptyCSList = new List<CollapsedLineSection>();
    return this.AllNodes.SelectMany<HeightTreeNode, CollapsedLineSection>((Func<HeightTreeNode, IEnumerable<CollapsedLineSection>>) (node => (node.lineNode.collapsedSections ?? emptyCSList).Concat<CollapsedLineSection>((IEnumerable<CollapsedLineSection>) (node.collapsedSections ?? emptyCSList)))).Distinct<CollapsedLineSection>();
  }

  private void InsertAsLeft(HeightTreeNode parentNode, HeightTreeNode newNode)
  {
    parentNode.left = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    HeightTree.UpdateAfterChildrenChange(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void InsertAsRight(HeightTreeNode parentNode, HeightTreeNode newNode)
  {
    parentNode.right = newNode;
    newNode.parent = parentNode;
    newNode.color = true;
    HeightTree.UpdateAfterChildrenChange(parentNode);
    this.FixTreeOnInsert(newNode);
  }

  private void FixTreeOnInsert(HeightTreeNode node)
  {
    HeightTreeNode parent1 = node.parent;
    if (parent1 == null)
    {
      node.color = false;
    }
    else
    {
      if (!parent1.color)
        return;
      HeightTreeNode parent2 = parent1.parent;
      HeightTreeNode heightTreeNode = HeightTree.Sibling(parent1);
      if (heightTreeNode != null && heightTreeNode.color)
      {
        parent1.color = false;
        heightTreeNode.color = false;
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
        HeightTreeNode parent3 = node.parent;
        HeightTreeNode parent4 = parent3.parent;
        parent3.color = false;
        parent4.color = true;
        if (node == parent3.left && parent3 == parent4.left)
          this.RotateRight(parent4);
        else
          this.RotateLeft(parent4);
      }
    }
  }

  private void RemoveNode(HeightTreeNode removedNode)
  {
    if (removedNode.left != null && removedNode.right != null)
    {
      HeightTreeNode leftMost = removedNode.right.LeftMost;
      HeightTreeNode parent = leftMost.parent;
      this.RemoveNode(leftMost);
      this.BeforeNodeReplace(removedNode, leftMost, parent);
      this.ReplaceNode(removedNode, leftMost);
      leftMost.left = removedNode.left;
      if (leftMost.left != null)
        leftMost.left.parent = leftMost;
      leftMost.right = removedNode.right;
      if (leftMost.right != null)
        leftMost.right.parent = leftMost;
      leftMost.color = removedNode.color;
      HeightTree.UpdateAfterChildrenChange(leftMost);
      if (leftMost.parent == null)
        return;
      HeightTree.UpdateAfterChildrenChange(leftMost.parent);
    }
    else
    {
      HeightTreeNode parent = removedNode.parent;
      HeightTreeNode heightTreeNode = removedNode.left ?? removedNode.right;
      this.BeforeNodeRemove(removedNode);
      this.ReplaceNode(removedNode, heightTreeNode);
      if (parent != null)
        HeightTree.UpdateAfterChildrenChange(parent);
      if (removedNode.color)
        return;
      if (heightTreeNode != null && heightTreeNode.color)
        heightTreeNode.color = false;
      else
        this.FixTreeOnDelete(heightTreeNode, parent);
    }
  }

  private void FixTreeOnDelete(HeightTreeNode node, HeightTreeNode parentNode)
  {
    if (parentNode == null)
      return;
    HeightTreeNode p = HeightTree.Sibling(node, parentNode);
    if (p.color)
    {
      parentNode.color = true;
      p.color = false;
      if (node == parentNode.left)
        this.RotateLeft(parentNode);
      else
        this.RotateRight(parentNode);
      p = HeightTree.Sibling(node, parentNode);
    }
    if (!parentNode.color && !p.color && !HeightTree.GetColor(p.left) && !HeightTree.GetColor(p.right))
    {
      p.color = true;
      this.FixTreeOnDelete(parentNode, parentNode.parent);
    }
    else if (parentNode.color && !p.color && !HeightTree.GetColor(p.left) && !HeightTree.GetColor(p.right))
    {
      p.color = true;
      parentNode.color = false;
    }
    else
    {
      if (node == parentNode.left && !p.color && HeightTree.GetColor(p.left) && !HeightTree.GetColor(p.right))
      {
        p.color = true;
        p.left.color = false;
        this.RotateRight(p);
      }
      else if (node == parentNode.right && !p.color && HeightTree.GetColor(p.right) && !HeightTree.GetColor(p.left))
      {
        p.color = true;
        p.right.color = false;
        this.RotateLeft(p);
      }
      HeightTreeNode heightTreeNode = HeightTree.Sibling(node, parentNode);
      heightTreeNode.color = parentNode.color;
      parentNode.color = false;
      if (node == parentNode.left)
      {
        if (heightTreeNode.right != null)
          heightTreeNode.right.color = false;
        this.RotateLeft(parentNode);
      }
      else
      {
        if (heightTreeNode.left != null)
          heightTreeNode.left.color = false;
        this.RotateRight(parentNode);
      }
    }
  }

  private void ReplaceNode(HeightTreeNode replacedNode, HeightTreeNode newNode)
  {
    if (replacedNode.parent == null)
      this.root = newNode;
    else if (replacedNode.parent.left == replacedNode)
      replacedNode.parent.left = newNode;
    else
      replacedNode.parent.right = newNode;
    if (newNode != null)
      newNode.parent = replacedNode.parent;
    replacedNode.parent = (HeightTreeNode) null;
  }

  private void RotateLeft(HeightTreeNode p)
  {
    HeightTreeNode right = p.right;
    this.ReplaceNode(p, right);
    p.right = right.left;
    if (p.right != null)
      p.right.parent = p;
    right.left = p;
    p.parent = right;
    this.UpdateAfterRotateLeft(p);
  }

  private void RotateRight(HeightTreeNode p)
  {
    HeightTreeNode left = p.left;
    this.ReplaceNode(p, left);
    p.left = left.right;
    if (p.left != null)
      p.left.parent = p;
    left.right = p;
    p.parent = left;
    this.UpdateAfterRotateRight(p);
  }

  private static HeightTreeNode Sibling(HeightTreeNode node)
  {
    return node == node.parent.left ? node.parent.right : node.parent.left;
  }

  private static HeightTreeNode Sibling(HeightTreeNode node, HeightTreeNode parentNode)
  {
    return node == parentNode.left ? parentNode.right : parentNode.left;
  }

  private static bool GetColor(HeightTreeNode node) => node != null && node.color;

  private static bool GetIsCollapedFromNode(HeightTreeNode node)
  {
    for (; node != null; node = node.parent)
    {
      if (node.IsDirectlyCollapsed)
        return true;
    }
    return false;
  }

  internal void AddCollapsedSection(CollapsedLineSection section, int sectionLength)
  {
    this.AddRemoveCollapsedSection(section, sectionLength, true);
  }

  private void AddRemoveCollapsedSection(CollapsedLineSection section, int sectionLength, bool add)
  {
    HeightTreeNode heightTreeNode = this.GetNode(section.Start);
    while (true)
    {
      if (add)
        heightTreeNode.lineNode.AddDirectlyCollapsed(section);
      else
        heightTreeNode.lineNode.RemoveDirectlyCollapsed(section);
      --sectionLength;
      if (sectionLength != 0)
      {
        if (heightTreeNode.right != null)
        {
          if (heightTreeNode.right.totalCount < sectionLength)
          {
            if (add)
              heightTreeNode.right.AddDirectlyCollapsed(section);
            else
              heightTreeNode.right.RemoveDirectlyCollapsed(section);
            sectionLength -= heightTreeNode.right.totalCount;
          }
          else
            break;
        }
        HeightTreeNode parent;
        for (parent = heightTreeNode.parent; parent.right == heightTreeNode; parent = heightTreeNode.parent)
          heightTreeNode = parent;
        heightTreeNode = parent;
      }
      else
        goto label_16;
    }
    HeightTree.AddRemoveCollapsedSectionDown(section, heightTreeNode.right, sectionLength, add);
label_16:
    HeightTree.UpdateAugmentedData(this.GetNode(section.Start), HeightTree.UpdateAfterChildrenChangeRecursionMode.WholeBranch);
    HeightTree.UpdateAugmentedData(this.GetNode(section.End), HeightTree.UpdateAfterChildrenChangeRecursionMode.WholeBranch);
  }

  private static void AddRemoveCollapsedSectionDown(
    CollapsedLineSection section,
    HeightTreeNode node,
    int sectionLength,
    bool add)
  {
    while (true)
    {
      for (node = node.right; node.left != null; node = node.left)
      {
        if (node.left.totalCount < sectionLength)
        {
          if (add)
            node.left.AddDirectlyCollapsed(section);
          else
            node.left.RemoveDirectlyCollapsed(section);
          sectionLength -= node.left.totalCount;
          break;
        }
      }
      if (add)
        node.lineNode.AddDirectlyCollapsed(section);
      else
        node.lineNode.RemoveDirectlyCollapsed(section);
      --sectionLength;
      if (sectionLength == 0)
        break;
    }
  }

  public void Uncollapse(CollapsedLineSection section)
  {
    int sectionLength = section.End.LineNumber - section.Start.LineNumber + 1;
    this.AddRemoveCollapsedSection(section, sectionLength, false);
  }

  private enum UpdateAfterChildrenChangeRecursionMode
  {
    None,
    IfRequired,
    WholeBranch,
  }
}
