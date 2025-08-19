// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Range
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class Range : IRange
{
  private Range.Boundary _start;
  private Range.Boundary _end;

  public Range(IDocument document)
  {
    this._start = new Range.Boundary()
    {
      Offset = 0,
      Node = (INode) document
    };
    this._end = new Range.Boundary()
    {
      Offset = 0,
      Node = (INode) document
    };
  }

  private Range(Range.Boundary start, Range.Boundary end)
  {
    this._start = start;
    this._end = end;
  }

  public INode Root => this._start.Node.GetRoot();

  public IEnumerable<INode> Nodes
  {
    get => this.CommonAncestor.GetNodes<INode>(predicate: new Func<INode, bool>(this.Intersects));
  }

  public INode Head => this._start.Node;

  public int Start => this._start.Offset;

  public INode Tail => this._end.Node;

  public int End => this._end.Offset;

  public bool IsCollapsed => this._start.Node == this._end.Node;

  public INode CommonAncestor
  {
    get
    {
      INode otherNode = this.Head;
      while (otherNode != null && !this.Tail.Contains(otherNode))
        otherNode = otherNode.Parent;
      return otherNode;
    }
  }

  public void StartWith(INode refNode, int offset)
  {
    if (refNode == null)
      throw new ArgumentNullException(nameof (refNode));
    if (refNode.NodeType == NodeType.DocumentType)
      throw new DomException(DomError.InvalidNodeType);
    if (offset > refNode.ChildNodes.Length)
      throw new DomException(DomError.IndexSizeError);
    Range.Boundary boundary = new Range.Boundary()
    {
      Node = refNode,
      Offset = offset
    };
    if (!(boundary > this._end) && this.Root == refNode.GetRoot())
      return;
    this._start = boundary;
  }

  public void EndWith(INode refNode, int offset)
  {
    if (refNode == null)
      throw new ArgumentNullException(nameof (refNode));
    if (refNode.NodeType == NodeType.DocumentType)
      throw new DomException(DomError.InvalidNodeType);
    if (offset > refNode.ChildNodes.Length)
      throw new DomException(DomError.IndexSizeError);
    Range.Boundary boundary = new Range.Boundary()
    {
      Node = refNode,
      Offset = offset
    };
    if (!(boundary < this._start) && this.Root == refNode.GetRoot())
      return;
    this._end = boundary;
  }

  public void StartBefore(INode refNode)
  {
    INode node = refNode != null ? refNode.Parent : throw new ArgumentNullException(nameof (refNode));
    if (node == null)
      throw new DomException(DomError.InvalidNodeType);
    this._start = new Range.Boundary()
    {
      Node = node,
      Offset = node.ChildNodes.Index(refNode)
    };
  }

  public void EndBefore(INode refNode)
  {
    INode node = refNode != null ? refNode.Parent : throw new ArgumentNullException(nameof (refNode));
    if (node == null)
      throw new DomException(DomError.InvalidNodeType);
    this._end = new Range.Boundary()
    {
      Node = node,
      Offset = node.ChildNodes.Index(refNode)
    };
  }

  public void StartAfter(INode refNode)
  {
    INode node = refNode != null ? refNode.Parent : throw new ArgumentNullException(nameof (refNode));
    if (node == null)
      throw new DomException(DomError.InvalidNodeType);
    this._start = new Range.Boundary()
    {
      Node = node,
      Offset = node.ChildNodes.Index(refNode) + 1
    };
  }

  public void EndAfter(INode refNode)
  {
    INode node = refNode != null ? refNode.Parent : throw new ArgumentNullException(nameof (refNode));
    if (node == null)
      throw new DomException(DomError.InvalidNodeType);
    this._end = new Range.Boundary()
    {
      Node = node,
      Offset = node.ChildNodes.Index(refNode) + 1
    };
  }

  public void Collapse(bool toStart)
  {
    if (toStart)
      this._end = this._start;
    else
      this._start = this._end;
  }

  public void Select(INode refNode)
  {
    INode node = refNode != null ? refNode.Parent : throw new ArgumentNullException(nameof (refNode));
    if (node == null)
      throw new DomException(DomError.InvalidNodeType);
    int num = node.ChildNodes.Index(refNode);
    this._start = new Range.Boundary()
    {
      Node = node,
      Offset = num
    };
    this._end = new Range.Boundary()
    {
      Node = node,
      Offset = num + 1
    };
  }

  public void SelectContent(INode refNode)
  {
    if (refNode == null)
      throw new ArgumentNullException(nameof (refNode));
    if (refNode.NodeType == NodeType.DocumentType)
      throw new DomException(DomError.InvalidNodeType);
    int length = refNode.ChildNodes.Length;
    this._start = new Range.Boundary()
    {
      Node = refNode,
      Offset = 0
    };
    this._end = new Range.Boundary()
    {
      Node = refNode,
      Offset = length
    };
  }

  public void ClearContent()
  {
    if (this._start.Equals(this._end))
      return;
    Range.Boundary boundary1 = new Range.Boundary();
    Range.Boundary start = this._start;
    Range.Boundary end = this._end;
    if (end.Node == start.Node && start.Node is ICharacterData)
    {
      int offset1 = start.Offset;
      ICharacterData node = (ICharacterData) start.Node;
      int num = end.Offset - start.Offset;
      int offset2 = offset1;
      int count = num;
      string empty = string.Empty;
      node.Replace(offset2, count, empty);
    }
    else
    {
      INode[] array = this.Nodes.Where<INode>((Func<INode, bool>) (m => !this.Intersects(m.Parent))).ToArray<INode>();
      Range.Boundary boundary2;
      if (!start.Node.IsInclusiveAncestorOf(end.Node))
      {
        INode node = start.Node;
        while (node.Parent != null && node.Parent.IsInclusiveAncestorOf(end.Node))
          node = node.Parent;
        boundary2 = new Range.Boundary()
        {
          Node = node.Parent,
          Offset = node.Parent.ChildNodes.Index(node) + 1
        };
      }
      else
        boundary2 = start;
      if (start.Node is ICharacterData)
      {
        int offset3 = start.Offset;
        ICharacterData node = (ICharacterData) start.Node;
        int num = end.Offset - start.Offset;
        int offset4 = offset3;
        int count = num;
        string empty = string.Empty;
        node.Replace(offset4, count, empty);
      }
      foreach (INode child in array)
        child.Parent.RemoveChild(child);
      if (end.Node is ICharacterData)
      {
        int num = 0;
        ICharacterData node = (ICharacterData) end.Node;
        int offset5 = end.Offset;
        int offset6 = num;
        int count = offset5;
        string empty = string.Empty;
        node.Replace(offset6, count, empty);
      }
      this._start = boundary2;
      this._end = boundary2;
    }
  }

  public IDocumentFragment ExtractContent()
  {
    IDocumentFragment documentFragment = this._start.Node.Owner.CreateDocumentFragment();
    if (!this._start.Equals(this._end))
    {
      Range.Boundary boundary1 = this._start;
      Range.Boundary start1 = this._start;
      Range.Boundary end1 = this._end;
      if (start1.Node == end1.Node && this._start.Node is ICharacterData)
      {
        ICharacterData node = (ICharacterData) start1.Node;
        int offset = start1.Offset;
        int count = end1.Offset - start1.Offset;
        ICharacterData child = (ICharacterData) node.Clone();
        child.Data = node.Substring(offset, count);
        documentFragment.AppendChild((INode) child);
        node.Replace(offset, count, string.Empty);
      }
      else
      {
        INode parent1 = start1.Node;
        while (!parent1.IsInclusiveAncestorOf(end1.Node))
          parent1 = parent1.Parent;
        INode node1 = !start1.Node.IsInclusiveAncestorOf(end1.Node) ? parent1.GetNodes<INode>(predicate: new Func<INode, bool>(this.IsPartiallyContained)).FirstOrDefault<INode>() : (INode) null;
        INode node2 = !end1.Node.IsInclusiveAncestorOf(start1.Node) ? parent1.GetNodes<INode>(predicate: new Func<INode, bool>(this.IsPartiallyContained)).LastOrDefault<INode>() : (INode) null;
        List<INode> list = parent1.GetNodes<INode>(predicate: new Func<INode, bool>(this.Intersects)).ToList<INode>();
        if (list.OfType<IDocumentType>().Any<IDocumentType>())
          throw new DomException(DomError.HierarchyRequest);
        Range.Boundary boundary2;
        if (!start1.Node.IsInclusiveAncestorOf(end1.Node))
        {
          INode parent2 = start1.Node;
          while (parent2.Parent != null && !parent2.IsInclusiveAncestorOf(end1.Node))
            parent2 = parent2.Parent;
          boundary2 = new Range.Boundary();
          boundary2.Node = parent2;
          boundary2.Offset = parent2.Parent.ChildNodes.Index(parent2) + 1;
          boundary1 = boundary2;
        }
        if (node1 is ICharacterData)
        {
          ICharacterData node3 = (ICharacterData) start1.Node;
          int offset = start1.Offset;
          int count = node3.Length - start1.Offset;
          ICharacterData child = (ICharacterData) node3.Clone();
          child.Data = node3.Substring(offset, count);
          documentFragment.AppendChild((INode) child);
          node3.Replace(offset, count, string.Empty);
        }
        else if (node1 != null)
        {
          INode child = node1.Clone();
          documentFragment.AppendChild(child);
          Range.Boundary start2 = start1;
          boundary2 = new Range.Boundary();
          boundary2.Node = node1;
          boundary2.Offset = node1.ChildNodes.Length;
          Range.Boundary end2 = boundary2;
          IDocumentFragment content = new Range(start2, end2).ExtractContent();
          documentFragment.AppendChild((INode) content);
        }
        foreach (INode child in list)
          documentFragment.AppendChild(child);
        if (node2 is ICharacterData)
        {
          ICharacterData node4 = (ICharacterData) end1.Node;
          ICharacterData child = (ICharacterData) node4.Clone();
          child.Data = node4.Substring(0, end1.Offset);
          documentFragment.AppendChild((INode) child);
          node4.Replace(0, end1.Offset, string.Empty);
        }
        else if (node2 != null)
        {
          INode child = node2.Clone();
          documentFragment.AppendChild(child);
          IDocumentFragment content = new Range(new Range.Boundary()
          {
            Node = node2,
            Offset = 0
          }, end1).ExtractContent();
          documentFragment.AppendChild((INode) content);
        }
        this._start = boundary1;
        this._end = boundary1;
      }
    }
    return documentFragment;
  }

  public IDocumentFragment CopyContent()
  {
    IDocumentFragment documentFragment = this._start.Node.Owner.CreateDocumentFragment();
    if (!this._start.Equals(this._end))
    {
      Range.Boundary start = this._start;
      Range.Boundary end = this._end;
      if (start.Node == end.Node && this._start.Node is ICharacterData)
      {
        ICharacterData node = (ICharacterData) start.Node;
        int offset = start.Offset;
        int count = end.Offset - start.Offset;
        ICharacterData child = (ICharacterData) node.Clone();
        child.Data = node.Substring(offset, count);
        documentFragment.AppendChild((INode) child);
      }
      else
      {
        INode parent = start.Node;
        while (!parent.IsInclusiveAncestorOf(end.Node))
          parent = parent.Parent;
        INode node1 = !start.Node.IsInclusiveAncestorOf(end.Node) ? parent.GetNodes<INode>(predicate: new Func<INode, bool>(this.IsPartiallyContained)).FirstOrDefault<INode>() : (INode) null;
        INode node2 = !end.Node.IsInclusiveAncestorOf(start.Node) ? parent.GetNodes<INode>(predicate: new Func<INode, bool>(this.IsPartiallyContained)).LastOrDefault<INode>() : (INode) null;
        List<INode> list = parent.GetNodes<INode>(predicate: new Func<INode, bool>(this.Intersects)).ToList<INode>();
        if (list.OfType<IDocumentType>().Any<IDocumentType>())
          throw new DomException(DomError.HierarchyRequest);
        if (node1 is ICharacterData)
        {
          ICharacterData node3 = (ICharacterData) start.Node;
          int offset = start.Offset;
          int count = node3.Length - start.Offset;
          ICharacterData child = (ICharacterData) node3.Clone();
          child.Data = node3.Substring(offset, count);
          documentFragment.AppendChild((INode) child);
        }
        else if (node1 != null)
        {
          INode child1 = node1.Clone();
          documentFragment.AppendChild(child1);
          IDocumentFragment child2 = new Range(start, new Range.Boundary()
          {
            Node = node1,
            Offset = node1.ChildNodes.Length
          }).CopyContent();
          documentFragment.AppendChild((INode) child2);
        }
        foreach (INode node4 in list)
          documentFragment.AppendChild(node4.Clone());
        if (node2 is ICharacterData)
        {
          ICharacterData node5 = (ICharacterData) end.Node;
          ICharacterData child = (ICharacterData) node5.Clone();
          child.Data = node5.Substring(0, end.Offset);
          documentFragment.AppendChild((INode) child);
        }
        else if (node2 != null)
        {
          INode child3 = node2.Clone();
          documentFragment.AppendChild(child3);
          IDocumentFragment child4 = new Range(new Range.Boundary()
          {
            Node = node2,
            Offset = 0
          }, end).CopyContent();
          documentFragment.AppendChild((INode) child4);
        }
      }
    }
    return documentFragment;
  }

  public void Insert(INode node)
  {
    if (node == null)
      throw new ArgumentNullException(nameof (node));
    INode node1 = this._start.Node;
    NodeType nodeType = node1.NodeType;
    bool flag = nodeType == NodeType.Text;
    if (nodeType == NodeType.ProcessingInstruction || nodeType == NodeType.Comment || flag && node1.Parent == null)
      throw new DomException(DomError.HierarchyRequest);
    INode child = flag ? node1 : this._start.ChildAtOffset;
    INode parent = child == null ? node1 : child.Parent;
    parent.EnsurePreInsertionValidity(node, child);
    if (flag)
    {
      child = (INode) ((IText) node1).Split(this._start.Offset);
      parent = child.Parent;
    }
    if (node == child)
      child = child.NextSibling;
    node.Parent?.RemoveChild(node);
    int num = (child == null ? parent.ChildNodes.Length : parent.ChildNodes.Index(child)) + (node.NodeType == NodeType.DocumentFragment ? node.ChildNodes.Length : 1);
    parent.PreInsert(node, child);
    if (!this._start.Equals(this._end))
      return;
    this._end = new Range.Boundary()
    {
      Node = parent,
      Offset = num
    };
  }

  public void Surround(INode newParent)
  {
    if (newParent == null)
      throw new ArgumentNullException(nameof (newParent));
    if (this.Nodes.Any<INode>((Func<INode, bool>) (m => m.NodeType != NodeType.Text && this.IsPartiallyContained(m))))
      throw new DomException(DomError.InvalidState);
    switch (newParent.NodeType)
    {
      case NodeType.Document:
      case NodeType.DocumentType:
      case NodeType.DocumentFragment:
        throw new DomException(DomError.InvalidNodeType);
      default:
        IDocumentFragment content = this.ExtractContent();
        while (newParent.HasChildNodes)
          newParent.RemoveChild(newParent.FirstChild);
        this.Insert(newParent);
        newParent.PreInsert((INode) content, (INode) null);
        this.Select(newParent);
        break;
    }
  }

  public IRange Clone() => (IRange) new Range(this._start, this._end);

  public void Detach()
  {
  }

  public bool Contains(INode node, int offset)
  {
    if (node == null)
      throw new ArgumentNullException(nameof (node));
    if (node.GetRoot() != this.Root)
      return false;
    if (node.NodeType == NodeType.DocumentType)
      throw new DomException(DomError.InvalidNodeType);
    if (offset > node.ChildNodes.Length)
      throw new DomException(DomError.IndexSizeError);
    return !this.IsStartAfter(node, offset) && !this.IsEndBefore(node, offset);
  }

  public RangePosition CompareBoundaryTo(RangeType how, IRange sourceRange)
  {
    if (sourceRange == null)
      throw new ArgumentNullException(nameof (sourceRange));
    if (this.Root != sourceRange.Head.GetRoot())
      throw new DomException(DomError.WrongDocument);
    Range.Boundary boundary1 = new Range.Boundary();
    Range.Boundary boundary2 = new Range.Boundary();
    Range.Boundary boundary3;
    Range.Boundary other;
    switch (how)
    {
      case RangeType.StartToStart:
        boundary3 = this._start;
        other = new Range.Boundary()
        {
          Node = sourceRange.Head,
          Offset = sourceRange.Start
        };
        break;
      case RangeType.StartToEnd:
        boundary3 = this._end;
        other = new Range.Boundary()
        {
          Node = sourceRange.Head,
          Offset = sourceRange.Start
        };
        break;
      case RangeType.EndToEnd:
        boundary3 = this._start;
        other = new Range.Boundary()
        {
          Node = sourceRange.Tail,
          Offset = sourceRange.End
        };
        break;
      case RangeType.EndToStart:
        boundary3 = this._end;
        other = new Range.Boundary()
        {
          Node = sourceRange.Tail,
          Offset = sourceRange.End
        };
        break;
      default:
        throw new DomException(DomError.NotSupported);
    }
    return boundary3.CompareTo(other);
  }

  public RangePosition CompareTo(INode node, int offset)
  {
    if (node == null)
      throw new ArgumentNullException(nameof (node));
    if (this.Root != this._start.Node.GetRoot())
      throw new DomException(DomError.WrongDocument);
    if (node.NodeType == NodeType.DocumentType)
      throw new DomException(DomError.InvalidNodeType);
    if (offset > node.ChildNodes.Length)
      throw new DomException(DomError.IndexSizeError);
    if (this.IsStartAfter(node, offset))
      return RangePosition.Before;
    return this.IsEndBefore(node, offset) ? RangePosition.After : RangePosition.Equal;
  }

  public bool Intersects(INode node)
  {
    if (node == null)
      throw new ArgumentNullException(nameof (node));
    if (this.Root != node.GetRoot())
      return false;
    INode parent = node.Parent;
    if (parent == null)
      return true;
    int offset = parent.ChildNodes.Index(node);
    return this.IsEndAfter(parent, offset) && this.IsStartBefore(parent, offset + 1);
  }

  public override string ToString()
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    int start = this.Start;
    int end = this.End;
    IText head = this.Head as IText;
    IText tail = this.Tail as IText;
    if (head != null && this.Head == this.Tail)
      return head.Substring(start, end - start);
    if (head != null)
      sb.Append(head.Substring(start, head.Length - start));
    foreach (IText descendent in this.CommonAncestor.Descendents<IText>())
    {
      if (this.IsStartBefore((INode) descendent, 0) && this.IsEndAfter((INode) descendent, descendent.Length))
        sb.Append(descendent.Text);
    }
    if (tail != null)
      sb.Append(tail.Substring(0, end));
    return sb.ToPool();
  }

  private bool IsStartBefore(INode node, int offset)
  {
    return this._start < new Range.Boundary()
    {
      Node = node,
      Offset = offset
    };
  }

  private bool IsStartAfter(INode node, int offset)
  {
    return this._start > new Range.Boundary()
    {
      Node = node,
      Offset = offset
    };
  }

  private bool IsEndBefore(INode node, int offset)
  {
    return this._end < new Range.Boundary()
    {
      Node = node,
      Offset = offset
    };
  }

  private bool IsEndAfter(INode node, int offset)
  {
    return this._end > new Range.Boundary()
    {
      Node = node,
      Offset = offset
    };
  }

  private bool IsPartiallyContained(INode node)
  {
    bool flag1 = node.IsInclusiveAncestorOf(this._start.Node);
    bool flag2 = node.IsInclusiveAncestorOf(this._end.Node);
    return flag1 && !flag2 || !flag1 & flag2;
  }

  private struct Boundary : IEquatable<Range.Boundary>
  {
    public INode Node;
    public int Offset;

    public static bool operator >(Range.Boundary a, Range.Boundary b) => false;

    public static bool operator <(Range.Boundary a, Range.Boundary b) => false;

    public bool Equals(Range.Boundary other)
    {
      return this.Node == other.Node && this.Offset == other.Offset;
    }

    public RangePosition CompareTo(Range.Boundary other)
    {
      if (this < other)
        return RangePosition.Before;
      return this > other ? RangePosition.After : RangePosition.Equal;
    }

    public INode ChildAtOffset
    {
      get
      {
        return this.Node.ChildNodes.Length <= this.Offset ? (INode) null : this.Node.ChildNodes[this.Offset];
      }
    }
  }
}
