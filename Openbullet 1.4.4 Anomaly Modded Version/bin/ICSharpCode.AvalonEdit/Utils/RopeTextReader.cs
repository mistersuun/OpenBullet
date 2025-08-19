// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.RopeTextReader
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public sealed class RopeTextReader : TextReader
{
  private Stack<RopeNode<char>> stack = new Stack<RopeNode<char>>();
  private RopeNode<char> currentNode;
  private int indexInsideNode;

  public RopeTextReader(Rope<char> rope)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    rope.root.Publish();
    if (rope.Length == 0)
      return;
    this.currentNode = rope.root;
    this.GoToLeftMostLeaf();
  }

  private void GoToLeftMostLeaf()
  {
    while (this.currentNode.contents == null)
    {
      if (this.currentNode.height == (byte) 0)
      {
        this.currentNode = this.currentNode.GetContentNode();
      }
      else
      {
        this.stack.Push(this.currentNode.right);
        this.currentNode = this.currentNode.left;
      }
    }
  }

  public override int Peek()
  {
    return this.currentNode == null ? -1 : (int) this.currentNode.contents[this.indexInsideNode];
  }

  public override int Read()
  {
    if (this.currentNode == null)
      return -1;
    char content = this.currentNode.contents[this.indexInsideNode++];
    if (this.indexInsideNode >= this.currentNode.length)
      this.GoToNextNode();
    return (int) content;
  }

  private void GoToNextNode()
  {
    if (this.stack.Count == 0)
    {
      this.currentNode = (RopeNode<char>) null;
    }
    else
    {
      this.indexInsideNode = 0;
      this.currentNode = this.stack.Pop();
      this.GoToLeftMostLeaf();
    }
  }

  public override int Read(char[] buffer, int index, int count)
  {
    if (this.currentNode == null)
      return 0;
    int length = this.currentNode.length - this.indexInsideNode;
    if (count < length)
    {
      Array.Copy((Array) this.currentNode.contents, this.indexInsideNode, (Array) buffer, index, count);
      this.indexInsideNode += count;
      return count;
    }
    Array.Copy((Array) this.currentNode.contents, this.indexInsideNode, (Array) buffer, index, length);
    this.GoToNextNode();
    return length;
  }
}
