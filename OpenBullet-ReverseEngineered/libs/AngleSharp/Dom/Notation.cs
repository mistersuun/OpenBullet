// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Notation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Notation")]
public sealed class Notation(Document owner, string name) : Node(owner, name, NodeType.Notation)
{
  [DomName("publicId")]
  public string PublicId { get; set; }

  [DomName("systemId")]
  public string SystemId { get; set; }

  public override Node Clone(Document newOwner, bool deep)
  {
    Notation target = new Notation(newOwner, this.NodeName);
    this.CloneNode((Node) target, newOwner, deep);
    return (Node) target;
  }
}
