// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.NodeEnumerable
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class NodeEnumerable : IEnumerable<INode>, IEnumerable
{
  private readonly INode _startingNode;

  public NodeEnumerable(INode startingNode) => this._startingNode = startingNode;

  public IEnumerator<INode> GetEnumerator()
  {
    return (IEnumerator<INode>) new NodeEnumerable.NodeEnumerator(this._startingNode);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  private class NodeEnumerator : IEnumerator<INode>, IDisposable, IEnumerator
  {
    private readonly Stack<NodeEnumerable.NodeEnumerator.EnumerationFrame> _frameStack;

    public NodeEnumerator(INode startingNode)
    {
      this._frameStack = new Stack<NodeEnumerable.NodeEnumerator.EnumerationFrame>();
      this.TryPushFrame(startingNode, 0);
    }

    public INode Current { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext()
    {
      if (this._frameStack.Count <= 0)
        return false;
      NodeEnumerable.NodeEnumerator.EnumerationFrame enumerationFrame = this._frameStack.Pop();
      this.Current = enumerationFrame.Parent.ChildNodes[enumerationFrame.ChildIndex];
      this.TryPushFrame(enumerationFrame.Parent, enumerationFrame.ChildIndex + 1);
      this.TryPushFrame(this.Current, 0);
      return true;
    }

    private void TryPushFrame(INode parent, int childIndex)
    {
      if (childIndex >= parent.ChildNodes.Length)
        return;
      this._frameStack.Push(new NodeEnumerable.NodeEnumerator.EnumerationFrame()
      {
        Parent = parent,
        ChildIndex = childIndex
      });
    }

    public void Dispose()
    {
    }

    public void Reset() => throw new NotSupportedException();

    private struct EnumerationFrame
    {
      public INode Parent;
      public int ChildIndex;
    }
  }
}
