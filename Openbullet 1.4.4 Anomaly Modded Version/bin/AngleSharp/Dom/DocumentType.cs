// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DocumentType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class DocumentType : 
  Node,
  IDocumentType,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode
{
  internal DocumentType(Document owner, string name)
    : base(owner, name, NodeType.DocumentType)
  {
    this.PublicIdentifier = string.Empty;
    this.SystemIdentifier = string.Empty;
  }

  public IElement PreviousElementSibling
  {
    get
    {
      Node parent = this.Parent;
      if (parent != null)
      {
        bool flag = false;
        for (int index = parent.ChildNodes.Length - 1; index >= 0; --index)
        {
          if (parent.ChildNodes[index] == this)
            flag = true;
          else if (flag && parent.ChildNodes[index] is IElement)
            return (IElement) parent.ChildNodes[index];
        }
      }
      return (IElement) null;
    }
  }

  public IElement NextElementSibling
  {
    get
    {
      Node parent = this.Parent;
      if (parent != null)
      {
        int length = parent.ChildNodes.Length;
        bool flag = false;
        for (int index = 0; index < length; ++index)
        {
          if (parent.ChildNodes[index] == this)
            flag = true;
          else if (flag && parent.ChildNodes[index] is IElement)
            return (IElement) parent.ChildNodes[index];
        }
      }
      return (IElement) null;
    }
  }

  public IEnumerable<Entity> Entities => Enumerable.Empty<Entity>();

  public IEnumerable<Notation> Notations => Enumerable.Empty<Notation>();

  public string Name => this.NodeName;

  public string PublicIdentifier { get; set; }

  public string SystemIdentifier { get; set; }

  public string InternalSubset { get; set; }

  public void Before(params INode[] nodes) => this.InsertBefore(nodes);

  public void After(params INode[] nodes) => this.InsertAfter(nodes);

  public void Replace(params INode[] nodes) => this.ReplaceWith(nodes);

  public void Remove() => this.RemoveFromParent();

  public override Node Clone(Document owner, bool deep)
  {
    DocumentType target = new DocumentType(owner, this.Name)
    {
      PublicIdentifier = this.PublicIdentifier,
      SystemIdentifier = this.SystemIdentifier,
      InternalSubset = this.InternalSubset
    };
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }

  protected override string LocateNamespace(string prefix) => (string) null;

  protected override string LocatePrefix(string namespaceUri) => (string) null;
}
