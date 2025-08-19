// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.TreeWalker
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Dom;

internal sealed class TreeWalker : ITreeWalker
{
  private readonly INode _root;
  private readonly FilterSettings _settings;
  private readonly NodeFilter _filter;
  private INode _current;

  public TreeWalker(INode root, FilterSettings settings, NodeFilter filter)
  {
    this._root = root;
    this._settings = settings;
    this._filter = filter ?? (NodeFilter) (m => FilterResult.Accept);
    this._current = this._root;
  }

  public INode Root => this._root;

  public FilterSettings Settings => this._settings;

  public NodeFilter Filter => this._filter;

  public INode Current
  {
    get => this._current;
    set => this._current = value;
  }

  public INode ToNext()
  {
    INode node = this._current;
    FilterResult filterResult = FilterResult.Accept;
    while (node != null)
    {
      while (filterResult != FilterResult.Reject && node.HasChildNodes)
      {
        node = node.FirstChild;
        filterResult = this.Check(node);
        if (filterResult == FilterResult.Accept)
        {
          this._current = node;
          return node;
        }
      }
      for (; node != this._root; node = node.Parent)
      {
        INode nextSibling = node.NextSibling;
        if (nextSibling != null)
        {
          node = nextSibling;
          break;
        }
      }
      if (node != this._root)
      {
        filterResult = this.Check(node);
        if (filterResult == FilterResult.Accept)
        {
          this._current = node;
          return node;
        }
      }
      else
        break;
    }
    return (INode) null;
  }

  public INode ToPrevious()
  {
    INode node = this._current;
    while (node != null && node != this._root)
    {
      INode previousSibling = node.PreviousSibling;
label_6:
      while (previousSibling != null)
      {
        node = previousSibling;
        FilterResult filterResult = this.Check(node);
        do
        {
          if (filterResult != FilterResult.Reject && node.HasChildNodes)
          {
            node = node.LastChild;
            filterResult = this.Check(node);
          }
          else
            goto label_6;
        }
        while (filterResult != FilterResult.Accept);
        this._current = node;
        return node;
      }
      if (node != this._root && node.Parent != null)
      {
        if (this.Check(node) == FilterResult.Accept)
        {
          this._current = node;
          return node;
        }
      }
      else
        break;
    }
    return (INode) null;
  }

  public INode ToParent()
  {
    INode node = this._current;
    while (node != null && node != this._root)
    {
      node = node.Parent;
      if (node != null && this.Check(node) == FilterResult.Accept)
      {
        this._current = node;
        return node;
      }
    }
    return (INode) null;
  }

  public INode ToFirst()
  {
    INode node = this._current?.FirstChild;
    while (node != null)
    {
      switch (this.Check(node))
      {
        case FilterResult.Accept:
          this._current = node;
          return node;
        case FilterResult.Skip:
          INode firstChild = node.FirstChild;
          if (firstChild != null)
          {
            node = firstChild;
            continue;
          }
          break;
      }
      INode parent;
      for (; node != null; node = parent)
      {
        INode nextSibling = node.NextSibling;
        if (nextSibling != null)
        {
          node = nextSibling;
          break;
        }
        parent = node.Parent;
        if (parent == null || parent == this._root || parent == this._current)
        {
          node = (INode) null;
          break;
        }
      }
    }
    return (INode) null;
  }

  public INode ToLast()
  {
    INode node = this._current?.LastChild;
    while (node != null)
    {
      switch (this.Check(node))
      {
        case FilterResult.Accept:
          this._current = node;
          return node;
        case FilterResult.Skip:
          INode lastChild = node.LastChild;
          if (lastChild != null)
          {
            node = lastChild;
            continue;
          }
          break;
      }
      INode parent;
      for (; node != null; node = parent)
      {
        INode previousSibling = node.PreviousSibling;
        if (previousSibling != null)
        {
          node = previousSibling;
          break;
        }
        parent = node.Parent;
        if (parent == null || parent == this._root || parent == this._current)
        {
          node = (INode) null;
          break;
        }
      }
    }
    return (INode) null;
  }

  public INode ToPreviousSibling()
  {
    INode node1 = this._current;
    if (node1 != this._root)
    {
      while (node1 != null)
      {
        INode node2 = node1.PreviousSibling;
        while (node2 != null)
        {
          node1 = node2;
          FilterResult filterResult = this.Check(node1);
          if (filterResult == FilterResult.Accept)
          {
            this._current = node1;
            return node1;
          }
          node2 = node1.LastChild;
          if (filterResult == FilterResult.Reject || node2 == null)
            node2 = node1.PreviousSibling;
        }
        node1 = node1.Parent;
        if (node1 == null || node1 == this._root || this.Check(node1) == FilterResult.Accept)
          break;
      }
    }
    return (INode) null;
  }

  public INode ToNextSibling()
  {
    INode node1 = this._current;
    if (node1 != this._root)
    {
      while (node1 != null)
      {
        INode node2 = node1.NextSibling;
        while (node2 != null)
        {
          node1 = node2;
          FilterResult filterResult = this.Check(node1);
          if (filterResult == FilterResult.Accept)
          {
            this._current = node1;
            return node1;
          }
          node2 = node1.FirstChild;
          if (filterResult == FilterResult.Reject || node2 == null)
            node2 = node1.NextSibling;
        }
        node1 = node1.Parent;
        if (node1 == null || node1 == this._root || this.Check(node1) == FilterResult.Accept)
          break;
      }
    }
    return (INode) null;
  }

  private FilterResult Check(INode node)
  {
    return !this._settings.Accepts(node) ? FilterResult.Skip : this._filter(node);
  }
}
